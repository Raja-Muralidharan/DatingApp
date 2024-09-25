using API.Extenstions;
using API.Middelware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServics(builder.Configuration);
builder.Services.AddIdendtityServiceExtention(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors( x => x.AllowAnyHeader().AllowAnyMethod()
              .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
