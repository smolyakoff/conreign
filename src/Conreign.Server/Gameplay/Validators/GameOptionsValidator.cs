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
                .GreaterThanOrEqualTo(0)
                .Must(FitTheMapSize)
                .WithMessage("Neutral planets count should fit the map size.");
            RuleFor(x => x.BotsCount)
                .GreaterThanOrEqualTo(0);
        }

        private bool FitTheMapSize(GameOptionsData data, int neutralPlanetsCount)
        {
            var totalPlanets = neutralPlanetsCount + data.BotsCount + _humansCount;
            return totalPlanets < data.MapWidth * data.MapHeight;
        }
    }
}