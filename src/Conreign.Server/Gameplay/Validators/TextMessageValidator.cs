using Conreign.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Server.Gameplay.Validators
{
    internal class TextMessageValidator : AbstractValidator<TextMessageData>
    {
        public TextMessageValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}