using System;

namespace Conreign.Contracts.Gameplay.Data
{
    public interface IPlanetData
    {
        string Name { get; }
        Guid? OwnerId { get; }
        double Power { get; }
        int ProductionRate { get; }
        int Ships { get; }
    }
}