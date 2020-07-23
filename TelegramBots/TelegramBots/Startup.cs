using System;
using BusinessLayer.Helpers;
using BusinessLayer.Services;
using BusinessLayer.Services.Idrink;
using BusinessLayer.Services.Insurance;
using DataLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
			services.AddControllersWithViews()
				.AddNewtonsoftJson(options =>
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
				);
			services.AddRazorPages();

			services.AddDbContext<IdrinkDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("IdrinkConnection")));
			services.AddScoped<IdrinkBotService>();
			services.AddScoped<InsuranceBotService>();

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

			ConfigData.AppLink = Configuration.GetSection("ConfigData")["AppLink"];
			ConfigData.TelegramIdrinkKey = Configuration.GetSection("ConfigData")["TelegramIdrinkKey"];
			ConfigData.TelegramProzorroKey = Configuration.GetSection("ConfigData")["TelegramProzorroKey"];
			ConfigData.TelegramInsuranceKey = Configuration.GetSection("ConfigData")["TelegramInsuranceKey"];
			ConfigData.AdminLogin = Configuration.GetSection("ConfigData")["AdminLogin"];
			ConfigData.AdminPass = Configuration.GetSection("ConfigData")["AdminPass"];

			ConfigData.EmailSmtp = Configuration.GetSection("ConfigData")["EmailSmtp"];
			ConfigData.EmailSmtpPort = Convert.ToInt32(Configuration.GetSection("ConfigData")["EmailSmtpPort"]);
			ConfigData.EmailLogin = Configuration.GetSection("ConfigData")["EmailLogin"];
			ConfigData.EmailPassword = Configuration.GetSection("ConfigData")["EmailPassword"];
			ConfigData.EmailSendFrom = Configuration.GetSection("ConfigData")["EmailSendFrom"];
			ConfigData.EmailSenderName = Configuration.GetSection("ConfigData")["EmailSenderName"];
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseRouting();
			app.UseEndpoints(e =>
			{
				e.MapRazorPages();
				e.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action}/{id?}",
					defaults: new { controller = "Home", action = "Index" });
			});

			QuartzService.StartSiteWorkJob().Wait();
			IdrinkBotService.Init();
			InsuranceBotService.WebRootPath = env.WebRootPath;
			InsurancePhraseHelper.WebRootPath = env.WebRootPath;
			InsuranceBotService.Init();
		}
	}
}