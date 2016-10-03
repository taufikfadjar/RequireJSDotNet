// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using RequireJsNet.EntryPointResolver;
using Microsoft.AspNetCore.Http;

namespace RequireJsNet
{
    public enum RequireJsOptionsScope
    {
        Page,
        Global
    }

    public static class RequireJsOptions
    {
        private const string GlobalOptionsKey = "globalOptions";

	    private static Dictionary<string, object> websiteOptions;

	    private static bool websiteOptionsLocked = false;

        private const string PageOptionsKey = "pageOptions";

        public static readonly RequireEntryPointResolverCollection ResolverCollection = new RequireEntryPointResolverCollection();
        
	    public static void LockGlobalOptions()
	    {
		    websiteOptionsLocked = true;
	    }

        public static Dictionary<string, object> GetGlobalOptions()
        {
			if (websiteOptions == null)
            {
				websiteOptions = new Dictionary<string, object>();
            }

			return websiteOptions;
        }

		public static Dictionary<string, object> GetPageOptions(HttpContext context)
        {
            var page = context.Items[PageOptionsKey] as Dictionary<string, object>;
            if (page == null)
            {
                context.Items[PageOptionsKey] = new Dictionary<string, object>();
            }

            return (Dictionary<string, object>)context.Items[PageOptionsKey];
        }             

        public static void AddRequireJsOption(this HttpContext context, string key, object value, RequireJsOptionsScope scope = RequireJsOptionsScope.Page)
        {
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    var pageOptions = GetPageOptions(context);
                    if (pageOptions.Keys.Contains(key))
                    {
                        pageOptions.Remove(key);
                    }

                    pageOptions.Add(key, value);
                    break;
                case RequireJsOptionsScope.Global:
		            if (websiteOptionsLocked)
		            {
			            throw new InvalidOperationException("Global Options are locked and can not be modified");
		            }

                    var globalOptions = GetGlobalOptions();
                    if (globalOptions.Keys.Contains(key))
                    {
                        globalOptions.Remove(key);
                    }

                    globalOptions.Add(key, value);
                    break;
            }
        }


        public static void AddRequireJsOption(
            this HttpContext context, 
            string key,
            Dictionary<string, object> value,
            RequireJsOptionsScope scope = RequireJsOptionsScope.Page,
            bool clearExisting = false)
        {
            var dictToModify = new Dictionary<string, object>();
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    dictToModify = GetPageOptions(context);
                    break;
                case RequireJsOptionsScope.Global:
					if (websiteOptionsLocked)
					{
						throw new InvalidOperationException("Global Options are locked and can not be modified");
					}
                    dictToModify = GetGlobalOptions();
                    break;
            }

            var existing = dictToModify.FirstOrDefault(r => r.Key == key).Value;
            if (existing != null)
            {
                if (!clearExisting && existing is Dictionary<string, object>)
                {
                    AppendItems(existing as Dictionary<string, object>, value);
                }
                else
                {
                    dictToModify.Remove(key);
                    dictToModify.Add(key, value);
                }
            }
            else
            {
                dictToModify.Add(key, value);
            }
        }

        public static object GetRequireJsOptionByKey(this HttpContext context, string key, RequireJsOptionsScope scope)
        {
            return scope == RequireJsOptionsScope.Page ? GetPageOptions(context).FirstOrDefault(r => r.Key == key)
                                                       : GetGlobalOptions().FirstOrDefault(r => r.Key == key);
        }

        public static void ClearRequireJsOptions(this HttpContext context, RequireJsOptionsScope scope)
        {
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    GetPageOptions(context).Clear();
                    break;
                case RequireJsOptionsScope.Global:
                    GetGlobalOptions().Clear();
                    break;
            }
        }

        public static void ClearAllRequireJsOptions(this HttpContext context)
        {
            ClearRequireJsOptions(context, RequireJsOptionsScope.Global);
            ClearRequireJsOptions(context, RequireJsOptionsScope.Page);
        }
        

        private static void AppendItems(Dictionary<string, object> to, Dictionary<string, object> from)
        {
            foreach (var item in from)
            {
                var existing = to.FirstOrDefault(r => item.Key == r.Key).Value;
                if (existing != null)
                {
                    to.Remove(item.Key);
                }

                to.Add(item.Key, item.Value);
            }
        }        
    }
}