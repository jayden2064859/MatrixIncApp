using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache(); // Vereist voor session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Sessie timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // We gebruiken voor nu even een SQLite voor de database,
            // omdat deze eenvoudig lokaal te gebruiken is en geen extra configuratie nodig heeft.
            builder.Services.AddDbContext<MatrixIncDbContext>(
                options => options.UseSqlite("Data Source=MatrixInc.db"));

            // We registreren de repositories in de DI container
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

  

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<MatrixIncDbContext>();
                context.Database.EnsureCreated();
                MatrixIncDbInitializer.Initialize(context);
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
