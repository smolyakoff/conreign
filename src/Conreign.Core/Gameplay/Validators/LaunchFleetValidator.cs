using System;
using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Validators
{
    internal class LaunchFleetValidator : AbstractValidator<FleetData>
    {
        private readonly Map _map;
        private readonly Guid _senderId;

        public LaunchFleetValidator(Guid senderId, Map map)
        {
            _senderId = senderId;
            _map = map ?? throw new ArgumentNullException(nameof(map));

            RuleFor(x => x.From)
                .NotEmpty()
                .Must(Exist)
                .Must(BelongToSender);
            RuleFor(x => x.To)
                .Must(Exist)
                .Must(NotBeTheSameAsFrom);
            RuleFor(x => x.Ships)
                .GreaterThan(0)
                .Must(BeEnoughShips);
        }

        private static bool NotBeTheSameAsFrom(FleetData fleet, int to)
        {
            return to != fleet.From;
        }

        private bool BeEnoughShips(FleetData fleet, int ships)
        {
            var availableShips = _map[fleet.From].Ships;
            return availableShips >= ships;
        }

        private bool Exist(int coordinate)
        {
            return _map.ContainsPlanet(coordinate);
        }

        private bool BelongToSender(int coordinate)
        {
            return _map[coordinate].OwnerId == _senderId;
        }
    }
}