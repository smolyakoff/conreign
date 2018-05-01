using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Editor;
using FluentAssertions;
using Xunit;

namespace Conreign.Core.Tests.Editor
{
    public class PlayerListEditorTest
    {
        [Fact]
        public void AdjustBotCount__Generates_And_Adds_Bots_To_Empty_List()
        {
            var state = new List<PlayerData>();
            var editor = new PlayerListEditor(state);
            const int desiredBotsCount = 3;
            var (botsAdded, botsRemoved) = editor.AdjustBotCount(desiredBotsCount);
            botsRemoved.Should().BeEmpty();
            botsAdded.Should().HaveCount(desiredBotsCount);
            botsAdded.Should().Equal(state);
            AssertStateInvariants(state);
        }

        private static void AssertStateInvariants(IReadOnlyCollection<PlayerData> state)
        {
            state.Should().OnlyHaveUniqueItems(x => x.UserId, "user ids are expected to be unique");
            state.Should().OnlyHaveUniqueItems(x => x.Nickname, "nicknames are expected to be unique");
            state.Should().OnlyHaveUniqueItems(x => x.Color, "colors are expected to be unique");
            foreach (var player in state)
            {
                player.Should().NotBeNull();
                player.Nickname.Should().NotBeEmpty();
                player.Color.Should().MatchRegex("^#(?:[0-9a-fA-F]{3}){1,2}$");
            }
        }
    }
}
