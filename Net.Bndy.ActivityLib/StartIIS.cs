using System.Activities;
using System.ServiceProcess;

namespace Net.Bndy.ActivityLib
{
	public sealed class StartIIS : CodeActivity
	{
		[RequiredArgument]
		public InArgument<string> MachineName { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string machine = context.GetValue(this.MachineName);
			if (string.IsNullOrWhiteSpace(machine))
				machine = ".";

			ServiceController svc = new ServiceController("W3SVC", machine);
			if (svc != null && svc.Status != ServiceControllerStatus.Running)
			{
				svc.Start();
			}
		}
	}
}
