using BusinessLayer.Helpers;
using BusinessLayer.Services;
using BusinessLayer.Services.Festival;
using BusinessLayer.Services.NBCocktailsBar;
using BusinessLayer.Services.PlayZone;
using BusinessLayer.Services.PopCorn;
using DataLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
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
			services.AddDbContext<PlayZoneDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("PlayZoneConnection")));
			services.AddDbContext<PopCornDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("PopCornConnection")));
			services.AddDbContext<NBCocktailsBarDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("NBCocktailsBarConnection")));
            services.AddDbContext<FestivalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FestivalConnection")));
            services.AddScoped<MemoryCacheHelper, MemoryCacheHelper>();
			services.AddScoped<ExportService, ExportService>();
			services.AddScoped<PlayZoneBotServiceBase, PlayZoneBotServiceBase>();
			services.AddScoped<PlayZoneBotServiceTelegram, PlayZoneBotServiceTelegram>();
			services.AddScoped<PopCornBotServiceBase, PopCornBotServiceBase>();
			services.AddScoped<PopCornBotServiceTelegram, PopCornBotServiceTelegram>();
            services.AddScoped<NBCocktailsBarBotServiceBase, NBCocktailsBarBotServiceBase>();
            services.AddScoped<NBCocktailsBarBotServiceTelegram, NBCocktailsBarBotServiceTelegram>();
            services.AddScoped<FestivalBotService, FestivalBotService>();
			services.AddScoped<FestivalBotService, FestivalBotService>();

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

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Festival}/{action=Index}/{id?}");
			});

			QuartzService.StartSiteWorkJob().Wait();
            /*PopCornBotServiceTelegram.Init();
			PopCornBotServiceViber.Init();
			PlayZoneBotServiceTelegram.Init();
			PlayZoneBotServiceViber.Init();
			NBCocktailsBarBotServiceTelegram.Init();*/
            FestivalBotService.Init();
		}
	}
}