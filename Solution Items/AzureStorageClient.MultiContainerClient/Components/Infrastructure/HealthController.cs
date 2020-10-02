namespace AzureStorageClient.MultiContainerClient.Components.Infrastructure
{
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ExternalResources _externalResources;

        public HealthController(IWebHostEnvironment webHostEnvironment, ExternalResources externalResources)
        {
            _webHostEnvironment = webHostEnvironment;
            _externalResources = externalResources;
        }

        private static string ApiVersion
            => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

        /// <summary>
        /// Endpoint to verify server's responsiveness.
        /// </summary>
        /// <param name="ct"> Cancellation Token to pass </param>
        /// <response code="200"> Server is responsive </response>
        /// <response code="424"> Server is unresponsive </response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthStateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status424FailedDependency)]
        public async Task<IActionResult> Health(CancellationToken ct)
        {
            var healthState = new HealthStateDto
            {
                Name = _webHostEnvironment.ApplicationName,
                Environment = _webHostEnvironment.EnvironmentName,
                ApiVersion = ApiVersion
            };

            var areNotAccessible = await _externalResources.AreNotAccessible(ct);
            if (areNotAccessible)
            {
                healthState.IsHealthy = false;
                return StatusCode(StatusCodes.Status424FailedDependency, healthState);
            }

            healthState.IsHealthy = true;
            return StatusCode(StatusCodes.Status200OK, healthState);
        }
    }
}
