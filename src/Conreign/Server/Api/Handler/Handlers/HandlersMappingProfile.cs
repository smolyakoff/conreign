using AutoMapper;
using Conreign.Server.Contracts.Client.Messages;
using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Api.Handler.Handlers;

internal class HandlersMappingProfile : Profile
{
    public HandlersMappingProfile()
    {
        CreateMap<CancelFleetCommand, FleetCancelationData>();
        CreateMap<SendMessageCommand, TextMessageData>();
    }
}