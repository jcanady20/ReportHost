using Ninject;
using ReportHost.Logging;
using ReportHost.Data.Context;

namespace ReportHost
{
	public static class NinjectConfig
	{
        private static IKernel m_kernal;
		public static IKernel CreateKernel()
		{
            if(m_kernal == null)
            {
                m_kernal = new StandardKernel();
                RegisterBindings(m_kernal);
            }
			
			return m_kernal;
		}

		private static void RegisterBindings(IKernel kernel)
		{
			kernel.Bind<ILogger>().To<NLogger>();
			kernel.Bind<IContext>().ToMethod(x => ContextFactory.CreateContext());
		}
	}
}
