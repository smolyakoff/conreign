using Conreign.Server.Contracts.Shared.Gameplay.Data;
using FluentValidation;

namespace Conreign.Server.Core.Gameplay.Validators;

internal class TextMessageValidator : AbstractValidator<TextMessageData>
{
    public TextMessageValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}