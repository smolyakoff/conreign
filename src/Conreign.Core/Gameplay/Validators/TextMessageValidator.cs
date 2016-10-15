using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Validators
{
    internal class TextMessageValidator : AbstractValidator<TextMessageData>
    {
        public TextMessageValidator()
        {
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
