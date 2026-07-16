using Roleta.Application.Show;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;

namespace Roleta.Application.Tests;

public class ShowStateFlipTests
{
    [Fact]
    public void ToggleRule_marks_rule_as_flipped_and_shows_opposite()
    {
        using var show = new ShowState();
        var rule = RuleWithOpposite();

        show.ShowRule(rule);
        show.ToggleRule(rule);

        Assert.True(show.IsRuleFlipped(rule.Id));
        Assert.True(show.IsShowingOpposite);
        Assert.Equal(rule.OppositeText, show.Current!.Text);
    }

    [Fact]
    public void ToggleRule_again_returns_to_base()
    {
        using var show = new ShowState();
        var rule = RuleWithOpposite();
        show.ShowRule(rule);
        show.ToggleRule(rule);

        show.ToggleRule(rule);

        Assert.False(show.IsRuleFlipped(rule.Id));
        Assert.False(show.IsShowingOpposite);
        Assert.Equal(rule.Text, show.Current!.Text);
    }

    [Fact]
    public void ShowRule_respects_persisted_flipped_state()
    {
        using var show = new ShowState();
        var rule = RuleWithOpposite();
        show.ShowRule(rule);
        show.ToggleRule(rule);
        show.ClearReveal();

        show.ShowRule(rule);

        Assert.True(show.IsRuleFlipped(rule.Id));
        Assert.Equal(rule.OppositeText, show.Current!.Text);
        Assert.False(show.IsFlipReveal); // Mostrar abre, não vira
    }

    [Fact]
    public void LoadGame_clears_flipped_state()
    {
        using var show = new ShowState();
        var rule = RuleWithOpposite();
        show.ShowRule(rule);
        show.ToggleRule(rule);

        show.LoadGame(new Domain.Games.Game { Name = "Novo", Rules = [rule] });

        Assert.False(show.IsRuleFlipped(rule.Id));
    }

    private static RuleDefinition RuleWithOpposite() => new()
    {
        Color = Color.Blue,
        Text = "Só fale no diminutivo.",
        OppositeText = "Só fale no aumentativo.",
    };
}
