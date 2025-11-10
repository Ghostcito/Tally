using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.Service.Implementation;
using SoftWC.Service.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;


var builder = WebApplication.CreateBuilder(args);

//Conexion SQLite
// var connectionString = builder.Configuration.GetConnectionString("SqlLiteConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found."); ;

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlite("Data Source=appdata.db"));

//Conexion PostgreSQL

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// Agregar DbContext con soporte para PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+";
    // También puedes permitir espacios agregando el carácter espacio " "
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

// Registrar servicios
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AsistenciaService>();
builder.Services.AddScoped<EmpleadoService>();

// Servicios de exportación (pueden ser Transient)
builder.Services.AddTransient<IExcelExportService, ExcelExportService>();

builder.Services.AddTransient<IPdfExportService, PdfExportService>();

// Configuración de DinkToPdf (si lo usas)
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Servicios de Apis
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//configura los roles y el usuario admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //await IdentityDataInitializer.SeedData(services); //Método para iniciar usuarios admin y clientes iniciales
    //await UserDataInitializer.SeedData2(services); //Método para iniciar la base de datos y crear tablas
    //await newRolesDataInitializer.SeedData4(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//evitan que el navegador sirva la vista desde su caché.
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

