using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

public interface ISpeRequestBuilder<TService>
{
    ISpeMemberRequestBuilder<TService> Member(Expression<Action<TService>> expression);
    ISpeMemberRequestBuilder<TService> Member(Expression<Func<TService, Task>> expression);
    ISpeMemberRequestBuilder<TService, TResult> Member<TResult>(Expression<Func<TService, Task<TResult>>> expression);
    ISpeMemberRequestBuilder<TService, TResult> Member<TResult>(Expression<Func<TService, TResult>> expression);
    ISpeMemberRequestBuilder<TService, TValue> Member<TValue>(Expression<Func<TService, Task<TValue>>> expression, TValue newValue);
    ISpeMemberRequestBuilder<TService, TValue> Member<TValue>(Expression<Func<TService, TValue>> expression, TValue newValue);
}

public interface ISpeMemberRequestBuilder<TService>
{
    ISpeMemberRequestBuilder<TService> Parameters(params Type?[] types);
    ISpeMemberRequestBuilder<TService> ReturnType(Type type);
    Task Send(CancellationToken cancellationToken = default);
}

public interface ISpeMemberRequestBuilder<TService, TResult>
{
    ISpeMemberRequestBuilder<TService, TResult> Parameters(params Type?[] types);
    ISpeMemberRequestBuilder<TService, TResult> ReturnType(Type type);
    Task<TResult?> Send(CancellationToken cancellationToken = default);
}
