﻿using System;
using System.Collections.Generic;

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RequireJSNetFrameworkCore.Models;

namespace RequireJSNetFrameworkCore
{
    public class RequireRendererConfiguration
    {
        private string baseUrl = string.Empty;

        private string requireJsUrl = string.Empty;

        private string urlArgs = null;

        private string entryPointRoot = "~/Scripts/";

        private bool loadOverrides = true;

        private IList<string> configPaths = new[] { "~/RequireJS.json" };

        public RequireRendererConfiguration()
        {
            LocaleSelector = helper => CultureInfo.CurrentUICulture.Name.Split('-')[0];
            ProcessConfig = config => { };
            ProcessOptions = options => { };
        }

        /// <summary>
        /// Scripts base url
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return baseUrl;
            }

            set
            {
                baseUrl = value;
            }
        }

        /// <summary>
        /// RequireJS javascript file url
        /// </summary>
        public string RequireJsUrl
        {
            get
            {
                return this.requireJsUrl;
            }

            set
            {
                this.requireJsUrl = value;
            }
        }

        /// <summary>
        /// Value passed to the urlArgs setting of RequireJS
        /// </summary>
        public string UrlArgs
        {
            get
            {
                return urlArgs;
            }

            set
            {
                urlArgs = value;
            }
        }

        /// <summary>
        /// List of RequireJS config file paths
        /// </summary>
        public IList<string> ConfigurationFiles
        {
            get
            {
                return configPaths;
            }

            set
            {
                configPaths = value;
            }
        }

        /// <summary>
        /// Scripts folder relative path (ex. ~/Scripts/)
        /// </summary>
        public string EntryPointRoot
        {
            get
            {
                return entryPointRoot;
            }

            set
            {
                entryPointRoot = value;
            }
        }

        /// <summary>
        /// Logger for library output
        /// </summary>
        public IRequireJsLogger Logger { get; set; }


        /// <summary>
        /// A value indicating whether overrides generated by the AutoCompressor shoudl eb loaded.
        /// </summary>
        public bool LoadOverrides
        {
            get
            {
                return loadOverrides;
            }

            set
            {
                loadOverrides = value;
            }
        }

        /// <summary>
        /// Gets or sets a function that returns the current locale in short format (ex. "en")
        /// </summary>
        public Func<ActionContext, string> LocaleSelector { get; set; }

        public Action<JsonRequireOutput> ProcessConfig { get; set; }

        public Action<JsonRequireOptions> ProcessOptions { get; set; }
    }
}
