using System.Collections.Concurrent;
using RazorLight;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace DSDD.Automations.Payments.Views;

public abstract class ShimTemplatePage<TModel>: TemplatePage<TModel>
{
    public IHtmlContent DisplayNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
    {
        DisplayAttribute? display = GetMemberAttribute<DisplayAttribute>(expression, out MemberInfo member);

        if (display is { Name: { } name })
            return new StringHtmlContent(name);
        return new StringHtmlContent(member.Name);
    }

    public IHtmlContent? DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression)
        where TValue : Enum
    {
        Func<TModel, TValue> @delegate = (Func<TModel, TValue>)_compilationCache.GetOrAdd(expression, expression => expression.Compile());
        TValue value = @delegate(Model);

        var displayAttribute = value.GetType()
            .GetMember(value.ToString()!)
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>();

        return new StringHtmlContent(displayAttribute?.GetName() ?? value.ToString()!);
    }

    private static ConcurrentDictionary<LambdaExpression, Delegate> _compilationCache = new();

    private static TAttribute? GetMemberAttribute<TAttribute>(LambdaExpression expression, out MemberInfo member)
        where TAttribute : Attribute
    {

        if (expression.Body is not MemberExpression memberExpression)
            throw new InvalidOperationException(
                $"Expected {nameof(MemberExpression)} but got {expression.Body.GetType().Name}!");

        member = memberExpression.Member;
        return member.GetCustomAttribute<TAttribute>();
    }
}