﻿using System;
using System.Threading.Tasks;

namespace SmartHouse.Lib
{
	public class TcpCommands
	{
		public class Pandora 
		{
			public const string PANDORA_START = "Pandora start";
			public const string PANDORA_STOP = "Pandora stop";
			public const string PANDORA_RESTART = "Pandora restart";
			public const string PANDORA_SONG_INFO = "Pandora song info";
		}

		public class SmartHouse
		{
			public const string RESTART_VPN = "Smart House - Restart VPN"; 
			public const string PLAY_ALARM = "Smart House - Play Alarm";
			public const string TIMER = "Smart House - Timer";
		}

		public async Task<Result> ExecuteTcpCommand(string command)
		{
			switch (command)
			{
				case Pandora.PANDORA_START:
					return await new PandoraService().Start();
				case Pandora.PANDORA_STOP:
					return new PandoraService().Stop();
				case Pandora.PANDORA_RESTART:
					return await new PandoraService().Restart();
				case SmartHouse.RESTART_VPN:
					return await new SmartHouseService().RestartOpenVPNService();
				case SmartHouse.PLAY_ALARM:					
					return new SmartHouseService().PlayAlarm();
				case SmartHouse.TIMER:
					RunTimer();
					return new Result() { Ok = true, ErrorCode = 0, Message = "Ok" };
				default:
					throw new Exception($"Command {command} is not defined");
			}
		}		

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
		private async void RunTimer()
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
		{
			await new Timer().RunCommand();
		}
	}
}
