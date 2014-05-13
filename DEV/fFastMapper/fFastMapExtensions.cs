using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grax.fFastMapper
{
    public static class fFastMapExtensions
    {
        public static TReturn Map<TLeft, TReturn>(this fFastMapperFluent<TLeft, TReturn> fluent, TLeft source)
            where TReturn : class,new()
        {
            return fFastMapperInternal<TLeft, TReturn>.mapperFunc(source, new TReturn());
        }
    }
}
