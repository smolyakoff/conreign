using AutoMapper;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Client.Handlers
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CancelFleetCommand, FleetCancelationData>();
            CreateMap<WriteCommand, TextMessageData>();
        }
    }
}
