using Microsoft.Azure.Functions.Worker;
using System.Linq.Expressions;
using System.Reflection;

namespace DSDD.Automations.Payments.Durable;

public static class FunctionHelpers
{
    public static string GetFunctionName<TSource>(Expression<Action<TSource>> pick)
    {
        if (pick is not { Body: MethodCallExpression methodCall })
            throw new ArgumentException($"Parameter {nameof(pick)} must point to a method call.");

        if (methodCall.Method.GetCustomAttribute<FunctionAttribute>() is not { } functionAttribute)
            throw new ArgumentException($"Parameter {nameof(pick)} must point to a method call with {nameof(FunctionAttribute)}.");

        return functionAttribute.Name;
    }
}