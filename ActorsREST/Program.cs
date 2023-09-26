using ActorRepositoryLib;
using ActorsREST;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => {
    options.AddPolicy(name: PolicyNames.AllowAllPolicy, policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
    options.AddPolicy(name: PolicyNames.OnlyZealand, policy =>
    {
        policy.WithOrigins("http://zealand.dk"
            , "https://zealand.dk")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(PolicyNames.OnlyZealand);

app.UseAuthorization();

app.MapControllers();

app.Run();
