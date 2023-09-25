using ActorRepositoryLib;
using Microsoft.EntityFrameworkCore;

var AllowAllPolicy = "AllowAll";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => {
    options.AddPolicy(name: AllowAllPolicy, policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
    options.AddPolicy(name: "OnlyZealand", policy =>
    {
        policy.WithOrigins("http://zealand.dk"
            , "https://zealand.dk")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

bool useSql = false;
if (useSql)
{
    var optionsBuilder = new DbContextOptionsBuilder<ActorsDbContext>();
    optionsBuilder.UseSqlServer(Secrets.ConnectionString);
    ActorsDbContext context = new ActorsDbContext(optionsBuilder.Options);
    builder.Services.AddSingleton<IActorsRepository>
        (new ActorsRepositoryDB(context));
}
else
{
    builder.Services.AddSingleton<IActorsRepository>
        (new ActorsRepositoryList());
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(AllowAllPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
