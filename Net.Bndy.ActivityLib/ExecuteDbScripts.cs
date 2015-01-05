using System;
using System.Activities;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Net.Bndy.ActivityLib
{
	public sealed class ExecuteDbScripts : CodeActivity
	{
		[RequiredArgument]
		public InArgument<string> ServerInstance { get; set; }
		[RequiredArgument]
		public InArgument<string> DatabaseName { get; set; }
		[RequiredArgument]
		public InArgument<string> UserName { get; set; }
		[RequiredArgument]
		public InArgument<string> Password { get; set; }
		[RequiredArgument]
		public InArgument<string> Scripts { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string serverName = context.GetValue(this.ServerInstance);
			string dbName = context.GetValue(this.DatabaseName);
			string user = context.GetValue(this.UserName);
			string pwd = context.GetValue(this.Password);
			string text = context.GetValue(this.Scripts);

			if (!string.IsNullOrWhiteSpace(serverName)
				&& !string.IsNullOrWhiteSpace(dbName)
				&& !string.IsNullOrWhiteSpace(user)
				&& !string.IsNullOrWhiteSpace(pwd)
				&& !string.IsNullOrWhiteSpace(text))
			{
				Server server = new Server(new ServerConnection(serverName, user, pwd));
				Database db = server.Databases[dbName];
				db.ExecuteNonQuery(text);
			}
		}
	}
}
