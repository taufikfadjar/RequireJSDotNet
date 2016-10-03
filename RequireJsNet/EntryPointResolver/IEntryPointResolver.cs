using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RequireJsNet.EntryPointResolver
{
    public interface IEntryPointResolver
    {
        string Resolve(ViewContext viewContext, string entryPointRoot);
    }
}
