using AutoMapper;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Client.Handlers
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdatePlayerOptionsCommand, PlayerOptionsData>();
        }
    }
}
