using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

using Microsoft.TeamFoundation.Build.Client;

namespace Net.Bndy.ActivityLib
{
	[BuildActivity(HostEnvironmentOption.All)]
	public sealed class BackupDatabase : CodeActivity
	{
		[RequiredArgument]
		public InArgument<string> ServerInstance { get; set; }
		[RequiredArgument]
		public InArgument<string> DatabaseName { get; set; }
		[RequiredArgument]
		public InArgument<string> UserName { get; set; }
		[RequiredArgument]
		public InArgument<string> Password { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string serverName = context.GetValue(this.ServerInstance);
			string dbName = context.GetValue(this.DatabaseName);
			string user = context.GetValue(this.UserName);
			string pwd = context.GetValue(this.Password);

			string device = string.Format("{0}_{1}", dbName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));

			if (!string.IsNullOrWhiteSpace(serverName)
				&& !string.IsNullOrWhiteSpace(dbName)
				&& !string.IsNullOrWhiteSpace(user)
				&& !string.IsNullOrWhiteSpace(pwd))
			{
				Server server = new Server(new ServerConnection(serverName, user, pwd));
				Backup bk = new Backup();

				bk.Database = dbName;
				bk.Action = BackupActionType.Database;
				bk.Devices.AddDevice(device + ".bak", DeviceType.File);
				bk.BackupSetName = device;
				bk.BackupSetDescription = string.Format("Auto-Backed up by WorkflowID#{0}", context.WorkflowInstanceId.ToString());
				bk.SqlBackup(server);
			}
		}
	}
}
