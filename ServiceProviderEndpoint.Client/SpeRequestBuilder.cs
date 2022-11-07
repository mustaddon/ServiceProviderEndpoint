using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

internal class SpeRequestBuilder<TService> : ISpeRequestBuilder<TService>
{
    public SpeRequestBuilder(SpeClient client)
    {
        _client = client;
    }

    readonly SpeClient _client;


    public ISpeMemberRequestBuilder<TService> Member(Expression<Action<TService>> expression)
    {
        return new SpeMemberRequestBuilder<TService, object>(_client, expression);
    }

    public ISpeMemberRequestBuilder<TService> Member(Expression<Func<TService, Task>> expression)
    {
        return new SpeMemberRequestBuilder<TService, object>(_client, expression);
    }

    public ISpeMemberRequestBuilder<TService, TResult> Member<TResult>(Expression<Func<TService, Task<TResult>>> expression)
    {
        return new SpeMemberRequestBuilder<TService, TResult>(_client, expression);
    }
    public ISpeMemberRequestBuilder<TService, TResult> Member<TResult>(Expression<Func<TService, TResult>> expression)
    {
        return new SpeMemberRequestBuilder<TService, TResult>(_client, expression);
    }

    public ISpeMemberRequestBuilder<TService, TValue> Member<TValue>(Expression<Func<TService, TValue>> expression, TValue newValue)
    {
        return new SpeMemberRequestBuilder<TService, TValue>(_client, expression, () => newValue);
    }

    public ISpeMemberRequestBuilder<TService, TValue> Member<TValue>(Expression<Func<TService, Task<TValue>>> expression, TValue newValue)
    {
        return new SpeMemberRequestBuilder<TService, TValue>(_client, expression, () => newValue);
    }
}

internal class SpeMemberRequestBuilder<TService, TResult> : ISpeMemberRequestBuilder<TService, TResult>, ISpeMemberRequestBuilder<TService>
{
    public SpeMemberRequestBuilder(SpeClient client, LambdaExpression expression, Func<object?>? newValue = null)
    {
        _client = client;
        _expression = expression;
        _newValue = newValue;
    }

    private readonly SpeClient _client;
    private readonly LambdaExpression _expression;
    private readonly Func<object?>? _newValue;
    private Type?[]? _parameters;
    private Type? _returnType;

    Task ISpeMemberRequestBuilder<TService>.Send(CancellationToken cancellationToken) => Send(cancellationToken);

    public async Task<TResult?> Send(CancellationToken cancellationToken = default)
    {
        MemberInfo? member = null;
        object?[]? args = null;

        if (_expression.Body is MethodCallExpression methodCall)
        {
            args = methodCall.Arguments.Select(GetArgumentValue).ToArray();
            member = methodCall.Method;
        }
        else if (_expression.Body is MemberExpression memberExpr)
        {
            args = _newValue != null ? new[] { _newValue() } : Array.Empty<object>();
            member = memberExpr.Member;
        }

        if (member == null || args == null)
            throw new InvalidOperationException("Failed to build a query for this member expression");

        var requestTask = _client.CreateRequest(typeof(TService), member, _parameters, args, cancellationToken);
        var resultTask = _client.GetResult(requestTask, _returnType ?? typeof(TResult), cancellationToken);

        return (TResult?)(await resultTask);
    }

    private static object? GetArgumentValue(Expression element)
    {
        if (element is ConstantExpression constantExpression)
            return constantExpression.Value;

        return Expression.Lambda(Expression.Convert(element, element.Type))
            .Compile().DynamicInvoke();
    }


    ISpeMemberRequestBuilder<TService> ISpeMemberRequestBuilder<TService>.Parameters(params Type?[] types)
    {
        _parameters = types; return this;
    }

    public ISpeMemberRequestBuilder<TService, TResult> Parameters(params Type?[] types)
    {
        _parameters = types; return this;
    }

    public ISpeMemberRequestBuilder<TService> ReturnType(Type type)
    {
        _returnType = type; return this;
    }

    ISpeMemberRequestBuilder<TService, TResult> ISpeMemberRequestBuilder<TService, TResult>.ReturnType(Type type)
    {
        _returnType = type; return this;
    }
}