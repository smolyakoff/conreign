using AutoMapper;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Client.CommandHandler.Handlers
{
    internal class HandlersMappingProfile : Profile
    {
        public HandlersMappingProfile()
        {
            CreateMap<CancelFleetCommand, FleetCancelationData>();
            CreateMap<WriteCommand, TextMessageData>();
        }
    }
}
