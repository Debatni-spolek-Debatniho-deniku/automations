using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace DSDD.Automations.Hosting.Razor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRazorRenderer(this IServiceCollection services, params CompiledRazorAssemblyPart[] viewContainingParts)
    {
        IList<ApplicationPart> parts = services
            .AddMvcCore()
            .AddViews()
            .AddRazorViewEngine()
            .PartManager
            .ApplicationParts;

        foreach (ApplicationPart viewContainingPart in viewContainingParts)
            parts.Add(viewContainingPart);

        return services.AddTransient<IRazorRenderer, RazorRenderer>();
    }
}