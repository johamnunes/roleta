using Roleta.Application.Show;
using Roleta.Domain.Games;

namespace Roleta.Application.Tests;

public class ShowStateEndingScriptTests
{
    [Fact]
    public void LoadGame_keeps_ending_script_without_clearing_on_end()
    {
        using var show = new ShowState();
        show.LoadGame(new Game
        {
            Name = "Show sexta",
            EndingScript = "Obrigado a todos!\nVencedor: o do meio.",
        });

        Assert.Equal("Show sexta", show.ActiveGameName);
        Assert.Equal("Obrigado a todos!\nVencedor: o do meio.", show.ActiveEndingScript);
        Assert.True(show.HasActiveGame);
    }

    [Fact]
    public void ClearGame_clears_ending_script()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "X", EndingScript = "Tchau" });

        show.ClearGame();

        Assert.Null(show.ActiveEndingScript);
        Assert.False(show.HasActiveGame);
    }

    [Fact]
    public void LoadGame_blank_ending_script_becomes_null()
    {
        using var show = new ShowState();
        show.LoadGame(new Game { Name = "X", EndingScript = "   " });

        Assert.Null(show.ActiveEndingScript);
    }
}
