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
    Task Send(CancellationToken cancellationToken = default);
}

public interface ISpeMemberRequestBuilder<TService, TResult>
{
    Task<TResult?> Send(CancellationToken cancellationToken = default);
}
