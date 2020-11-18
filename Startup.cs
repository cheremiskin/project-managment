using System;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using project_managment.Authentication;
using project_managment.Data.Repositories;
using project_managment.Data.Repositories.RepositoryImpl;
using project_managment.Data.Services;
using project_managment.Data.Services.ServiceImpl;
using project_managment.Services;
using project_managment.Services.ServiceImpl;
using Synercoding.FormsAuthentication;
using AuthenticationOptions = project_managment.Authentication.AuthenticationOptions;
using EncryptionMethod = Synercoding.FormsAuthentication.EncryptionMethod;

namespace project_managment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthenticationOptions.Issuer,
 
                        ValidateAudience = true,
                        ValidAudience = AuthenticationOptions.Audience,
                        ValidateLifetime = true,
 
                        IssuerSigningKey = AuthenticationOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,                        
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", b => b.RequireClaim(ClaimTypes.Role, "ROLE_ADMIN"));
                options.AddPolicy("IsUser", b => b.RequireClaim(ClaimTypes.Role, "ROLE_USER"));
                options.AddPolicy("IsUserOrAdmin", b => b.RequireClaim(ClaimTypes.Role, "ROLE_USER", "ROLE_ADMIN"));
            });
            
            services.AddSingleton(Configuration);
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProjectRepository, ProjectRepository>();

            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITaskService, TaskService>();

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
        }
    }
}
