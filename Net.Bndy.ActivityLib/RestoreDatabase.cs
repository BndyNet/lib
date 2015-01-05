using System.Data;
using System.Activities;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Net.Bndy.ActivityLib
{
	public sealed class RestoreDatabase : CodeActivity
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

			if (!string.IsNullOrWhiteSpace(serverName)
						&& !string.IsNullOrWhiteSpace(dbName)
						&& !string.IsNullOrWhiteSpace(user)
						&& !string.IsNullOrWhiteSpace(pwd))
			{
				Server server = new Server(new ServerConnection(serverName, user, pwd));
				Database db = server.Databases[dbName];

				DataSet ds = db.ExecuteWithResults(@"
SELECT TOP 1
	s.database_name
   ,m.physical_device_name
   ,CAST(DATEDIFF(second, s.backup_start_date, s.backup_finish_date) AS VARCHAR(100))
	+ ' ' + 'Seconds' TimeTaken
   ,s.backup_start_date
   ,CASE s.[type]
	  WHEN 'D' THEN 'Full'
	  WHEN 'I' THEN 'Differential'
	  WHEN 'L' THEN 'Transaction Log'
	END AS BackupType
   ,s.server_name
   ,s.recovery_model
FROM
	msdb.dbo.backupset s
	INNER JOIN msdb.dbo.backupmediafamily m ON s.media_set_id = m.media_set_id
WHERE
	s.database_name = DB_NAME() -- Remove this line for all the database
ORDER BY
	backup_start_date DESC
   ,backup_finish_date
");
				if (ds != null && ds.Tables.Count > 0)
				{
					string bkpFile = ds.Tables[0].Rows[0]["physical_device_name"].ToString();

					Restore res = new Restore();
					res.Database = dbName;
					res.ReplaceDatabase = true;
					res.Action = RestoreActionType.Database;
					res.Devices.AddDevice(bkpFile, DeviceType.File);
					server.KillAllProcesses(dbName);
					res.SqlRestore(server);
				}
			}
		}
	}
}
