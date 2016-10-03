using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequireJsNet.EntryPointResolver;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace RequireJsNet
{
    public class RequireEntryPointResolverCollection
    {
        private List<IEntryPointResolver> resolvers = new List<IEntryPointResolver>();

        public void Clear()
        {
            lock (resolvers)
            {
                resolvers.Clear();
            }
            
        }

        public bool IsEmpty()
        {
            return resolvers.Count == 0;
        }

        public void Prepend(IEntryPointResolver resolver)
        {
            lock (resolvers)
            {
                resolvers.Insert(0, resolver);
            }
        }

        public void Add(IEntryPointResolver resolver)
        {
            lock (resolvers)
            {
                resolvers.Add(resolver);
            }
        }

        internal string Resolve(ViewContext viewContext, string entryPointRoot)
        {
            string result = null;

            lock (resolvers)
            {
                foreach (var resolver in resolvers)
                {
                    result = resolver.Resolve(viewContext, entryPointRoot);
                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
