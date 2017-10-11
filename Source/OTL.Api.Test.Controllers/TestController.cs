using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace OTL.Api.Test.Controllers
{
	[Route("v1/[controller]")]
	[Produces("application/json")]
	[Consumes("application/json")]
	[Authorize]
	public class TestController : ControllerBase
	{
		private readonly ILogger _logger;

		public TestController(ILogger logger )
		{
			_logger = logger;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult SampleGet()
		{
			return Ok();
		}
		
		[HttpPost]
		[AllowAnonymous]
		public IActionResult SamplePost([FromHeader]string username)
		{
			return Ok();
		}

		[HttpGet("testmodelauth")]
		[Authorize(Roles = "Model")]
		public IActionResult ModelAuth()
		{
			return Ok();
		}

	    [HttpGet("testloggedin")]
	    public IActionResult TestLoggedIn()
	    {
	        return Ok();
	    }
	}
}