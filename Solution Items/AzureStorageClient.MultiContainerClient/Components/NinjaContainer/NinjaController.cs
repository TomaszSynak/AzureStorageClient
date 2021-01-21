namespace AzureStorageClient.MultiContainerClient.Components.NinjaContainer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NinjaController : ControllerBase
    {
        private readonly IAzureBlobClient<NinjaSettings> _ninjaBlobClient;

        public NinjaController(IAzureBlobClient<NinjaSettings> ninjaBlobClient)
        {
            _ninjaBlobClient = ninjaBlobClient;
        }

        /// <summary>
        /// Endpoint to verify connection to Ninja blob container.
        /// </summary>
        /// <param name="ct"> Cancellation Token to pass </param>
        /// <response code="200"> Connection to Ninja container established </response>
        /// <response code="424"> Connection to Ninja container cannot be established </response>
        [HttpGet]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status424FailedDependency)]
        public async Task<IActionResult> Health(CancellationToken ct)
        {
            var isAccessible = await _ninjaBlobClient.IsAccessible(ct);

            return StatusCode(isAccessible ? StatusCodes.Status200OK : StatusCodes.Status424FailedDependency);
        }
    }
}
