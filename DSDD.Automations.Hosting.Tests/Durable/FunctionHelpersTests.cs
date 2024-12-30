using System.Collections;
using System.Linq.Expressions;
using Microsoft.Azure.Functions.Worker;
using Sut = DSDD.Automations.Hosting.Durable.FunctionHelpers;

namespace DSDD.Automations.Hosting.Tests.Durable;

public class FunctionHelpersTests
{
    [Test]
    public void GetFunctionName()
    {
        // Setup
        Expression<Action<FunctionHelpersTests>> methodCallLambda = _ => _.TestFunc1();

        // Act
        string result = Sut.GetFunctionName(methodCallLambda);

        // Assert
        Assert.That(result, Is.EqualTo(TEST_FUNCT_1_NAME));
    }

    public static IEnumerable GetFunctionName_BadExpresison_Source
    {
        get
        {
            Expression<Action<FunctionHelpersTests>> methodCallLambda = _ => _.TestFunc2();
            Type expectedException = typeof(ArgumentException);

            yield return new object?[] { methodCallLambda, expectedException };

            methodCallLambda = _ => new object();

            yield return new object?[] { methodCallLambda, expectedException };
        }
    }

    [TestCaseSource(nameof(GetFunctionName_BadExpresison_Source))]
    public void GetFunctionName_BadExpresison(Expression<Action<FunctionHelpersTests>> methodCallLambda, Type exception)
    {
        // Act & assert
        Assert.Throws(exception, () => Sut.GetFunctionName(methodCallLambda));
    }

    [Function(TEST_FUNCT_1_NAME)]
    public Task TestFunc1() => Task.CompletedTask;
    
    public Task TestFunc2() => Task.CompletedTask;

    public const string TEST_FUNCT_1_NAME = "test1";
}