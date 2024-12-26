using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using TestGeneratorAPI.Application.Services;
using TestGeneratorAPI.Core.Abstractions;
using TestGeneratorAPI.DataAccess.Context;
using TestGeneratorAPI.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("JWTToken", new OpenApiSecurityScheme
    {
        Description = "Authorization with access token",
        Name = "Access Token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityDefinition("Basic Auth", new OpenApiSecurityScheme
    {
        Description = "Authorization with login and password",
        Name = "Basic Auth",
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<TestGeneratorDbContext>(
    options => options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONTEXT")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokensService.Issuer,
            ValidateAudience = true,
            ValidAudience = TokensService.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = TokensService.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddAuthentication(BasicDefaults.AuthenticationScheme)
    .AddBasic<UsersService>(options => { options.Realm = "TestGeneratorAPI"; });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ITokensRepository, TokensRepository>();
builder.Services.AddScoped<ITokensService, TokensService>();
builder.Services.AddScoped<IPluginsRepository, PluginsRepository>();
builder.Services.AddScoped<IPluginsService, PluginsService>();
builder.Services.AddScoped<IPluginReleasesRepository, PluginReleasesRepository>();
builder.Services.AddScoped<IPluginReleasesService, PluginReleasesService>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IAppFilesRepository, AppFilesRepository>();
builder.Services.AddScoped<IAppFileService, AppFileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.MapMetrics(registry: new CollectorRegistry());

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<TestGeneratorDbContext>();
await dbContext.Database.MigrateAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();