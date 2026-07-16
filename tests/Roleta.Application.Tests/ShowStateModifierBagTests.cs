using Roleta.Application.Show;
using Roleta.Domain.Catalog;
using Roleta.Domain.Enums;
using Roleta.Domain.Games;

namespace Roleta.Application.Tests;

public class ShowStateModifierBagTests
{
    [Fact]
    public void SetModifierPool_fills_bag_with_two_copies_each()
    {
        using var show = new ShowState(new Random(1));
        var mods = CanonicalList();

        show.SetModifierPool(mods);

        Assert.Equal(mods.Count * CanonicalModifiers.CopiesPerKind, show.ModifierBagRemaining);
        Assert.Equal(CanonicalModifiers.CopiesPerKind, show.ModifierBagCount(ModifierKind.Left));
    }

    [Fact]
    public void DrawModifier_sets_kind_and_consumes_copy()
    {
        using var show = new ShowState(new Random(42));
        show.SetModifierPool(CanonicalList());

        Assert.True(show.DrawModifier());
        Assert.NotNull(show.Current);
        Assert.Equal(RevealKind.Modifier, show.Current.Kind);
        Assert.NotNull(show.Current.ModifierKind);
        Assert.Equal(CanonicalModifiers.NameOf(show.Current.ModifierKind.Value), show.Current.Text);
        Assert.Equal(CanonicalList().Count * CanonicalModifiers.CopiesPerKind - 1, show.ModifierBagRemaining);
    }

    [Fact]
    public void DrawModifier_after_exhausting_kind_removes_it_until_refill()
    {
        // Random que sempre pega o índice 0 → depois de shuffle controlado via seed,
        // consumimos até zerar um kind específico forçando o pool.
        using var show = new ShowState(new Random(0));
        var onlyLeft = new List<ModifierDefinition>
        {
            new() { Kind = ModifierKind.Left, Name = "ESQUERDA", IsActive = true },
        };
        show.SetModifierPool(onlyLeft);

        Assert.True(show.DrawModifier());
        Assert.Equal(ModifierKind.Left, show.Current!.ModifierKind);
        Assert.Equal(1, show.ModifierBagRemaining);

        Assert.True(show.DrawModifier());
        Assert.Equal(0, show.ModifierBagRemaining);

        // Sacola vazia → refill automático no próximo draw
        Assert.True(show.DrawModifier());
        Assert.Equal(ModifierKind.Left, show.Current!.ModifierKind);
        Assert.Equal(CanonicalModifiers.CopiesPerKind - 1, show.ModifierBagRemaining);
    }

    [Fact]
    public void LoadGame_rebuilds_modifier_bag()
    {
        using var show = new ShowState(new Random(7));
        var mods = CanonicalList();
        show.LoadGame(new Game { Name = "Show", Modifiers = mods });

        Assert.Equal(mods.Count * CanonicalModifiers.CopiesPerKind, show.ModifierBagRemaining);
        Assert.True(show.DrawModifier());
    }

    [Fact]
    public void SetModifierPool_same_ids_does_not_reset_bag()
    {
        using var show = new ShowState(new Random(3));
        var mods = CanonicalList();
        show.SetModifierPool(mods);
        show.DrawModifier();
        var remaining = show.ModifierBagRemaining;

        show.SetModifierPool(mods); // mesmos IDs

        Assert.Equal(remaining, show.ModifierBagRemaining);
    }

    private static List<ModifierDefinition> CanonicalList() =>
        CanonicalModifiers.All
            .Select(x => new ModifierDefinition { Kind = x.Kind, Name = x.Name, IsActive = true })
            .ToList();
}
