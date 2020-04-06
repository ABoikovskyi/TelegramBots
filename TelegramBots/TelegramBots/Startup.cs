using BusinessLayer.Helpers;
using BusinessLayer.Services;
using BusinessLayer.Services.Idrink;
using BusinessLayer.Services.OrangeClub;
using BusinessLayer.Services.Prozorro;
using DataLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramBots
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			/*services.AddDbContext<PlayZoneDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("PlayZoneConnection")));
			services.AddDbContext<PopCornDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("PopCornConnection")));
			services.AddDbContext<NBCocktailsBarDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("NBCocktailsBarConnection")));
            services.AddDbContext<FestivalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FestivalConnection")));*/
			services.AddDbContext<IdrinkDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("IdrinkConnection")));
			services.AddDbContext<ProzorroDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("ProzorroConnection")));
			/*services.AddScoped<MemoryCacheHelper, MemoryCacheHelper>();
			services.AddScoped<ExportService, ExportService>();*/
			/*services.AddScoped<PlayZoneBotServiceBase, PlayZoneBotServiceBase>();
			services.AddScoped<PlayZoneBotServiceTelegram, PlayZoneBotServiceTelegram>();
			services.AddScoped<PopCornBotServiceBase, PopCornBotServiceBase>();
			services.AddScoped<PopCornBotServiceTelegram, PopCornBotServiceTelegram>();
            services.AddScoped<NBCocktailsBarBotServiceBase, NBCocktailsBarBotServiceBase>();
            services.AddScoped<NBCocktailsBarBotServiceTelegram, NBCocktailsBarBotServiceTelegram>();
            services.AddScoped<FestivalBotService, FestivalBotService>();*/
			services.AddScoped<IdrinkBotService, IdrinkBotService>();
			services.AddScoped<ProzorroBotService, ProzorroBotService>();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/Account/Login");
					options.AccessDeniedPath = new PathString("/Home/Error");
				});

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddCors(options =>
			{
				options.AddPolicy("My",
					builder => builder.WithOrigins("https://orangeclub.ua/")
						.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			ConfigData.AppLink = Configuration.GetSection("ConfigData")["AppLink"];
			ConfigData.TelegramIdrinkKey = Configuration.GetSection("ConfigData")["TelegramIdrinkKey"];
			ConfigData.TelegramProzorroKey = Configuration.GetSection("ConfigData")["TelegramProzorroKey"];
			ConfigData.AdminLogin = Configuration.GetSection("ConfigData")["AdminLogin"];
			ConfigData.AdminPass = Configuration.GetSection("ConfigData")["AdminPass"];

			//QuartzService.ResetFestivalJobs(services.BuildServiceProvider().GetService<FestivalDbContext>());
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			/*if (env.IsDevelopment())
		    {
			    app.UseDeveloperExceptionPage();
		    }
		    else
		    {
			    app.UseExceptionHandler("/Home/Error");
			    app.UseHsts();
		    }*/
			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();
			app.UseCors("My");
			//app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			QuartzService.StartSiteWorkJob().Wait();
			QuartzService.StartTendersMonitoringJob().Wait();
			/*PopCornBotServiceTelegram.Init();
			PopCornBotServiceViber.Init();
			PlayZoneBotServiceTelegram.Init();
			PlayZoneBotServiceViber.Init();
			NBCocktailsBarBotServiceTelegram.Init();
			FestivalBotService.Init();*/
			IdrinkBotService.Init();
			ProzorroBotService.Init();
		}
	}
}