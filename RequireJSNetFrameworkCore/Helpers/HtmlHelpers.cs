// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html
using Microsoft.AspNetCore.Mvc.Rendering;
using RequireJSNetFrameworkCore.Models;

namespace RequireJSNetFrameworkCore.Helpers
{
    internal static class HtmlHelpers
    {
        public static RoutingInfo GetRoutingInfo(this ViewContext viewContext)
        {
            var area = viewContext.RouteData.DataTokens["area"] != null
                ? viewContext.RouteData.DataTokens["area"].ToString()
                : "Root";

            var controller = viewContext.RouteData.Values["controller"].ToString();
            var action = viewContext.RouteData.Values["action"].ToString();
            return new RoutingInfo
            {
                Area = area,
                Controller = controller,
                Action = action
            };
        }
    }
}
