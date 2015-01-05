using System.IO;
using System.Activities;

namespace Net.Bndy.ActivityLib
{
	public sealed class CopyFile : CodeActivity
	{
		[RequiredArgument]
		public InArgument<string> Source { get; set; }
		[RequiredArgument]
		public InArgument<string> Destination { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string source = context.GetValue(this.Source);
			string dest = context.GetValue(this.Destination);

			FileInfo fi = new FileInfo(source);

			if (!dest.EndsWith(fi.Name, System.StringComparison.OrdinalIgnoreCase))
			{
				// The desination is Directory
				if (!Directory.Exists(dest))
					Directory.CreateDirectory(dest);

				dest = Path.Combine(dest, fi.Name);
			}

			File.Copy(source, dest, true);
		}
	}
}
