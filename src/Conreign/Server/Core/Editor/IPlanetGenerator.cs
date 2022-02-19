using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Core.Editor;

public interface IPlanetGenerator
{
    PlanetData Generate(string name, Guid? ownerId);
}