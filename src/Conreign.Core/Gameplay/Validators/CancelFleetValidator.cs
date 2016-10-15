using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Validators
{
    internal class CancelFleetValidator : AbstractValidator<FleetCancelationData>
    {
        public CancelFleetValidator(int totalWaitingFleets)
        {
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0).LessThan(totalWaitingFleets);
        }
    }
}