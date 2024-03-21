using AfghanWheelzz.Models;
using AfghanWheelzz.Models.UserModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using AfghanWheelzz.Data;
using AfghanWheelzz.Repository;
using AfghanWheelzz.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AfghanWheelzDbContext")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure password requirements, lockout, etc. if needed
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/Account/Login";
    option.LogoutPath = "/Account/Logout";
    option.Cookie.Name = "AfghanWheelzz";
    option.ExpireTimeSpan = TimeSpan.FromDays(30);
    option.SlidingExpiration = true;

});

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

app.UseAuthentication(); // Add this line for Identity authentication
app.UseAuthorization();

app.MapGet("/", async context =>
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var user = await userManager.FindByEmailAsync("nawab.safi61@gmail.com");
        if (user != null)
        {
            //Check if Admin Role exist 
            var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                //if not exist create role Admin
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //Add Selected User to Admin
            await userManager.AddToRoleAsync(user, "Admin");


            // Redirect the response before any content is written
            context.Response.Redirect("/Cars/Index");
            return;
        }
      
    }

/*    if (context.User.Identity.IsAuthenticated)
    {
        // If the user is authenticated, redirect to the Cars/Index page
        context.Response.Redirect("/Cars/Index");
    }
    else
    {
        // If not authenticated, redirect to the Home/Index page
        context.Response.Redirect("/Home/Index");
    }*/
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();