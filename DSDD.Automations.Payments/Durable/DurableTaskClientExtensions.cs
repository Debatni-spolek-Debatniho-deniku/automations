using System.Linq.Expressions;
using Microsoft.DurableTask.Client;

namespace DSDD.Automations.Payments.Durable;

public static class DurableTaskClientExtensions
{
    public static Task<string> ScheduleNewMethodOrchestrationInstanceAsync<TSource>(this DurableTaskClient client, Expression<Action<TSource>> pick)
    {
        string name = FunctionHelpers.GetFunctionName(pick);
        return client.ScheduleNewOrchestrationInstanceAsync(name);
    }
}