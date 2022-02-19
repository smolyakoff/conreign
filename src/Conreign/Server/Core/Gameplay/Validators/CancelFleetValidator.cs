using Conreign.Server.Contracts.Shared.Gameplay.Data;
using FluentValidation;

namespace Conreign.Server.Core.Gameplay.Validators;

internal class CancelFleetValidator : AbstractValidator<FleetCancelationData>
{
    public CancelFleetValidator(int totalWaitingFleets)
    {
        RuleFor(x => x.Index).GreaterThanOrEqualTo(0).LessThan(totalWaitingFleets);
    }
}