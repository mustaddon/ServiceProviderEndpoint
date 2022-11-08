using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

internal class SpeServiceRequest<TService> : ISpeServiceRequest<TService>
{
    public SpeServiceRequest(SpeClient client)
    {
        _client = client;
    }

    readonly SpeClient _client;


    public ISpeMemberRequest<TService> Member(Expression<Action<TService>> expression)
    {
        return new SpeMemberRequest<TService, object>(_client, expression);
    }

    public ISpeMemberRequest<TService> Member(Expression<Func<TService, Task>> expression)
    {
        return new SpeMemberRequest<TService, object>(_client, expression);
    }

    public ISpeMemberRequest<TService, TResult> Member<TResult>(Expression<Func<TService, Task<TResult>>> expression)
    {
        return new SpeMemberRequest<TService, TResult>(_client, expression);
    }
    public ISpeMemberRequest<TService, TResult> Member<TResult>(Expression<Func<TService, TResult>> expression)
    {
        return new SpeMemberRequest<TService, TResult>(_client, expression);
    }

    public ISpeMemberRequest<TService> Member<TValue>(Expression<Func<TService, TValue>> expression, TValue newValue)
    {
        return new SpeMemberRequest<TService, object>(_client, expression, () => newValue);
    }

    public ISpeMemberRequest<TService> Member<TValue>(Expression<Func<TService, Task<TValue>>> expression, TValue newValue)
    {
        return new SpeMemberRequest<TService, object>(_client, expression, () => newValue);
    }
}