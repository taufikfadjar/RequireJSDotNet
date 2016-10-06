// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

namespace RequireJSNetFrameworkCore.Configuration
{
    using RequireJSNetFrameworkCore.Models;

    public interface IConfigWriter
    {
        string Path { get; }

        void WriteConfig(ConfigurationCollection conf);
    }
}
