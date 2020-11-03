using Microsoft.AspNetCore.Mvc;

namespace ServiceObserver.Controller
{
	// port 5003
	[Route("observer/service")]
	[ApiController]
	public class AccessController
	{

		[HttpGet]
		public string getSomeString()
		{
			return "Hello there, I am observing, don't worry ... ";
		}

	}
}