using Conreign.Core.Contracts.Gameplay.Data;
using FluentValidation;

namespace Conreign.Core.Gameplay.Editor
{
    internal class GameOptionsValidator : AbstractValidator<GameOptionsData>
    {
        private readonly int _playersCount;

        public GameOptionsValidator(int playersCount)
        {
            _playersCount = playersCount;
            RuleFor(x => x.MapHeight)
                .GreaterThanOrEqualTo(Defaults.MinMapSize)
                .LessThanOrEqualTo(Defaults.MaxMapSize);
            RuleFor(x => x.MapWidth)
                .GreaterThanOrEqualTo(Defaults.MinMapSize)
                .LessThanOrEqualTo(Defaults.MaxMapSize);
            RuleFor(x => x.NeutralPlanetsCount)
                .GreaterThanOrEqualTo(0)
                .Must(FitTheMapSize).WithMessage("Neutral planets count should fit the map size.");
        }

        private bool FitTheMapSize(GameOptionsData data, int neutralPlanetsCount)
        {
            var totalPlanets = neutralPlanetsCount + _playersCount;
            return totalPlanets < data.MapWidth*data.MapHeight;
        }
    }
}