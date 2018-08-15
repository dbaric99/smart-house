﻿using SmartHouse.Lib;
using SmartHouse.UWPLib.BLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using SmartHouse.UWPLib.Service;

namespace SmartHouse.UWPClient.VoiceCommands
{
    public sealed class SmartHouseVoiceCommandService : IBackgroundTask
    {
        /// <summary>
        /// the service connection is maintained for the lifetime of a cortana session, once a voice command
        /// has been triggered via Cortana.
        /// </summary>
        VoiceCommandServiceConnection voiceServiceConnection;

        /// <summary>
        /// Lifetime of the background service is controlled via the BackgroundTaskDeferral object, including
        /// registering for cancellation events, signalling end of execution, etc. Cortana may terminate the 
        /// background service task if it loses focus, or the background task takes too long to provide.
        /// 
        /// Background tasks can run for a maximum of 30 seconds.
        /// </summary>
        BackgroundTaskDeferral serviceDeferral;

        /// <summary>
        /// Background task entrypoint. Voice Commands using the <VoiceCommandService Target="...">
        /// tag will invoke this when they are recognized by Cortana, passing along details of the 
        /// invocation. 
        /// 
        /// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
        /// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
        /// input). There is no execution time limit on the background task managed by Cortana,
        /// but developers should use plmdebug (https://msdn.microsoft.com/en-us/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
        /// on the Cortana app package in order to prevent Cortana timing out the task during
        /// debugging.
        /// 
        /// Cortana dismisses its UI if it loses focus. This will cause it to terminate the background
        /// task, even if the background task is being debugged. Use of Remote Debugging is recommended
        /// in order to debug background task behaviors. In order to debug background tasks, open the
        /// project properties for the app package (not the background task project), and enable
        /// Debug -> "Do not launch, but debug my code when it starts". Alternatively, add a long
        /// initial progress screen, and attach to the background task process while it executes.
        /// </summary>
        /// <param name="taskInstance">Connection to the hosting background service process.</param>
        /// 
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Starting backgruond task...");
            serviceDeferral = taskInstance.GetDeferral();

            // Register to receive an event if Cortana dismisses the background task. This will
            // occur if the task takes too long to respond, or if Cortana's UI is dismissed.
            // Any pending operations should be cancelled or waited on to clean up where possible.
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            // This should match the uap:AppService and VoiceCommandService references from the 
            // package manifest and VCD files, respectively. Make sure we've been launched by
            // a Cortana Voice Command.
            if (triggerDetails != null && triggerDetails.Name == nameof(SmartHouseVoiceCommandService))
            {
                try
                {
                    voiceServiceConnection =
                        VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                            triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;
                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    // Depending on the operation (defined in SmartHouse:SmartHouseCommands.xml)
                    // perform the appropriate command.
                    switch (voiceCommand.CommandName)
                    {
                        case "pandoraCommands":
                            var command = voiceCommand.Properties["command"][0];
                            await ExecutePandoraCommands(command);
                            break;
                        case "smartHouseTurnOnCommand":
                            await ExecuteSmarthouseCommands("Turn on");
                            break;
                        case "smartHouseTurnOffCommand":
                            await ExecuteSmarthouseCommands("Turn off");
                            break;
                        case "smartHouseVolumeUp":
                            await ExecuteSmarthouseCommands("Volume up");
                            break;
                        case "smartHouseVolumeDown":
                            await ExecuteSmarthouseCommands("Volume down");
                            break;
                        case "showTemperature":
                            await CurrentTempeature();
                            break;
                        case "nowPlaying":
                            await NowPlaying();
                            break;
                        case "smartHouseMode":
                            var mode = voiceCommand.Properties["mode"][0];
                            await SetSmartHouseMode(mode);
                            break;
                        case "smartHouseInput":
                            var input = voiceCommand.Properties["input"][0];
                            await SetSmartHouseSetInput(input);
                            break;
                        case "smartHouseCurrentInput":
                            await SmartHouseGetInput();
                            break;
                        case "smartHouseRestartVPN":
                            await SmartHouseRestartVPN();
                            break;
                        case "smartHouseNextStation":
                            await ExecutePandoraCommands("NextStation");
                            break;
                        case "smartHousePrevStation":
                            await ExecutePandoraCommands("PrevStation");
                            break;
                        case "turnOnAirConditioner":
                            await TurnOnAirCondition();
                            break;
                        case "turnOffAirConditioner":
                            await TurnOffAirCondition();
                            break;
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await CompleteMessageError(ex.Message);
                    Debug.WriteLine("Handling Voice Command failed " + ex);
                }
            }
        }

