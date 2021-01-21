namespace AzureStorageClient.MultiContainerClient.Components.SamuraiContainer
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
    public class SamuraiController : ControllerBase
    {
        private readonly IAzureBlobClient<SamuraiSettings> _samuraiBlobClient;

        public SamuraiController(IAzureBlobClient<SamuraiSettings> samuraiBlobClient)
        {
            _samuraiBlobClient = samuraiBlobClient;
        }

        /// <summary>
        /// Endpoint to verify connection to Samurai blob container.
        /// </summary>
        /// <param name="ct"> Cancellation Token to pass </param>
        /// <response code="200"> Connection to Samurai container established </response>
        /// <response code="424"> Connection to Samurai container cannot be established </response>
        [HttpGet]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status424FailedDependency)]
        public async Task<IActionResult> Health(CancellationToken ct)
        {
            var isAccessible = await _samuraiBlobClient.IsAccessible(ct);

            return StatusCode(isAccessible ? StatusCodes.Status200OK : StatusCodes.Status424FailedDependency);
        }
    }
}
