using Ninject;
using ReportHost.Logging;
using ReportHost.Data.Context;

namespace ReportHost
{
	public static class NinjectConfig
	{
		public static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			RegisterBindings(kernel);
			return kernel;
		}

		private static void RegisterBindings(IKernel kernel)
		{
			kernel.Bind<ILogger>().To<NLogger>();
			kernel.Bind<IContext>().ToMethod(x => ContextFactory.CreateContext());
		}
	}
}
