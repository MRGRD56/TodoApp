using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TodoApp.DbInterop;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Models.Auth;
using TodoApp.WebApp.Hubs;
using TodoApp.WebApp.Middleware;
using TodoApp.WebApp.Services;

namespace TodoApp.WebApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthOptions>(_configuration.GetSection("Auth"));

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
            services.AddSignalR();

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

            services.AddDbContext<AppDbContext>(options =>
            {
                if (!options.IsConfigured)
                {
                    options.UseSqlServer(_configuration.GetConnectionString("SqlServer"),
                        optionsBuilder => optionsBuilder.MigrationsAssembly("TodoApp.WebApp"));
                }
            });
            services.AddTodoItemsRepository();
            services.AddUsersRepository();
            services.AddRolesRepository();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiddleware<HttpLogger>();
            }

            ConfigureExceptionHandlers(app, env);

            ApplyDatabaseMigrations(app, env);

            if (!env.IsDevelopment())
            {
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCors();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<TodoHub>("/hubs/todo");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.StartupTimeout = TimeSpan.FromMinutes(5);
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer("start");
                }
            });
        }

        private static void ConfigureExceptionHandlers(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Use(async (context, next) =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    switch (exception)
                    {
                        case HttpRequestException httpRequestException:
                            context.Response.StatusCode =
                                (int) (httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError);
                            await context.Response.WriteAsync(httpRequestException.Message);
                            break;
                        case BadHttpRequestException badHttpRequestException:
                            context.Response.StatusCode = badHttpRequestException.StatusCode;
                            await context.Response.WriteAsync(badHttpRequestException.Message);
                            break;
                        default:
                            await next();
                            break;
                    }
                });
            });
        }

        private static void ApplyDatabaseMigrations(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var appDbContext = scope.ServiceProvider.GetService<AppDbContext>();
            appDbContext?.Database.Migrate();
        }
    }
}