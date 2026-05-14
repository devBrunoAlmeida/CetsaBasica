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
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
#endregion

#region Repositories
builder.Services.AddScoped<FuncionarioRepository>();
builder.Services.AddScoped<RetiradaRepository>();
builder.Services.AddScoped<CestaRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<DashboardRepository>();
#endregion

#region Applications
builder.Services.AddScoped<FuncionarioApplication>();
builder.Services.AddScoped<RetiradaApplication>();
builder.Services.AddScoped<CestaApplication>();
builder.Services.AddScoped<NotificacaoApplication>();
builder.Services.AddScoped<DashboardApplication>();
builder.Services.AddScoped<ImportacaoApplication>();
#endregion

#region Services
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<RetiradaService>();
builder.Services.AddScoped<CestaService>();
builder.Services.AddScoped<NotificacaoService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ImportacaoService>();
#endregion

var app = builder.Build();

// Swagger só em dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();

app.Run();
