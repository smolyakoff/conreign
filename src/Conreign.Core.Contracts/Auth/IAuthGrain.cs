﻿using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth.Data;
using Orleans;

namespace Conreign.Core.Contracts.Auth
{
    public interface IAuthGrain : IGrain
    {
        Task<AuthenticationResultPayload> AuthenticateAnonymousUser();
    }
}
