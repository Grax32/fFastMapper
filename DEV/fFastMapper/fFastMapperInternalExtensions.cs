using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Grax.fFastMapper
{
    internal static class fFastMapperInternalExtensions
    {
        internal static IEnumerable<PropertyInfo> WhereAssignableFrom(this IEnumerable<PropertyInfo> properties, Type otherType)
        {
            return properties.Where(v => v.PropertyType.IsAssignableFrom(otherType));
        }
    }
}
