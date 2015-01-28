using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Optimization;

namespace RequireJsNet.Compressor.Helper
{
	class BundleFileEqualityComparer : IEqualityComparer<BundleFile>
	{
		public bool Equals(BundleFile x, BundleFile y)
		{
			return x.IncludedVirtualPath.ToLower().Equals(y.IncludedVirtualPath.ToLower());
		}

		public int GetHashCode(BundleFile obj)
		{
			return obj.GetHashCode();
		}
	}
}
