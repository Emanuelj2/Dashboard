using ManagementDashboard.Data;
using ManagementDashboard.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSession();
builder.Services.AddControllersWithViews();

//add the questPDF community license
QuestPDF.Settings.License = LicenseType.Community;

//add the containers
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPdfService, PdfService>();

//add auth service
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ManagementDashboardContext") ?? throw new InvalidOperationException("Connection string 'ManagementDashboardContext' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
