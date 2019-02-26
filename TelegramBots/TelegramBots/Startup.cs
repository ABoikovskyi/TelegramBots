using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBots.Context;
using TelegramBots.Helpers;
using TelegramBots.Services;

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
			services.AddScoped<ExportService, ExportService>();
			services.AddScoped<PlayZoneBotServiceBase, PlayZoneBotServiceBase>();
			services.AddScoped<PlayZoneBotServiceTelegram, PlayZoneBotServiceTelegram>();
			services.AddScoped<PlayZoneBotServiceViber, PlayZoneBotServiceViber>();
			services.AddScoped<PopCornBotServiceBase, PopCornBotServiceBase>();
			services.AddScoped<PopCornBotServiceTelegram, PopCornBotServiceTelegram>();
			services.AddScoped<PopCornBotServiceViber, PopCornBotServiceViber>();

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
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			MemoryCacheHelper.ServiceProvider = app.ApplicationServices;
			QuartzService.StartSiteWorkJob().Wait();
			PopCornBotServiceTelegram.Init(app.ApplicationServices);
			PopCornBotServiceViber.Init(app.ApplicationServices);
			PlayZoneBotServiceTelegram.Init(app.ApplicationServices);
			PlayZoneBotServiceViber.Init(app.ApplicationServices);
		}
	}
}