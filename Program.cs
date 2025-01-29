using FillableFormWebApp;
using FillableFormWebApp.Data;
using FillableFormWebApp.Interfaces;
using FillableFormWebApp.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static FillableFormWebApp.Data.InitDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDbContextFactory<FillableFormWebAppContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

IConfigurationRoot formConfing = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("FormFields.json")
.Build();

builder.Services.Configure<PdfForm>(p => formConfing.GetSection("PdfForm").Bind(p));
builder.Services.AddSingleton<IPdfUtil, PdfUtil>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    InitDb initDb = new(scope);
    initDb.ApplyMigrations();

    string[] roles = { "Admin", "Supervisor" };
    await initDb.createRoles(roles);

    List<createUserModel> createUserModel = new()
    { 
        new createUserModel { Email = "admin@example.com", Password = "Admin@123", Role = "Admin" },
        new createUserModel { Email = "supervisor@example.com", Password = "Supervisor@123", Role = "Supervisor" },
        new createUserModel { Email = "johndoe@example.com", Password = "Johndoe@123"}
    };

    await initDb.CreateTestUsers(createUserModel);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
