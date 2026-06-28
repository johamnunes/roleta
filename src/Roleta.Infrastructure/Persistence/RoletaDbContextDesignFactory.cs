using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Roleta.Infrastructure.Persistence;

/// <summary>
/// Permite ao "dotnet ef" criar o contexto em design-time (para gerar migrations)
/// sem precisar rodar o app Web. A connection string aqui é só de scaffolding.
/// </summary>
public class RoletaDbContextDesignFactory : IDesignTimeDbContextFactory<RoletaDbContext>
{
    public RoletaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RoletaDbContext>()
            .UseSqlite("Data Source=roleta-design.db")
            .Options;
        return new RoletaDbContext(options);
    }
}
