using Microsoft.AspNetCore.Mvc;
using ServiceObserver.Data;

namespace ServiceObserver.Controller
{
	// dev: 5004 prod: 3000
	[Route("observer/service")]
	[ApiController]
	public class ServiceObserverController : ControllerBase
	{

		private IDatabaseService db;

		public ServiceObserverController(IDatabaseService db)
		{
			this.db = db;
		}

		[HttpGet]
		public string getSomeString()
		{
			return "Hello there, I am observing, don't worry ... ";
		}

		[HttpGet]
		[Route("all")]
		public IActionResult getAllEvents()
		{

			return StatusCode(500);
		}

		[HttpGet]
		[Route("fromDate")]
		public IActionResult getFromDate([FromQuery] )
		{

			return StatusCode(500);
		}

		[HttpGet]
		[Route("latest")]
		public IActionResult getLatest()
		{

			return StatusCode(500);
		}


	}
}