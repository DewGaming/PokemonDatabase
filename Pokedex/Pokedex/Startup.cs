using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pokedex.DataAccess.Models;
using RobotsTxt;

namespace Pokedex
{
    /// <summary>
    /// The class that is used to represent the startup of the program.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The program's configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the program's configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">A collection of services for the application.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<AppConfig>(this.Configuration.GetSection("AppConfig"));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: System.TimeSpan.FromSeconds(30), errorNumbersToAdd: null));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDbContext<MyKeysContext>(options =>
            {
                options.UseSqlServer(this.Configuration.GetConnectionString("MyKeysConnection"), options => options.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: System.TimeSpan.FromSeconds(30), errorNumbersToAdd: null));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            })
            .AddDataProtection()
            .PersistKeysToDbContext<MyKeysContext>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access_denied";
                });

            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddStaticRobotsTxt(robots =>
            {
                robots.AddSection(section => section.AddUserAgent("*").Disallow("/admin/"));

                return robots;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The builder for the program.</param>
        /// <param name="env">The web host's environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            CookiePolicyOptions cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx => ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "public,max-age=" + (60 * 60 * 24 * 7),
            });

            app.UseHttpsRedirection();
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRobotsTxt();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// The class that is used to represent the data protection key database context.
    /// </summary>
    internal class MyKeysContext : DbContext, IDataProtectionKeyContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyKeysContext"/> class.
        /// </summary>
        /// <param name="options">The data protection keys database context.</param>
        public MyKeysContext(DbContextOptions<MyKeysContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the database table DataProtectionKeys.
        /// </summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
