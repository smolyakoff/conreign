using System;
using Conreign.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Server.Gameplay.Validators
{
    internal class GameOptionsValidator : AbstractValidator<GameOptionsData>
    {
        private readonly int _humansCount;

        public GameOptionsValidator(int humansCount)
        {
            if (humansCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(humansCount));
            }
            _humansCount = humansCount;
            RuleFor(x => x.MapHeight)
                .GreaterThanOrEqualTo(GameOptionsData.MinumumMapSize)
                .LessThanOrEqualTo(GameOptionsData.MaximumMapSize);
            RuleFor(x => x.MapWidth)
                .GreaterThanOrEqualTo(GameOptionsData.MinumumMapSize)
                .LessThanOrEqualTo(GameOptionsData.MaximumMapSize);
            RuleFor(x => x.NeutralPlanetsCount)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.BotsCount)
                .LessThanOrEqualTo(GameOptionsData.MaximumBotsCount)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x)
                .Must(FitTheMapSize)
                .WithMessage("Too many bots or neutral planets.");
        }

        private bool FitTheMapSize(GameOptionsData data)
        {
            var totalPlanets = data.NeutralPlanetsCount + data.BotsCount + _humansCount;
            return totalPlanets < data.MapWidth * data.MapHeight;
        }
    }
}