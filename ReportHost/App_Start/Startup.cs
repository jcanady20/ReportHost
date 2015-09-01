using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Cors;
using System.Web.Http;
using System.IO;
using System.Reflection;


namespace ReportHost
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var root = ".";

			//app.Use(typeof(OwinStats.StatsMiddleWare));
			app.UseCors(CorsOptions.AllowAll);

			HttpConfiguration config = new Configuration();

			//	WEB API CORS Support
			config.MessageHandlers.Add(new ReportHost.Service.MessageHandlers.WebApiCorsHandler());
			app.UseWebApi(config);

			//	Static File Hosting for Licensing
			var fileSystem = new PhysicalFileSystem(root);
			var fileOptions = new FileServerOptions();
			fileOptions.EnableDirectoryBrowsing = false;
			app.UseFileServer(fileOptions);
		}
	}
}
