using System;
using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Validators
{
    internal class LaunchFleetValidator : AbstractValidator<FleetData>
    {
        private readonly Guid _senderId;
        private readonly Map _map;

        public LaunchFleetValidator(Guid senderId, Map map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            _senderId = senderId;
            _map = map;

            RuleFor(x => x.From)
                .NotEmpty()
                .Must(Exist)
                .Must(BelongToSender);
            RuleFor(x => x.To)
                .NotEmpty()
                .Must(Exist);
            RuleFor(x => x.Ships)
                .GreaterThan(0)
                .Must(BeEnoughShips);
        }

        private bool BeEnoughShips(FleetData fleet, int ships)
        {
            var availableShips = _map.GetPlanetByNameOrNull(fleet.From).Ships;
            return availableShips >= ships;
        }

        private bool Exist(string planet)
        {
            return _map.GetPlanetByNameOrNull(planet) != null;
        }

        private bool BelongToSender(string planet)
        {
            return _map.GetPlanetByNameOrNull(planet).OwnerId == _senderId;
        }
    }
}
