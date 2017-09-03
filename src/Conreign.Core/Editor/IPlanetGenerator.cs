using System;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core.Editor
{
    public interface IPlanetGenerator
    {
        PlanetData Generate(string name, Guid? ownerId);
    }
}