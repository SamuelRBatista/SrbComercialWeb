using InfraData.Context;
using Infrastructure.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Application.Services;
using FluentValidation;
using Application.Validations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateValidator).Assembly);

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductUpdateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ClientCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ClientUpdateValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<SupplierCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SupplierUpdateValidator>();


// Configurar a autenticação Bearer com JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "unique-app-id-12345",
            ValidAudience = "MyAppUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:SecretKey"]
                    ?? throw new InvalidOperationException("JWT secret key is not configured.")))
        };
    });

// ==========================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ==========================================

// CORREÇÃO 1: Passar IConfiguration, não string
builder.Services.AddSingleton<DapperContext>(); // DapperContext vai receber IConfiguration no construtor

// Se quiser manter a outra forma, crie um construtor alternativo em DapperContext
// Mas a forma correta é registrar como singleton e deixar ele pegar a IConfiguration sozinho

// Registrar os serviços
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<SupplierService>();

// ==========================================

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EcomApi", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira apenas o token JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Injeta dependências da infraestrutura
builder.Services.AddInfrastructure();

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();