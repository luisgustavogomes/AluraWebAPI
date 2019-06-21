using Alura.ListaLeitura.HttpClients;
using Alura.ListaLeitura.Seguranca;
using AluraWebAPI.Formatters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AuthDB"));
            });

            //services.AddIdentity<Usuario, IdentityRole>(options =>
            //{
            //    options.Password.RequiredLength = 3;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //}).AddEntityFrameworkStores<AuthDbContext>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Usuario/Login";
                });


            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new LivroCsvFormatter());
                })
                .AddXmlSerializerFormatters();

            services.AddHttpClient<LivroApiClient>(client =>
            {
                client.BaseAddress = new System.Uri("http://localhost:6000/api/");
            });

            services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new System.Uri("http://localhost:5000/api/");
            });

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
