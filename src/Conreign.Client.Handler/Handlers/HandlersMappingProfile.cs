using AutoMapper;
using Conreign.Client.Contracts.Messages;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Client.Handler.Handlers
{
    internal class HandlersMappingProfile : Profile
    {
        public HandlersMappingProfile()
        {
            CreateMap<CancelFleetCommand, FleetCancelationData>();
            CreateMap<SendMessageCommand, TextMessageData>();
        }
    }
}