        private async Task ExecuteSmarthouseCommands(string command)
        {
            var smartHouse = new SmartHouseService();

            switch (command)
            {
                case "Turn on":
                    await ShowProgressScreen($"Please wait");
                    var result = await smartHouse.Run(UWPLib.Model.SmartHouseCommands.TurnOn);
                    await CompleteMessage(result.Message);
                    break;
                case "Turn off":
                    await ShowProgressScreen($"Please wait");
                    result = await smartHouse.Run(UWPLib.Model.SmartHouseCommands.TurnOff);
                    await CompleteMessage(result.Message);
                    break;
                case "Volume up":	                
                    await VolumeUp();
                    break;
                case "Volume down":	                
                    await VolumeDown();
                    break;
                default:
                    LaunchAppInForeground();
                    break;
            }
        }

        private async Task SmartHouseRestartVPN()
        {
            var smartHouse = new SmartHouseService();
            await ShowProgressScreen("Please Wait...");

            var result = await smartHouse.RestartOpenVPN();
            await CompleteMessage("");
        }

        private async Task SetSmartHouseMode(string mode)
        {
            var smartHouse = new SmartHouseService();
            await ShowProgressScreen($"Setting {mode} mode");

            var result = await smartHouse.SetMode(mode);
            await CompleteMessage("");
        }

        private async Task SetSmartHouseSetInput(string input)
        {
            var smartHouse = new SmartHouseService();
            await ShowProgressScreen($"Setting {input} input");

            var result = await smartHouse.SetInput(input);
            await CompleteMessage("");
        }

        private async Task SmartHouseGetInput()
        {
            var smartHouse = new SmartHouseService();
            await ShowProgressScreen("Please wait...");

            var result = await smartHouse.GetCurrentState();
            await CompleteMessage($"Connected input: {result}");
        }

        private async Task TurnOnAirCondition()
        {
            var smartHouse = new SmartHouseService();

            await ShowProgressScreen("Please wait...");
            var result = await smartHouse.TurnOnAirConditioner();
            await CompleteMessage($"Air conditioner {result?.Message}");
        }

        private async Task TurnOffAirCondition()
        {
            var smartHouse = new SmartHouseService();

            await ShowProgressScreen("Please wait...");
            var result = await smartHouse.TurnOffAirConditioner();
            await CompleteMessage($"Air conditioner {result?.Message}");
        }

        private async Task NowPlaying()
        {
            var smartHouse = new SmartHouseService();

            await ShowProgressScreen("Of course");
            var result = await smartHouse.GetCurrentSong();
            await CortanaResponseShowSong(result);
        }

        private async Task CurrentTempeature()
        {
            var smartHouse = new SmartHouseService();

            await ShowProgressScreen("Please wait...");
            var result = await smartHouse.GetRoomTemperature();

            var message = $"Tempearture is at {result.Temperature} deggree, humidity {result.Humidity} percent, and gas is {result.GasValue}.";
            if (result.GasValue > 250)
                message += " Phill, Gas value is high... You smoking to much.";

            await CompleteMessage(message);
        }

