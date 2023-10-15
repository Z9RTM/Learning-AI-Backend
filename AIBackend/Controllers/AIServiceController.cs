using AIBackend.AIPlugins;
using Microsoft.AspNetCore.Mvc;

namespace AIBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AIServiceController : ControllerBase
{
    PageSuggestionService _service;
    public AIServiceController(PageSuggestionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<PageInfo?> execute()
    {
        return await _service.SuggestPageAsync("Counter");
    }
}
