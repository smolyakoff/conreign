using System.Text.RegularExpressions;
using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Validators
{
    public class PlayerOptionsValidator : AbstractValidator<PlayerOptionsData>
    {
        public PlayerOptionsValidator()
        {
            RuleFor(x => x.Nickname).NotEmpty();
            RuleFor(x => x.Color).NotEmpty().Matches(new Regex("^#[0-9a-f]{6}$"));
        }
    }
}