        private async Task ExecutePandoraCommands(string command)
        {
            var pandora = new PandoraService();
            var smartHouse = new SmartHouseService();
            await ShowProgressScreen($"Starting to {command} song");

            switch (command)
            {
                case "Play":
                    await smartHouse.Run(UWPLib.Model.SmartHouseCommands.Play);
                    await CompleteMessage("");
                    break;
                case "Stop":
                    await pandora.Run(SmartHouse.UWPLib.Model.PandoraCommands.Play);
                    await CompleteMessage("");
                    break;
                case "Next":
                    await smartHouse.Run(UWPLib.Model.SmartHouseCommands.Next);
                    await CompleteMessage("");
                    break;
                case "Love":
                    await smartHouse.LoveSong();
                    await CompleteMessage("");
                    break;
                case "Hate":
                    await pandora.Run(SmartHouse.UWPLib.Model.PandoraCommands.ThumbDown);
                    await CompleteMessage("");
                    break;
                case "Boring":
                    await pandora.Run(SmartHouse.UWPLib.Model.PandoraCommands.Tired);
                    await CompleteMessage("");
                    break;
                case "Show":
                    var info = await smartHouse.GetCurrentSong();
                    await CortanaResponseShowSong(info);
                    break;
                case "NextStation":
                    var nextInfo = await pandora.Run(SmartHouse.UWPLib.Model.PandoraCommands.NextStation);
                    await CompleteMessage(nextInfo?.Message);
                    break;
                case "PrevStation":
                    var prevInfo = await pandora.Run(SmartHouse.UWPLib.Model.PandoraCommands.PrevStation);
                    await CompleteMessage(prevInfo?.Message);
                    break;
                default:
                    LaunchAppInForeground();
                    break;
            }
        }

        private async Task CompleteMessage(string message)
        {
            // Provide a completion message to the user.
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = userMessage.SpokenMessage = message;
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async Task CompleteMessageError(string message)
        {
            // Provide a completion message to the user.
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = userMessage.SpokenMessage = $"Error message: {message}";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportFailureAsync(response);
        }

        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launcing Smart House Application";

            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private async Task VolumeUp()
        {
            await ShowProgressScreen($"Please wait");

            var service = new SmartHouseService();
            var data = await service.GetDashboardData();

            var newVolume = (data.Volume / 10D) + 2D;
            await service.SetVolume(newVolume);

            await CompleteMessage($"Volume is at {newVolume}");
        }

        private async Task VolumeDown()
        {
            await ShowProgressScreen($"Please wait");

            var service = new SmartHouseService();
            var data = await service.GetDashboardData();

            var newVolume = (data.Volume / 10D) - 2D;
            await service.SetVolume(newVolume);

            await CompleteMessage($"Volume is at {newVolume}");
        }

        private async Task CortanaResponseShowSong(SongResult info)
        {
            if (info == null)
            {
                await CompleteMessage("Right now... Nothing is playing");
            }
            else if (info.Loved)
            {
                var message = $"Now is playing: {info.Song}, from artist {info.Artist}, on album {info.Album}. You liked this song.";
                await CompleteMessage(message);
            }
            else
            {
                var message = $"Now is playing: {info.Song}, from artist {info.Artist}, on album {info.Album}. Do you like this song?";

                var userPrompt = new VoiceCommandUserMessage();
                userPrompt.DisplayMessage = userPrompt.SpokenMessage = message;

                var userReprompt = new VoiceCommandUserMessage();
                var prompt = "Do you like this song?";
                userReprompt.DisplayMessage = userReprompt.SpokenMessage = prompt;

                var response = VoiceCommandResponse.CreateResponseForPrompt(userPrompt, userReprompt);
                var voiceCommandConfirmation = await voiceServiceConnection.RequestConfirmationAsync(response);

                if (voiceCommandConfirmation == null)
                {
                    response = VoiceCommandResponse.CreateResponse(userPrompt);
                    await voiceServiceConnection.ReportSuccessAsync(response);
                }
                else if(voiceCommandConfirmation.Confirmed)
                {
                    var service = new SmartHouseService();
                    var result = await service.LoveSong();

                    await CompleteMessage(result?.Message);
                }
                else
                {
                    await CompleteMessage("");
                }
            }
        }

        /// <summary>
        /// Show a progress screen. These should be posted at least every 5 seconds for a 
        /// long-running operation, such as accessing network resources over a mobile 
        /// carrier network.
        /// </summary>
        /// <param name="message">The message to display, relating to the task being performed.</param>
        /// <returns></returns>
        private async Task ShowProgressScreen(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }

        /// <summary>
        /// Handle the completion of the voice command. Your app may be cancelled
        /// for a variety of reasons, such as user cancellation or not providing 
        /// progress to Cortana in a timely fashion. Clean up any pending long-running
        /// operations (eg, network requests).
        /// </summary>
        /// <param name="sender">The voice connection associated with the command.</param>
        /// <param name="args">Contains an Enumeration indicating why the command was terminated.</param>
        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            this.serviceDeferral?.Complete();
        }

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            this.serviceDeferral?.Complete();
        }
    }
}
