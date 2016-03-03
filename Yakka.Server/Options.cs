using System.Reflection;
using System.Text;
using CommandLine;

namespace Yakka.Server
{
	/// <summary>
	/// Reference documentation: https://github.com/gsscoder/commandline
	/// Extend with more options as necessary
	/// </summary>
	public class Options
	{
		[Option('p', "port", HelpText="Port that the server should listen on.", DefaultValue = 8081)]
		public int Port { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			// this without using CommandLine.Text
			//  or using HelpText.AutoBuild
			var usage = new StringBuilder();
			usage.AppendLine("Yakka.Server version " + Assembly.GetEntryAssembly().GetName().Version);
			usage.AppendLine("Read user manual for usage instructions...");
			return usage.ToString();
		}
	}
}
