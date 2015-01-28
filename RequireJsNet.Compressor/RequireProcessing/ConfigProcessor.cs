// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using RequireJsNet.Models;

namespace RequireJsNet.Compressor
{
	internal abstract class ConfigProcessor
	{
		protected const string ConfigFileName = "RequireJS.*";

		protected const string DefaultScriptDirectory = "Scripts";

		protected BundleContext Context;

		protected string EntryPoint { get; set; }

		protected ConfigurationCollection Configuration { get; set; }

		protected string OutputPath { get; set; }

		protected List<string> FilePaths { get; set; }

		protected List<string> AlreadyProcessedFileNames { get; set; }

		public abstract BundleCollection ParseConfigs();
		

		protected void FindConfigs()
		{
			if (FilePaths.Any())
			{
				return;
			}
			
			var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("/"), ConfigFileName);
			foreach (var file in files.Where(r => !r.ToLower().Contains(".override.")))
			{
				FilePaths.Add(file);
			}

			if (!FilePaths.Any())
			{
				throw new ArgumentException("No Require config files were provided and none were found in the project directory.");
			}
		}
	}
}
