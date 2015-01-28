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
using System.Web;
using System.Web.Optimization;
using RequireJsNet.Compressor.AutoDependency;
using RequireJsNet.Compressor.Helper;
using RequireJsNet.Compressor.Models;
using RequireJsNet.Configuration;
using RequireJsNet.Models;

namespace RequireJsNet.Compressor.RequireProcessing
{
	internal class AutoBundleConfigProcessor : ConfigProcessor
	{
		private PathResolver Resolver;

		public AutoBundleConfigProcessor(List<string> filePaths)
		{
			AlreadyProcessedFileNames = new List<string>();
			FilePaths = filePaths;

			EntryPoint = "~/Scripts/";
			Resolver = new PathResolver(EntryPoint);
		}

		public override BundleCollection ParseConfigs()
		{
			var bundles = new BundleCollection();
			

			FindConfigs();

			var loader = new ConfigLoader(
				FilePaths,
				new ExceptionThrowingLogger(),
				new ConfigLoaderOptions { ProcessBundles = true });

			// Load Configurations
			Configuration = loader.Get();

			// new builder instance includes the set configuration
			var builder = new RequireBuilder();
			
			// iterate over all bundle definitions of the configuration and build the equivalent web optimization bundles
			foreach (var bundle in Configuration.AutoBundles.Bundles)
			{
				var bundleResult = new RequireCompressorBundle(Resolver.GetOutputPath(bundle.OutputPath, bundle.Id), builder)
									   {
											ContainingConfig = bundle.ContainingConfig,
									   };

				// add bundle to collection
				bundles.Add(bundleResult);

				// construct queue utilities
				var includeQueue = new Queue<AutoBundleItem>();
				var alreadyProcessedFileNames = new List<string>();

				// initially enqueuing
				EnqueueBundles(bundle.Includes, includeQueue, alreadyProcessedFileNames);

				// construct dependency resolver
				var processor = new ScriptProcessor(Configuration, Resolver);

				// iterate over the includes
				while(includeQueue.Any())
				{
					var include = includeQueue.Dequeue();

					if (!string.IsNullOrEmpty(include.File))
					{
						// include file
						var filePath = Resolver.RequirePathToVirtualPath(include.File);
		
						var processedFile = processor.Process(filePath);
						bundleResult.Include(processedFile);
						alreadyProcessedFileNames.Add(include.File);

						EnqueueBundles(processedFile.Dependencies, includeQueue, alreadyProcessedFileNames);
					}
					else if (!string.IsNullOrEmpty(include.Directory))
					{

						// get virtual directory path
						var virtualDirectoryPath = Resolver.RequirePathToVirtualPath(include.Directory, false);
						// get absolute directory path
						var absoluteDirectoryPath = HttpContext.Current.Server.MapPath(virtualDirectoryPath);

						// get all js files in directory and subdirectories
						var files = Directory.EnumerateFiles(absoluteDirectoryPath, "*.js", SearchOption.AllDirectories);

						// convert to virtual paths and include in bundle
						foreach (var file in files.Select(r => virtualDirectoryPath + r.Replace(absoluteDirectoryPath, "").Replace("\\", "/")))
						{
							var processedFile = processor.Process(file);
							bundleResult.Include(processedFile);
							alreadyProcessedFileNames.Add(Resolver.VirtualPathToRequirePath(file));

							EnqueueBundles(processedFile.Dependencies, includeQueue, alreadyProcessedFileNames);
						}
					}
				}
	
			}

			Context = new BundleContext(new HttpContextWrapper(HttpContext.Current),bundles, EntryPoint);

			this.WriteOverrideConfigs(bundles);

			return bundles;
		}


		/// <summary>
		/// Pushes a set of bundles into a queue if the do not already exists in the queue or
		/// in an exclude set
		/// </summary>
		/// <param name="bundles">The set of bundles</param>
		/// <param name="queue">The existing queue</param>
		/// <param name="excludeItems">The set of file names to be excluded from enqueuing</param>
		private static void EnqueueBundles(IEnumerable<AutoBundleItem> bundles, Queue<AutoBundleItem> queue, IEnumerable<string> excludeItems)
		{
			foreach (var bundle in bundles)
			{
				if (!excludeItems.Any(r => string.Equals(r, bundle.File, StringComparison.CurrentCultureIgnoreCase))
					&& !queue.Any(r => string.Equals(r.File, bundle.File, StringComparison.CurrentCultureIgnoreCase)))
				{
					queue.Enqueue(bundle);
				}
			}
		}

		private void WriteOverrideConfigs(BundleCollection bundles)
		{
			var groupings = bundles.GroupBy(r => ((RequireCompressorBundle)r).ContainingConfig).ToList();
			foreach (var grouping in groupings)
			{
				var path = RequireJsNet.Helpers.PathHelpers.GetOverridePath(grouping.Key);
				var writer = WriterFactory.CreateWriter(path, null);
				var collection = this.ComposeCollection(grouping.ToList());
				writer.WriteConfig(collection);    
			}
		}

		private ConfigurationCollection ComposeCollection(IEnumerable<Bundle> bundles)
		{
			var conf = new ConfigurationCollection();
			conf.Overrides = new List<CollectionOverride>();
			foreach (var bundle in bundles)
			{
				var scripts = bundle.EnumerateFiles(Context).Select(r => Resolver.VirtualPathToRequirePath(r.IncludedVirtualPath)).ToList();
				var paths = new RequirePaths
								{
									PathList = new List<RequirePath>()
								};
				foreach (var script in scripts)
				{
					paths.PathList.Add(new RequirePath
										   {
											   Key = script,
											   Value = Resolver.VirtualPathToRequirePath(bundle.Path)
										   });
				}

				var over = new CollectionOverride
							   {
								   BundleId = bundle.Path,
								   BundledScripts = scripts,
								   Paths = paths
							   };
				conf.Overrides.Add(over);
			}

			return conf;
		}
	}
}
