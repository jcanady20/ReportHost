using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace ReportHost.DependencyInjector
{
	public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
	{
		private IKernel m_kernel;

		public NinjectDependencyResolver(IKernel kernel) : base(kernel)
		{
			m_kernel = kernel;
		}


		public IDependencyScope BeginScope()
		{
			return new NinjectDependencyScope(m_kernel);
		}
	}
}
