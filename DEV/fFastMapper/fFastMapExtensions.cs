using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grax.fFastMapper
{
    public static class fFastMapExtensions
    {
        public static TReturn Map<TLeft, TReturn>(this fFastMapperFluent<TLeft, TReturn> fluent, TLeft source)
            where TReturn : new()
        {
            return fFastMapperInternal<TLeft, TReturn>.mapperFunc(source, new TReturn());
        }
        
        internal static bool IsTypeMatchable(this Type type)
        {
            if (typeof(string).IsAssignableFrom(type))
            {
                return true;
            }

            return type.IsClass == false && type.IsInterface == false;
        }
    }
}
