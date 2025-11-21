using Microsoft.EntityFrameworkCore;
using PatientsProject.App.Domain;
using PatientsProject.App.Models;
using PatientsProject.App.Services;
using PatientsProject.Core.App.Services.MVC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbContext, Db>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Db")));
builder.Services.AddScoped<PatientObsoleteService>();

builder.Services.AddScoped<IService<PatientRequest, PatientResponse>, PatientService>();
builder.Services.AddScoped<IService<DoctorRequest, DoctorResponse>, DoctorService>();
builder.Services.AddScoped<IService<BranchRequest, BranchResponse>, BranchService>();
builder.Services.AddScoped<IService<UserRequest, UserResponse>, UserService>();

builder.Services.AddScoped<IService<GroupRequest, GroupResponse>, GroupService>();

builder.Services.AddScoped<IService<RoleRequest, RoleResponse>, RoleService>();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
