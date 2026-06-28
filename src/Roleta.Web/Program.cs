using Roleta.Application.Show;
using Roleta.Infrastructure;
using Roleta.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Servir em http://localhost:5005 (loopback, sem HTTPS) — show ao vivo, 100% local.
builder.WebHost.UseUrls("http://localhost:5005");

// Banco local em <raiz do repo>/data/roleta.db (ou ./data ao lado do app, como fallback).
var dataDir = Path.Combine(ResolveRepoRoot(builder.Environment.ContentRootPath), "data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "roleta.db");

builder.Services.AddRoletaInfrastructure(dbPath);

// Estado vivo do show (placar + quadro do telão), compartilhado entre Controle e Público.
builder.Services.AddSingleton<ShowState>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Aplica migrations, garante WAL e popula o catálogo na primeira execução.
await app.Services.InitializeRoletaDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Procura a raiz do repositório (Roleta.sln/.slnx ou .git) subindo a partir do content root.
static string ResolveRepoRoot(string start)
{
    var dir = new DirectoryInfo(start);
    while (dir is not null)
    {
        if (dir.GetFiles("Roleta.sln*").Length > 0 || dir.GetDirectories(".git").Length > 0)
            return dir.FullName;
        dir = dir.Parent;
    }
    return start;
}
