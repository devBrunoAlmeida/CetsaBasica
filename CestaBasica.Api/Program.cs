using CestaBasica.Api.Data;
using CestaBasica.Api.Repositories;
using CestaBasica.Api.Services;
using CestaBasica.Api.Applications;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

#region Configurações de CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Configurações de banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("CONNECTION STRING:");
Console.WriteLine(connectionString);
Console.WriteLine("EVOLUTION CONFIG:");
Console.WriteLine(builder.Configuration["EvolutionApi:BaseUrl"]);
Console.WriteLine(builder.Configuration["EvolutionApi:ApiKey"]);
Console.WriteLine(builder.Configuration["EvolutionApi:Instance"]);


if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("ConnectionString DefaultConnection não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
#endregion

#region Repositories
builder.Services.AddScoped<FuncionarioRepository>();
builder.Services.AddScoped<RetiradaRepository>();
builder.Services.AddScoped<CestaRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<HistoricoImportacaoRepository>();
builder.Services.AddScoped<ConfiguracoesRepository>();
#endregion

#region Applications
builder.Services.AddScoped<FuncionarioApplication>();
builder.Services.AddScoped<RetiradaApplication>();
builder.Services.AddScoped<CestaApplication>();
builder.Services.AddScoped<NotificacaoApplication>();
builder.Services.AddScoped<DashboardApplication>();
builder.Services.AddScoped<ImportacaoApplication>();
builder.Services.AddScoped<UsuarioApplication>();
builder.Services.AddScoped<ConfiguracoesApplication>();
#endregion

#region Services
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<RetiradaService>();
builder.Services.AddScoped<CestaService>();
builder.Services.AddScoped<NotificacaoService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ImportacaoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ConfiguracoesService>();
builder.Services.AddHttpClient<EvolutionApiService>();
#endregion

var app = builder.Build();

// Swagger só em dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
//app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();
// Rodar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.Run();
