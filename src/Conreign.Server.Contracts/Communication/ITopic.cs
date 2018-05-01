﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Contracts.Communication
{
    public interface ITopic
    {
        Task NotifyServer(params IServerEvent[] events);
        Task NotifyServerAndClients(ISet<Guid> userIds, ISet<Guid> connectionIds, params IEvent[] events);
    }
}