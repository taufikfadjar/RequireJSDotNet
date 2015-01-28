using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Web;

namespace RequireJsNet.Compressor.Helper
{
	internal class PathResolver
	{
		private readonly string _entryPoint;

		public PathResolver(string entryPoint)
		{
			_entryPoint = entryPoint;
		}

		public string RequirePathToVirtualPath(string requirePath, bool appendExtension = true)
		{
			if (requirePath.StartsWith("//"))
			{
				throw new ArgumentException(String.Format("Can not convert CDN path '{0}'", requirePath));
			}

			var path = GetOutputPath(_entryPoint, requirePath, appendExtension);

			// get absolute path for existence check
			var absolutePath = HttpContext.Current.Server.MapPath(path);

			// check for existence
			var exists = appendExtension ? File.Exists(absolutePath) : Directory.Exists(absolutePath);

			if(!exists)
			{
				throw new Exception(string.Format("The virtual path '{0}' is not valid", path));
			}

			return path;
		}

		public string VirtualPathToRequirePath(string virtualFilePath)
		{
			var requirePath = VirtualPathUtility.MakeRelative(_entryPoint, virtualFilePath);
			
			return requirePath.Replace(".js",string.Empty);
		}


		public string GetOutputPath(string basePath, string filePath, bool appendExtension = true)
		{
			var path = VirtualPathUtility.Combine(basePath, filePath);

			if (appendExtension)
			{
				path += ".js";
			}

			return path;
		}
	}
}
