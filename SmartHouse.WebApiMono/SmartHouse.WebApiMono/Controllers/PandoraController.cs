﻿using System;
using System.Web.Http;
using SmartHouse.Lib;

namespace SmartHouse.WebApiMono
{
	[RoutePrefix("api/Pandora")]
	public class PandoraController : BaseController
	{
		private PandoraCommand command;

		public PandoraController(ISettingsService service) : base(service)
		{
			command = new PandoraCommand();
		}

		[HttpGet]
		[Route("Play")]
		public Result Play()
		{
			return command.Play();
		}

		[HttpGet]
		[Route("Stop")]
		public Result Stop()
		{
			return command.Stop();
		}

		[HttpGet]
		[Route("Next")]
		public Result Next()
		{
			return command.Next();
		}

		[HttpGet]
		[Route("ThumbUp")]
		public Result ThumbUp()
		{
			return command.ThumbUp();
		}

		[HttpGet]
		[Route("ThumbDown")]
		public Result ThumbDown()
		{
			return command.ThumbDown();
		}

		[HttpGet]
		[Route("VolumeUp")]
		public Result VolumeUp()
		{
			return command.VolumeUp();
		}

		[HttpGet]
		[Route("VolumeDown")]
		public Result VolumeDown()
		{
			return command.VolumeDown();
		}

	}
}