using WebAppPractica.Models;
using WebAppPractica.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

var pathsConfigPath = Path.Combine(builder.Environment.ContentRootPath, "Paths", "path.json");
var pathsConfig = new ConfigurationBuilder().AddJsonFile(pathsConfigPath, optional: false, reloadOnChange: true).Build();
builder.Services.Configure<PathsConfig>(pathsConfig);

builder.Services.AddHttpClient<IParticipanteService, ParticipanteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
