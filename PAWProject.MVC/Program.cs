var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cliente HTTP genérico para consumir URLs de las fuentes.
builder.Services.AddHttpClient();

// Registro de servicios de dominio de la aplicación.
builder.Services.AddSingleton<PAWProject.MVC.Services.ISourceStore, PAWProject.MVC.Services.InMemorySourceStore>();
builder.Services.AddSingleton<PAWProject.MVC.Services.ISourceItemStore, PAWProject.MVC.Services.InMemorySourceItemStore>();
builder.Services.AddScoped<PAWProject.MVC.Services.INewsIngestionService, PAWProject.MVC.Services.NewsIngestionService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
 
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();