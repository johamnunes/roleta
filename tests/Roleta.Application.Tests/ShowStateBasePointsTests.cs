using Roleta.Application.Show;
using Roleta.Domain.Games;

namespace Roleta.Application.Tests;

public class ShowStateBasePointsTests
{
    [Fact]
    public void LoadGame_stores_base_points()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "Show", BasePoints = 5 });

        Assert.Equal(5, show.ActiveBasePoints);
        Assert.All(show.Players, p => Assert.Equal(0, p.Score));
    }

    [Fact]
    public void AssignBaseScores_sets_each_player_to_base_points()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "Show", BasePoints = 3 });
        show.AddScore(0, 9);

        Assert.True(show.AssignBaseScores());
        Assert.All(show.Players, p => Assert.Equal(3, p.Score));
    }

    [Fact]
    public void AssignBaseScores_noop_when_zero_or_no_game()
    {
        using var show = new ShowState();
        Assert.False(show.AssignBaseScores());

        show.LoadGame(new Game { Name = "Show", BasePoints = 0 });
        show.AddScore(1, 2);
        Assert.False(show.AssignBaseScores());
        Assert.Equal(2, show.Players[1].Score);
    }

    [Fact]
    public void ClearGame_clears_base_points()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "X", BasePoints = 4 });
        show.ClearGame();
        Assert.Equal(0, show.ActiveBasePoints);
    }

    [Fact]
    public void LoadGame_clamps_base_points()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "X", BasePoints = 150 });
        Assert.Equal(99, show.ActiveBasePoints);
    }
}
