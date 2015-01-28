using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using RequireJsNet.Compressor.RequireProcessing;
using RequireJsNet.Compressor.Models;

namespace RequireJsNet.Compressor
{
	/// <summary>
	/// Initializes the use of Asp.Net bundling and minification in RequireJsNet
	/// </summary>
	public class RequireWebOptimization 
	{
		public string PackagePath { get; set; }

		public string[] RequireConfigs { get; set; }

		public string EntryPointOverride { get; set; }

		public bool AutoBundles { get; set; }

		/// <summary>
		/// Initializes the bundling
		/// </summary>
		/// <param name="encoding">The file encoding used to encode the .js files</param>
		/// <param name="autoBundles">Whether to use auto bundles or not</param>
		public RequireWebOptimization(bool autoBundles = true )
		{
			AutoBundles = autoBundles;
		}

		/// <summary>
		/// Creates bundles by using the web optimization framework
		/// </summary>
		public BundleCollection CreateBundles()
		{
			var files = new List<string>();
			if (RequireConfigs != null)
			{
				files = RequireConfigs.ToList();
			}

			var configProcessor = new AutoBundleConfigProcessor(files);

			// create the bundles
			return configProcessor.ParseConfigs();
		}
	}
}
