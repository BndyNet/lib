using System.IO;
using System.Activities;

namespace Net.Bndy.ActivityLib
{
	public sealed class DeleteDirectoryOrFile : CodeActivity
	{
		public InArgument<string> Target { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string file = context.GetValue(this.Target);

			if (Directory.Exists(file))
				Directory.Exists(file);
			else if (File.Exists(file))
				File.Exists(file);

		}
	}
}
