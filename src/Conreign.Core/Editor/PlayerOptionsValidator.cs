using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Conreign.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Editor
{
    public class PlayerOptionsValidator : AbstractValidator<PlayerOptionsData>
    {
        private readonly HashSet<string> _usedNicknames;

        public PlayerOptionsValidator(HashSet<string> usedNicknames)
        {
            _usedNicknames = usedNicknames ?? throw new ArgumentNullException(nameof(usedNicknames));
            RuleFor(x => x.Nickname).Must(BeUnique).NotEmpty();
            RuleFor(x => x.Color).NotEmpty().Matches(new Regex("^#[0-9a-f]{6}$"));
        }

        private bool BeUnique(string nickname)
        {
            return !_usedNicknames.Contains(nickname);
        }
    }
}