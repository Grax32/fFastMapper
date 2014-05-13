using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Grax.fFastMapper
{
    public class fFastMapperFluent<TLeft, TRight>
    {
        internal fFastMapperFluent() { }

        public fFastMapperFluent<TLeft, TRight> ClearMappers()
        {
            fFastMapperInternal<TLeft, TRight>.ClearMappers(fFastMap.CallReverseTrue);
            return this;
        }

        [Obsolete]
        public fFastMapperFluent<TLeft, TRight> ClearMappers(bool oneWay)
        {
            var callReverse = !oneWay;
            fFastMapperInternal<TLeft, TRight>.ClearMappers(callReverse);
            return this;
        }

        public fFastMapperFluent<TLeft, TRight> DeletePropertyMapper<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty)
        {
            fFastMapperInternal<TLeft, TRight>.DeletePropertyMapper<TPropertyType>(leftProperty, fFastMap.CallReverseTrue);
            return this;
        }

        [Obsolete]
        [ExcludeFromCodeCoverage]
        public fFastMapperFluent<TLeft, TRight> DeletePropertyMapper<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, bool oneWay)
        {
            var callReverse = !oneWay;
            fFastMapperInternal<TLeft, TRight>.DeletePropertyMapper<TPropertyType>(leftProperty, callReverse);
            return this;
        }

        public fFastMapperFluent<TLeft, TRight> AddPropertyMapper<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, Expression<Func<TRight, TPropertyType>> rightProperty)
        {
            fFastMapperInternal<TLeft, TRight>.AddPropertyMapperByExpression<TPropertyType>(leftProperty, rightProperty, fFastMap.CallReverseTrue);
            return this;
        }

        public fFastMapperFluent<TLeft, TRight> AddPropertyMapper<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, Expression<Func<TRight, TPropertyType>> rightProperty, bool oneWay)
        {
            fFastMapperInternal<TLeft, TRight>.AddPropertyMapperByExpression<TPropertyType>(leftProperty, rightProperty, !oneWay);
            return this;
        }

        public fFastMapperFluent<TLeft, TRight> AddDefaultPropertyMappers()
        {
            fFastMapperInternal<TLeft, TRight>.AddPropertyMappingByMatchedNameAndType(fFastMap.CallReverseTrue);
            return this;
        }

        [Obsolete]
        [ExcludeFromCodeCoverage]
        public fFastMapperFluent<TLeft, TRight> AddDefaultPropertyMappers(bool oneWay)
        {
            var callReverse = !oneWay;
            fFastMapperInternal<TLeft, TRight>.AddPropertyMappingByMatchedNameAndType(callReverse);
            return this;
        }

        public TRight Map(TLeft source, TRight destination)
        {
            return fFastMapperInternal<TLeft, TRight>.mapperFunc(source, destination);
        }

        public Func<TLeft, TRight, TRight> GetMapFunction()
        {
            return fFastMapperInternal<TLeft, TRight>.mapperFunc;
        }

        public List<Tuple<string, string>> Mappings()
        {
            return fFastMapperInternal<TLeft, TRight>.Mappings();
        }

        public fFastMapperFluent<TLeft, TRight> SetMappingDirection(fFastMap.MappingDirection mappingDirection)
        {
            fFastMapperInternal<TLeft, TRight>.MappingDirection = mappingDirection;
            return this;
        }

        public string MappingView()
        {
            return fFastMapperInternal<TLeft, TRight>.MappingsView();
        }

        public fFastMapperFluent<TRight, TLeft> Reverse()
        {
            return fFastMapperInternal<TRight, TLeft>.fFastMapFluent;
        }

        public fFastMapperFluent<TLeft,TRight> SetMaxRecursionLevel(int maxRecursionLevel)
        {
            fFastMapperInternal<TLeft, TRight>.MaxRecursionLevel = maxRecursionLevel;
            return this;
        }
    }
}
