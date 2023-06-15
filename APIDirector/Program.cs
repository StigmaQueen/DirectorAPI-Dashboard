using APIDirector.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

string? cadena = builder.Configuration.GetConnectionString("PrimariaConnectionStrings");
builder.Services.AddDbContext<Sistem21PrimariaContext>(ob => ob.UseMySql(cadena, ServerVersion.AutoDetect(cadena)));
var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run();
