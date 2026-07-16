using Microsoft.EntityFrameworkCore;
using Roleta.Domain.Catalog;
using Roleta.Infrastructure.Persistence;

namespace Roleta.Infrastructure.Catalog;

/// <summary>Garante os 5 modificadores canônicos no banco (idempotente).</summary>
public static class ModifierCatalogSeeder
{
    public static async Task EnsureCanonicalModifiersAsync(
        RoletaDbContext db, CancellationToken ct = default)
    {
        var existing = await db.Modifiers.ToListAsync(ct);

        foreach (var (kind, name) in CanonicalModifiers.All)
        {
            var matches = existing.Where(m => m.Kind == kind).OrderBy(m => m.CreatedAt).ToList();
            if (matches.Count == 0)
            {
                db.Modifiers.Add(new ModifierDefinition
                {
                    Kind = kind,
                    Name = name,
                    IsActive = true,
                });
                continue;
            }

            var keep = matches[0];
            if (keep.Name != name || !keep.IsActive)
            {
                keep.Name = name;
                keep.IsActive = true;
                keep.UpdatedAt = DateTimeOffset.UtcNow;
            }

            // Duplicatas do mesmo kind: desativa (não apaga — pode estar em jogos salvos).
            foreach (var dup in matches.Skip(1))
            {
                if (dup.IsActive)
                {
                    dup.IsActive = false;
                    dup.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
