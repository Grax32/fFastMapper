using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Grax.fFastMapper
{
    internal class fFastMapperGlobal
    {
        internal struct TypeMatchData
        {
            public string Prefix;
            public Type Type;
            public Expression Expression;

            public TypeMatchData(TypeMatchData source)
            {
                Prefix = source.Prefix;
                Type = source.Type;
                Expression = source.Expression;
            }
        }
    }
}
