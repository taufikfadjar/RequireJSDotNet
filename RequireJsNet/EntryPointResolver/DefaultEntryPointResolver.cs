using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using RequireJsNet.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RequireJsNet.EntryPointResolver
{
    public class DefaultEntryPointResolver : IEntryPointResolver
    {
        private const string DefaultEntryPointRoot = "~/Scripts/";
        private const string DefaultArea = "Common";
        private readonly PathHelper pathHelper;

        public DefaultEntryPointResolver(IHostingEnvironment environment)
        {
            this.pathHelper = new PathHelper(environment);
        }

        public string Resolve(ViewContext viewContext, string entryPointRoot)
        {
            var routingInfo = viewContext.GetRoutingInfo();
            var rootUrl = string.Empty;
            var withBaseUrl = true;

            //if (entryPointRoot != DefaultEntryPointRoot)
            //{
            //    withBaseUrl = false;
            //    rootUrl = new UrlHelper(viewContext).Content(entryPointRoot);
            //}

            // search for controller/action.js in current area
            var entryPointTmpl = "Views/{0}/" + routingInfo.Controller + "/" + routingInfo.Action;
            var entryPoint = PathHelper.ToModuleName(string.Format(entryPointTmpl, routingInfo.Area));
            var filePath = this.pathHelper.MapPath(entryPointRoot + entryPoint + ".js");

            if (File.Exists(filePath))
            {
                var computedEntry = this.GetEntryPoint(filePath, entryPointRoot);
                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            }

            // search for controller/action.js in common area
            entryPoint = PathHelper.ToModuleName(string.Format(entryPointTmpl, DefaultArea));
            filePath = this.pathHelper.MapPath(entryPointRoot + entryPoint + ".js");

            if (File.Exists(filePath))
            {
                var computedEntry = this.GetEntryPoint(filePath, entryPointRoot);
                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            }

            // search for controller/controller-action.js in current area
            entryPointTmpl = "Controllers/{0}/" + routingInfo.Controller + "/" + routingInfo.Controller + "-" + routingInfo.Action;
            entryPoint = PathHelper.ToModuleName(string.Format(entryPointTmpl, routingInfo.Area));
            filePath = this.pathHelper.MapPath(entryPointRoot + entryPoint + ".js");

            if (File.Exists(filePath))
            {
                var computedEntry = this.GetEntryPoint(filePath, entryPointRoot);
                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
            }

            // search for controller/controller-action.js in common area
            entryPoint = PathHelper.ToModuleName(string.Format(entryPointTmpl, DefaultArea));
            filePath = this.pathHelper.MapPath(entryPointRoot + entryPoint + ".js");

            if (File.Exists(filePath))
            {
                var computedEntry = this.GetEntryPoint(filePath, entryPointRoot);
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
