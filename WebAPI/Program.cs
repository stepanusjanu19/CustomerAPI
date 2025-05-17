using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Application.Interfaces;
using Application.Mappings;
using Application.Extensions;
using Infrastructure.Database;
using Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.jfeduzpmtljxaxsoyfij;Password=TsywqpHyd2NAmNzLB";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
    .ConfigureWarnings(warnings => 
               warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<CustomExtensions>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
public partial class Program { }
