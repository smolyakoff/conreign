﻿using System.Security.Claims;
using Conreign.Server.Contracts.Server.Auth;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Server.Core.Auth;

[StatelessWorker]
public class AuthGrain : Grain, IAuthGrain
{
    private AuthService _authService;

    public Task<ClaimsIdentity> Authenticate(string accessToken)
    {
        return _authService.Authenticate(accessToken);
    }

    public Task<string> Login(string accessToken = null)
    {
        return _authService.Login(accessToken);
    }

    public override Task OnActivateAsync()
    {
        var options = new AuthOptions { JwtSecret = "conreign!" };
        _authService = new AuthService(options);
        return base.OnActivateAsync();
    }
}