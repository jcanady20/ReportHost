using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ReportHost.Data.Context;

namespace ReportHost.Service.api
{
	public class BaseApiController : ApiController
	{
		internal IContext m_db;

		internal BaseApiController()
		{
			m_db = ContextFactory.CreateContext();
		}

		protected override void Dispose(bool disposing)
		{
			m_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
