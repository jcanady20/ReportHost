using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace LicenseManager
{
	[RunInstaller(true)]
	public class ServiceInstall : System.Configuration.Install.Installer
	{
		public ServiceInstall()
		{
			ServiceProcessInstaller process = new ServiceProcessInstaller();
			ServiceInstaller serviceAdmin = new ServiceInstaller();

			process.Account = ServiceAccount.LocalSystem;

			serviceAdmin.StartType = ServiceStartMode.Automatic;
			serviceAdmin.ServiceName = "ReportHostService";
			serviceAdmin.DisplayName = "Report Host Service";
			serviceAdmin.Description = "This service is used to host Report Services for VisDoc";

			Installers.Add(process);
			Installers.Add(serviceAdmin);

		}
	}
}
