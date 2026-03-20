using Dimensions.Admin.Api.Extensions;
using Dimensions.Admin.Api.Middleware;
using Dimensions.Admin.Api.Validation;
using Dimensions.Infrastructure.Extensions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "Logs"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDimensionsSwagger();
builder.Services.AddDimensionsServices(builder.Configuration);
builder.Services.AddDimensionsAuthentication(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<ValidationActionFilter>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

await app.Services.InitializeDimensionsDatabaseAsync();

app.UseDimensionsSwagger();
app.UseCors("DefaultCors");
app.UseMiddleware<CaseIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiRequestLogMiddleware>();
app.MapControllers();

app.Run();

