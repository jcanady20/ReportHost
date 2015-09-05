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
	public class NinjectDependencyScope : IDependencyScope
	{
		private IKernel m_kernel;
		public NinjectDependencyScope(IKernel kernel)
		{
			m_kernel = kernel;
		}
				
		public object GetService(Type serviceType)
		{
			if(m_kernel == null)
			{
				throw new ObjectDisposedException("this", "This scope has already been disposed");
			}
			return m_kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			if (m_kernel == null)
			{
				throw new ObjectDisposedException("this", "This scope has already been disposed");
			}
			return m_kernel.GetAll(serviceType);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				var disposable = m_kernel as IDisposable;
				if(disposable != null)
				{
					disposable.Dispose();
					m_kernel = null;
				}
			}
		}
	}
}
