using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;


public interface ISpeServiceRequest<TService>
{
    ISpeMemberRequest<TService> Member(Expression<Action<TService>> expression);
    ISpeMemberRequest<TService> Member(Expression<Func<TService, Task>> expression);
    ISpeMemberRequest<TService, TResult> Member<TResult>(Expression<Func<TService, Task<TResult>>> expression);
    ISpeMemberRequest<TService, TResult> Member<TResult>(Expression<Func<TService, TResult>> expression);
    ISpeMemberRequest<TService> Member<TValue>(Expression<Func<TService, Task<TValue>>> expression, TValue newValue);
    ISpeMemberRequest<TService> Member<TValue>(Expression<Func<TService, TValue>> expression, TValue newValue);
}

public interface ISpeMemberRequest<TService>
{
    ISpeMemberRequest<TService> Parameters(params Type?[] types);
    Task Send(CancellationToken cancellationToken = default);
}

public interface ISpeMemberRequest<TService, TResult>
{
    ISpeMemberRequest<TService, TResult> Parameters(params Type?[] types);
    ISpeMemberRequest<TService, TResult> ReturnType(Type type);
    Task<TResult?> Send(CancellationToken cancellationToken = default);
}
