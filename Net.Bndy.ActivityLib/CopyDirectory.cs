using System.IO;
using System.Activities;

namespace Net.Bndy.ActivityLib
{
	public sealed class CopyDirectory : CodeActivity
	{
		[RequiredArgument]
		public InArgument<string> Source { get; set; }
		[RequiredArgument]
		public InArgument<string> Destination { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			string source = context.GetValue(this.Source);
			string dest = context.GetValue(this.Destination);

			if (!Directory.Exists(dest))
				Directory.CreateDirectory(dest);

			CopyDir(source, dest);
		}

		private void CopyDir(string source, string dest)
		{
			if (Directory.Exists(source))
			{
				foreach (var item in Directory.GetFileSystemEntries(source))
				{
					if (Directory.Exists(item))
						CopyDir(Path.Combine(source, item), Path.Combine(dest, item));
					else
						File.Copy(Path.Combine(source, item), Path.Combine(dest, item));
				}
			}
		}
	}
}
