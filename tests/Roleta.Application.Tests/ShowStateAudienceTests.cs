using Roleta.Application.Show;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;

namespace Roleta.Application.Tests;

public class ShowStateAudienceTests
{
    [Fact]
    public void ArmAudienceRule_sets_pending_without_revealing_text()
    {
        using var show = new ShowState();
        var rule = WhiteRule("A plateia escolhe: ______");

        show.ArmAudienceRule(rule);

        Assert.True(show.IsAudiencePending);
        Assert.Null(show.Current);
        Assert.Same(rule, show.CurrentRule);
        Assert.Equal(rule.Id, Assert.Single(show.RuleHistory).Id);
    }

    [Fact]
    public void RevealAudienceRule_shows_rule_and_clears_pending()
    {
        using var show = new ShowState();
        var rule = WhiteRule("Tema livre: ______");
        show.ArmAudienceRule(rule);

        show.RevealAudienceRule();

        Assert.False(show.IsAudiencePending);
        Assert.NotNull(show.Current);
        Assert.Equal(RevealKind.Rule, show.Current.Kind);
        Assert.Equal(Color.White, show.Current.Color);
        Assert.Equal(rule.Text, show.Current.Text);
    }

    [Fact]
    public void CancelAudiencePending_clears_teaser_without_text_on_board()
    {
        using var show = new ShowState();
        show.ArmAudienceRule(WhiteRule("Obsessão: ______"));

        show.CancelAudiencePending();

        Assert.False(show.IsAudiencePending);
        Assert.Null(show.Current);
        Assert.Null(show.CurrentRule);
        Assert.Single(show.RuleHistory);
    }

    [Fact]
    public void DrawRule_white_arms_audience_instead_of_showing()
    {
        using var show = new ShowState();
        var game = new Domain.Games.Game
        {
            Name = "Teste",
            Rules =
            [
                WhiteRule("Personagem: ______"),
                new RuleDefinition { Color = Color.Blue, Text = "Só fale em perguntas." },
            ],
        };
        show.LoadGame(game);

        var ok = show.DrawRule(Color.White);

        Assert.True(ok);
        Assert.True(show.IsAudiencePending);
        Assert.Null(show.Current);
        Assert.Equal(0, show.RemainingRules(Color.White));
    }

    [Fact]
    public void DrawRule_blue_shows_immediately()
    {
        using var show = new ShowState();
        show.LoadGame(new Domain.Games.Game
        {
            Name = "Teste",
            Rules = [new RuleDefinition { Color = Color.Blue, Text = "Eco." }],
        });

        var ok = show.DrawRule(Color.Blue);

        Assert.True(ok);
        Assert.False(show.IsAudiencePending);
        Assert.Equal("Eco.", show.Current?.Text);
    }

    private static RuleDefinition WhiteRule(string text) =>
        new() { Color = Color.White, Text = text };
}
