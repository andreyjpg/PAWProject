using Microsoft.EntityFrameworkCore;
using PAWProject.Core.Business;
using PAWProject.Data.MSSQL;
using PAWProject.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NewsHubContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NewsHubConnection"));
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ISourceBusiness, SourceBusiness>();
builder.Services.AddScoped<ISourceItemBusiness, SourceItemBusiness>();
builder.Services.AddScoped<IRepositorySource, RepositorySource>();
builder.Services.AddScoped<IRepositorySourceItem, RepositorySourceItem>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
