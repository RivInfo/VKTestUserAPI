using UserWebApi.Contexts;
using UserWebApi.Options;
using UserWebApi.SingltonServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>
    (builder.Configuration.GetSection(nameof(DatabaseSettings)));

builder.Services.AddDbContext<UserContext>();

builder.Services.AddSingleton<ILoginBlock, LoginBlock>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();