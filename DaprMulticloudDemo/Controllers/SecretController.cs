using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DaprMulticloudDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecretController : ControllerBase
    {
        private readonly ILogger<SecretController> _logger;
        private readonly DaprClient _daprClient;

        public SecretController(ILogger<SecretController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<string> GetSecret()
        {
            var secret = await _daprClient.GetSecretAsync("demosecrets", "notsosecret");
            _logger.LogInformation("Retrieved secret 'notsosecret' from dapr secret store 'demosecrets'");

            return $"Dapr secret store returns: {secret["notsosecret"]}";
        }
    }
}
