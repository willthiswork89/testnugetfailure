using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace OTL.Api.Test
{
    [Route("error")]
    public class GlobalExceptionHandler : ControllerBase
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }
        [HttpGet("exceptionhandler")]
        public IActionResult Error()
        {
            try
            {
                // Get the details of the exception that occurred
                var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionFeature != null)
                {
                    // Get which route the exception occurred at
                    string routeWhereExceptionOccurred = exceptionFeature.Path;

                    // Get the exception that occurred
                    Exception exceptionThatOccurred = exceptionFeature.Error;

                    return BadRequest(
                        $"Uncaught/Unhandled exception in OTL.Api.ModelCams for route {routeWhereExceptionOccurred}: {exceptionThatOccurred.ToString()}");
                }
            }
            catch
            {
            }
            return BadRequest();
        }
    }
}