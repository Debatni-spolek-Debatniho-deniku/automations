using System.Linq.Expressions;
using Microsoft.DurableTask;

namespace DSDD.Automations.Hosting.Durable;

public static class TaskOrchestrationContextExtensions
{
    public static Task CallActivityFromMethodTaskAsync<TSource>(this TaskOrchestrationContext ctx, Expression<Action<TSource>> pick)
    {
        string name = FunctionHelpers.GetFunctionName(pick);
        return ctx.CallActivityAsync(name);
    }
}