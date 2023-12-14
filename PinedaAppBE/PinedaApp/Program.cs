using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using PinedaApp.Configurations;
using PinedaApp.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Get Setting Configuration
string allowedOrigin = builder.Configuration.GetSection("AppSettings:AllowedOrigin").Value;

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//Api Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

//Setting Database
builder.Services.AddDbContext<PinedaAppContext>
(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("PinedaAppContext"))
);

//Setting AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Setting CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins(allowedOrigin)
        .AllowAnyHeader()
        .WithMethods("GET", "POST", "PUT", "DELETE");
    });
});

//Inject Services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAcademicServices, AcademicService>();
builder.Services.AddTransient<IExperienceServices, ExperienceService>();
builder.Services.AddTransient<IPortfolioService, PortfolioService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    PinedaAppContext context = services.GetRequiredService<PinedaAppContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
