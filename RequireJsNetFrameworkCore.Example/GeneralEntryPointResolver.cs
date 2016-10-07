using Microsoft.AspNetCore.Hosting;
using RequireJSNetFrameworkCore.EntryPointResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using RequireJSNetFrameworkCore.Helpers;

namespace RequireJsNetFrameworkCore.Example
{
    public class GeneralEntryPointResolver : DefaultEntryPointResolver
    {
        private readonly PathHelper pathHelper;

        public GeneralEntryPointResolver(IHostingEnvironment environment):base(environment)
        {
            this.pathHelper = new PathHelper(environment);
        }

        public override string Resolve(ViewContext viewContext, string entryPointRoot)
        {
            var resolverResult =  base.Resolve(viewContext, entryPointRoot);

            var generalEntryPointPathFile = "Index";


            if (resolverResult == null)
            {
               return ResolveEntryPointRoot(entryPointRoot, generalEntryPointPathFile);
            }

            return null;
        }

        private string ResolveEntryPointRoot(string entryPointRoot, string generalEntryPointPathFile)
        {
            var withBaseUrl = true;
            var rootUrl = string.Empty;

            var filePath = this.pathHelper.MapPath(entryPointRoot + generalEntryPointPathFile + ".js");

            if (File.Exists(filePath))
            {
                var computedEntry = GetEntryPoint(filePath, entryPointRoot);
                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            }

            return null;

        }


        private string GetEntryPoint(string filePath, string root)
        {
            var fileName = PathHelper.GetExactFilePath(filePath);
            var folder = this.pathHelper.MapPath(root);
            return PathHelper.GetRequireRelativePath(folder, fileName);
        }
    }
}
