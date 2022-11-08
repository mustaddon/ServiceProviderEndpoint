using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;


internal class SpeMemberRequest<TService, TResult> : ISpeMemberRequest<TService, TResult>, ISpeMemberRequest<TService>
{
    public SpeMemberRequest(SpeClient client, LambdaExpression expression, Func<object?>? newValue = null)
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

    ISpeMemberRequest<TService> ISpeMemberRequest<TService>.Parameters(params Type?[] types)
    {
        _parameters = types; return this;
    }

    public ISpeMemberRequest<TService, TResult> Parameters(params Type?[] types)
    {
        _parameters = types; return this;
    }

    ISpeMemberRequest<TService, TResult> ISpeMemberRequest<TService, TResult>.ReturnType(Type type)
    {
        _returnType = type; return this;
    }

    Task ISpeMemberRequest<TService>.Send(CancellationToken cancellationToken) => Send(cancellationToken);

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
            if (_newValue == null)
                args = Array.Empty<object>();
            else
            {
                args = new[] { _newValue() };
                _returnType = Types.Object;
            }

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
}
