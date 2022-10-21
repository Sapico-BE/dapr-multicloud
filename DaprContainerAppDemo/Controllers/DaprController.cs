using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace DaprContainerAppDemo.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DaprController : ControllerBase
{
    private readonly ILogger<DaprController> _logger;
    private readonly DaprClient _daprClient;

    public DaprController(ILogger<DaprController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    [HttpGet(Name = "GetSecret")]
    public async Task<string> GetSecret()
    {
        var secret = await _daprClient.GetSecretAsync("secretstore", "notsosecret");
        _logger.LogInformation("Retrieved secret 'notsosecret' from dapr secret store 'secretstore'");

        return $"Dapr secret store returns: {secret["notsosecret"]}";
    }

    [HttpPost(Name = "SetState")]
    public async Task<IActionResult> SetState(string key, string state)
    {
        _logger.LogInformation("Set state in 'azurestatestore'");

        await _daprClient.SaveStateAsync("azurestatestore", key, state);

        return Ok();
    }

    [HttpGet(Name = "GetGetState")]
    public async Task<string> GetState(string key)
    {
        _logger.LogInformation($"Get state with key '{key}' in 'azurestatestore'");

        var state = await _daprClient.GetStateAsync<string>("azurestatestore", key);

        return $"Dapr state store returned '{state}' for key '{key}' in azurestatestore";
    }
}
