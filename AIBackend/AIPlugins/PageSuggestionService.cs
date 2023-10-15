using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Planners;

namespace AIBackend.AIPlugins;

public class PageSuggestionService
{
    private IKernel _kernel;

    public PageSuggestionService(IKernel kernel, IPageInfoProvider pageInfoProvider)
    {
        ImportPagePlugin(kernel, pageInfoProvider.GetPages());
        _kernel = kernel;
    }

    public static void ImportPagePlugin(IKernel kernel, IEnumerable<PageInfo> pages)
    {
        foreach (var page in pages)
        {
            var json = JsonSerializer.Serialize(page);
            kernel.RegisterCustomFunction(
                SKFunction.FromNativeFunction(
                    () => json,
                    nameof(PageInfoProvider),
                    page.Name,
                    page.Description));
        }
    }

    public async Task<PageInfo?> SuggestPageAsync(string goal)
    {
        var planner = new ActionPlanner(_kernel);
        var plan = await planner.CreatePlanAsync(goal);
        if (!plan.HasNextStep)
        {
            return null;
        }

        var planResult = await plan.InvokeAsync(_kernel);

        return JsonSerializer.Deserialize<PageInfo>(planResult.GetValue<string>());
    }
}