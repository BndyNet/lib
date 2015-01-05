using Microsoft.Win32;
using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Net.Bndy.Windows
{
	public class AppBase : Application
	{
		public AppBase()
		{
			// Register icon to Programs/Features in Control Panel
			if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
			{
				try
				{
					Assembly code = Assembly.GetCallingAssembly();
					AssemblyTitleAttribute asTitle =
					(AssemblyTitleAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyTitleAttribute));
					string assemblyDescription = asTitle.Title;

					//the icon is included in this program
					string iconSourcePath = string.Format("{0},{1}", code.Location, 0);

					RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Bndy.Net\Windows\CurrentVersion\Uninstall");
					string[] mySubKeyNames = myUninstallKey.GetSubKeyNames();
					for (int i = 0; i < mySubKeyNames.Length; i++)
					{
						RegistryKey myKey = myUninstallKey.OpenSubKey(mySubKeyNames[i], true);
						object myValue = myKey.GetValue("DisplayName");
						if (myValue != null && myValue.ToString() == assemblyDescription)
						{
							myKey.SetValue("DisplayIcon", iconSourcePath);
							break;
						}
					}
				}
				catch (Exception)
				{
					//log an error
				}
			}
		}
	}
}