using System;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay.Editor
{
    public interface IPlanetGenerator
    {
        PlanetData Generate(string name, Guid? ownerId);
    }
}