﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHouse.Lib;
using SmartHouse.UWPLib.Model;

namespace SmartHouse.UWPLib.Service
{
    public class SmartHouseService
    {
        private readonly SettingsService settingsService;

        public SmartHouseService()
        {
            settingsService = SettingsService.Instance;
        }

        public async Task<Result> Run(SmartHouseCommands command)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/{command}";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> RestartOpenVPN()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/RestartOpenVPN";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> SetMode(string mode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/SetMode?Mode={mode}";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> SetInput(string input)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/{input}";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<string> GetCurrentState()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/GetCurrentState";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<string>(json);
            }
        }

        public async Task<Result> LoveSong()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Remote/Love";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> BanSong()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Remote/Ban";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> TiredOfThisSong()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Remote/TiredOfThisSong";
                var json = await client.GetStringAsync(uri);

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> TurnOnAirConditioner()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Sensor/AirCondition?On=1";

                var response = await client.PostAsync(uri, null);
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task<Result> TurnOffAirConditioner()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Sensor/AirCondition?On=0";

                var response = await client.PostAsync(uri, null);
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Result>(json);
            }
        }

        public async Task PhoneCallStarted(PhoneCallData model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/PhoneCallStarted";

                var modelString = JsonConvert.SerializeObject(model);
                var content = new StringContent(modelString, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content);
                var json = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task UploadContent(ContentUploadModel model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/UploadContent";

                var modelString = JsonConvert.SerializeObject(model);
                var content = new StringContent(modelString, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task PhoneCallEnded()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/PhoneCallEnded";

                var response = await client.PostAsync(uri, null);
                var json = await response.Content.ReadAsStringAsync();
            }
        }

	    public async Task<TelemetryData> GetRoomTemperature()
	    {
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Sensor/GetCurrentTemperature";

				var json = await client.GetStringAsync(uri);
				return JsonConvert.DeserializeObject<TelemetryData>(json);
			}
		}

	    public async Task<SongResult> GetCurrentSong()
	    {
		    using (var client = new HttpClient())
		    {
			    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

			    var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/NowPlaying";

			    var json = await client.GetStringAsync(uri);
			    return JsonConvert.DeserializeObject<SongResult>(json);
		    }
	    }

        public async Task<DashboardData> GetDashboardData()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/SmartHouse/Dashboard";

                var json = await client.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<DashboardData>(json);
            }
        }

        public async Task SetVolume(double volume)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"http://{settingsService.HostIP}:{settingsService.HostPort}/api/Yamaha/SetVolume?volume={volume}";

                var json = await client.GetStringAsync(uri);
            }
        }
	}
}
