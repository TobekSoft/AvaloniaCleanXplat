var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// TODO: Register MyApp.Infrastructure services
// builder.Services.AddDbContext<AppDbContext>(options => ...);
// builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
