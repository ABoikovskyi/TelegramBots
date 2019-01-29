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

		    services.Configure<CookiePolicyOptions>(options =>
		    {
			    options.CheckConsentNeeded = context => true;
			    options.MinimumSameSitePolicy = SameSiteMode.None;
		    });

		    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

		    //PlayZoneBotService.GetBotClientAsync(app.ApplicationServices).Wait();
		    //PopCornBotService.GetBotClientAsync(app.ApplicationServices).Wait();
			MemoryCacheHelper.ServiceProvider = app.ApplicationServices;
	    }
	}
}
