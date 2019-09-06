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
			return "Some nice string ... ";
		}

	}
}