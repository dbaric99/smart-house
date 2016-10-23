﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Lib
{
	public class TemperatureUdp
	{
		private TelemetryService service = new TelemetryService();

		public async Task StartListen()
		{
			await service.InitializeAzure();

			await Task.Run(async () =>
			{
				var sensors = new TelemetryService();

				using (var client = new UdpClient())
				{
					var localEp = new IPEndPoint(IPAddress.Any, 9876);
					client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					client.ExclusiveAddressUse = false;

					client.Client.Bind(localEp);

					while (true)
					{
						try
						{
							var buffer = client.Receive(ref localEp);
							var data = Encoding.ASCII.GetString(buffer);

							await sensors.SaveTemperatureUdp(data);
						}
						catch (Exception ex)
						{
							Logger.LogErrorMessage("Reading UDP", ex);
						}
					}
				}

			});
		}

	}
}