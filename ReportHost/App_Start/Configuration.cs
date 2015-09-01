using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace ReportHost
{
	internal class Configuration : HttpConfiguration
	{
		public Configuration()
		{
			ConfigureRoutes();
			ConfigureJsonSerialization();
		}

		private void ConfigureRoutes()
		{
			this.MapHttpAttributeRoutes();
			Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
		}

		private void ConfigureJsonSerialization()
		{
			var jsonSettings = Formatters.JsonFormatter.SerializerSettings;
			jsonSettings.Formatting = Formatting.Indented;
			jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}
	}
}
