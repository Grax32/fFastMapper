using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Grax.fFastMapper
{
    public static class fFastMapperInternal<TLeft, TRight>
    {
        static fFastMapperInternal()
        {
            if (fFastMap.AutoInitialize)
            {
                bool quitSilentlyIfMappingStarted = true;
                AddDefaultMappings(IsMappingBidirectional, quitSilentlyIfMappingStarted);
            }
        }

        internal static fFastMap.MappingDirection MappingDirection = fFastMap.DefaultMappingDirection;
        private static bool IsMappingBidirectional
        {
            get
            {
                bool retval = (MappingDirection == fFastMap.MappingDirection.Bidirectional);
                retval &= (typeof(TLeft) != typeof(TRight));
                return retval;
            }
        }
        private static readonly fFastMapperFluent<TLeft, TRight> _fFastMapFluent = new fFastMapperFluent<TLeft, TRight>();
        private static readonly List<Tuple<PropertyInfo, PropertyInfo>> propertyMaps = new List<Tuple<PropertyInfo, PropertyInfo>>();
        private static readonly List<Tuple<Expression, Expression>> propertyExpressionMaps = new List<Tuple<Expression, Expression>>();

        public static fFastMapperFluent<TLeft, TRight> fFastMapFluent
        {
            get
            {
                return _fFastMapFluent;
            }
        }

        internal static void ClearMappers(bool callReverse)
        {
            propertyMaps.Clear();
            propertyExpressionMaps.Clear();
            CompileMapper();

            if (IsMappingBidirectional && callReverse)
            {
                fFastMapperInternal<TRight, TLeft>.ClearMappers(fFastMap.CallReverseFalse);
            }
        }

        internal static void AddPropertyMapperByExpression<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, Expression<Func<TRight, TPropertyType>> rightProperty)
        {
            AddPropertyMapperByExpression<TPropertyType>(leftProperty, rightProperty, fFastMap.CallReverseTrue);
        }

        internal static void AddPropertyMapperByExpression<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, Expression<Func<TRight, TPropertyType>> rightProperty, bool callReverse)
        {
            var leftPropertyMemberExpression = leftProperty.Body as MemberExpression;
            if (leftPropertyMemberExpression == null)
            {
                throw new ArgumentException("Must contain a MemberExpression", "leftProperty");
            }

            var rightPropertyMemberExpression = rightProperty.Body as MemberExpression;
            if (rightPropertyMemberExpression == null)
            {
                throw new ArgumentException("Must contain a MemberExpression", "rightProperty");
            }

            if (((PropertyInfo)leftPropertyMemberExpression.Member).CanRead == false)
            {
                throw new fFastMap.fFastMapException(string.Format("Source property must be readable for {0}.{1}", typeof(TLeft).Name, leftPropertyMemberExpression.ToString()));
            }

            if (((PropertyInfo)rightPropertyMemberExpression.Member).CanWrite == false)
            {
                throw new fFastMap.fFastMapException(string.Format("Destination property must be writeable for {0}.{1}", typeof(TRight).Name, rightPropertyMemberExpression.ToString()));
            }

            if (leftPropertyMemberExpression.Expression is ParameterExpression && rightPropertyMemberExpression.Expression is ParameterExpression)
            {
                AddPropertyMapperByPropertyInfo((PropertyInfo)leftPropertyMemberExpression.Member, (PropertyInfo)rightPropertyMemberExpression.Member);
            }
            else
            {
                propertyExpressionMaps.Add(Tuple.Create((Expression)leftPropertyMemberExpression, (Expression)rightPropertyMemberExpression));
            }

            CompileMapper();

            if (callReverse && IsMappingBidirectional)
            {
                fFastMapperInternal<TRight, TLeft>.AddPropertyMapperByExpression<TPropertyType>(rightProperty, leftProperty, fFastMap.CallReverseFalse);
            }
        }

        internal static void AddPropertyMapperByPropertyInfo(PropertyInfo leftProperty, PropertyInfo rightProperty)
        {
            propertyMaps.Add(Tuple.Create(leftProperty, rightProperty));
        }

        internal static void AddPropertyMappingByMatchedNameAndType(bool callReverse)
        {
            AddDefaultMappings(callReverse, false);
        }

        internal static void AddDefaultMappings(bool callReverse, bool quitSilentlyIfMappingStarted)
        {
            if (propertyMaps.Count > 0 || propertyExpressionMaps.Count > 0)
            {
                if (quitSilentlyIfMappingStarted) { return; }

                throw new fFastMap.fFastMapException("AddDefaultMappings must be called before other mappings are added.");
            }

            var leftType = typeof(TLeft);
            var rightType = typeof(TRight);

            var leftList = PropertyList<TLeft>.Properties;
            var rightList = PropertyList<TRight>.Properties;

            var leftKeys = PropertyList<TLeft>.MatchingKeys;
            var rightKeys = PropertyList<TRight>.MatchingKeys;

            var keyList = leftKeys.Keys.Intersect(rightKeys.Keys);

            foreach (var key in keyList)
            {
                var leftKey = leftKeys[key];
                var rigthKey = rightKeys[key];

                //Debug.WriteLine("{0} == {1}", leftList[leftKey].ToString(), rightList[rigthKey].ToString());

                var leftProperty = leftList[leftKey];
                var rightProperty = rightList[rigthKey];

                var leftPropertyInfo = (PropertyInfo)leftProperty.Member;
                var rightPropertyInfo = (PropertyInfo)rightProperty.Member;

                if (leftPropertyInfo.CanRead && rightPropertyInfo.CanWrite)
                {
                    propertyExpressionMaps.Add(Tuple.Create((Expression)leftProperty, (Expression)rightProperty));
                }
            }


            //var leftProperties = leftType.GetProperties().Where(v => v.CanRead);
            //var rightProperties = rightType.GetProperties().Where(v => v.CanWrite);

            //foreach (var leftProperty in leftProperties)
            //{
            //    var rightProperty = rightProperties.Where(v => v.Name == leftProperty.Name && v.PropertyType == leftProperty.PropertyType).SingleOrDefault();

            //    var propertyType = leftProperty.PropertyType;

            //    if (rightProperty != null && propertyType.IsTypeMatchable())
            //    {
            //        AddPropertyMapperByPropertyInfo(leftProperty, rightProperty);
            //    }
            //    else
            //    {
            //        //if (


            //        // SubSubDescription maps to Sub.Sub.Description
            //        // Sub.Sub.Description maps to SubSubDescription

            //        //foreach (var rightPropertyMatch in rightProperties.Where(v => v.Name != leftProperty.Name && leftProperty.Name.StartsWith(v.Name)))
            //        //{
            //        //    Expression propertyExpression = Expression.Parameter(leftType);
            //        //    if (FindNameMatch(leftProperty.Name, leftType, ref propertyExpression))
            //        //    {
            //        //        Debug.Print("Matched " + leftProperty.Name + " to " + propertyExpression.ToString());
            //        //    }
            //        //}

            //        //foreach (var rightPropertyMatch in rightProperties.Where(v => v.Name != leftProperty.Name && v.Name.StartsWith(leftProperty.Name)))
            //        //{
            //        //    Expression propertyExpression = Expression.Parameter(leftType);
            //        //    if (FindNameMatch(rightPropertyMatch.Name, rightType, ref propertyExpression))
            //        //    {
            //        //        Debug.Print("Matched " + rightPropertyMatch.Name + " to " + propertyExpression.ToString());
            //        //    }
            //        //}
            //    }
            //}

            if (callReverse && IsMappingBidirectional)
            {
                fFastMapperInternal<TRight, TLeft>.AddDefaultMappings(fFastMap.CallReverseFalse, quitSilentlyIfMappingStarted);
            }

            CompileMapper();
        }

        internal static void CompileMapper()
        {
            var leftParam = Expression.Parameter(typeof(TLeft), "left");
            var rightParam = Expression.Parameter(typeof(TRight), "right");
            var parameters = new ParameterExpression[] { leftParam, rightParam };
            var expressions = new List<Expression>();

            expressions.Add(NullCheckExpression(leftParam));
            expressions.Add(NullCheckExpression(rightParam));

            foreach (var propertyMap in propertyMaps)
            {
                var leftPropertyExpression = Expression.Property(leftParam, propertyMap.Item1);
                var rightPropertyExpression = Expression.Property(rightParam, propertyMap.Item2);
                var assignmentExpression = Expression.Assign(rightPropertyExpression, leftPropertyExpression);
                expressions.Add(assignmentExpression);
            }

            foreach (var propertyExpressionMap in propertyExpressionMaps)
            {
                var getProperty = ParameterExpressionModifier.ReplaceAllParameterExpression(propertyExpressionMap.Item1, leftParam);
                var setProperty = ParameterExpressionModifier.ReplaceAllParameterExpression(propertyExpressionMap.Item2, rightParam);

                var nullCheckGet = DeepNullCheckExpression(getProperty);
                var nullCheckSet = DeepNullCheckExpression(setProperty);

                Expression condition;

                if (nullCheckGet == null & nullCheckSet == null)
                {
                    expressions.Add(Expression.Assign(setProperty, getProperty));
                    continue;
                }
                else if (nullCheckGet == null)
                {
                    condition = nullCheckSet;
                }
                else if (nullCheckSet == null)
                {
                    condition = nullCheckGet;
                }
                else
                {
                    condition = Expression.AndAlso(nullCheckGet, nullCheckSet);
                }

                var ifThen = Expression.IfThen(condition, Expression.Assign(setProperty, getProperty));
                expressions.Add(ifThen);
            }

            expressions.Add(rightParam);

            var body = Expression.Block(expressions);

            var mapperExpression = Expression.Lambda<Func<TLeft, TRight, TRight>>(body, parameters);
            mapperFunc = mapperExpression.Compile();
        }

        static BinaryExpression DeepNullCheckExpression(Expression expressionToCheck)
        {
            return DeepNullCheckMemberExpression((MemberExpression)expressionToCheck);
        }

        static BinaryExpression DeepNullCheckMemberExpression(MemberExpression expressionToCheck)
        {
            if (expressionToCheck.Expression is MemberExpression)
            {
                var isNotNull = Expression.NotEqual(expressionToCheck.Expression, Expression.Constant(null));

                var leftExpression = DeepNullCheckMemberExpression(expressionToCheck.Expression as MemberExpression);

                if (leftExpression == null)
                {
                    return isNotNull;
                }
                else
                {
                    return Expression.AndAlso(leftExpression, isNotNull);
                }
            }

            return null;
        }

        internal static Expression NullCheckExpression(ParameterExpression expressionToCheck)
        {
            var isNull = Expression.Equal(expressionToCheck, Expression.Constant(null));
            var argumentException = new ArgumentException("Value must not be null", expressionToCheck.Name);
            return Expression.IfThen(isNull, Expression.Throw(Expression.Constant(argumentException)));
        }

        internal static Func<TLeft, TRight, TRight> mapperFunc = (left, right) => { throw new fFastMap.fFastMapException(string.Format("Mapping has not been configured from {0} to {1} ", typeof(TLeft).Name, typeof(TRight).Name)); };

        /// <summary>
        /// Read-only list of mappings
        /// </summary>
        /// <returns></returns>
        internal static List<Tuple<string, string>> Mappings()
        {
            var retval = new List<Tuple<string, string>>();

            var leftType = typeof(TLeft);
            var rightType = typeof(TRight);

            foreach (var mapping in propertyMaps)
            {
                retval.Add(Tuple.Create(string.Format("{0}.{1}", leftType.Name, mapping.Item1.Name), string.Format("{0}.{1}", rightType.Name, mapping.Item2.Name)));
            }

            foreach (var mapping in propertyExpressionMaps)
            {
                var leftItem = ((MemberExpression)mapping.Item1);
                var rightItem = ((MemberExpression)mapping.Item2);

                var leftParam = Expression.Parameter(typeof(TLeft), "v");
                var rightParam = Expression.Parameter(typeof(TRight), "v");

                leftItem = ParameterExpressionModifier.ReplaceAllParameterExpression(leftItem, leftParam);
                rightItem = ParameterExpressionModifier.ReplaceAllParameterExpression(rightItem, rightParam);

                var leftItemString = leftItem.ToString().Substring(2);
                var rightItemString = rightItem.ToString().Substring(2);

                retval.Add(Tuple.Create(string.Format("{0}.{1}", leftType.Name, leftItemString), string.Format("{0}.{1}", rightType.Name, rightItemString)));
            }

            return retval;
        }

        internal static string MappingsView()
        {
            var leftType = typeof(TLeft);
            var rightType = typeof(TRight);

            var retval = new StringBuilder();
            retval.AppendFormat("fFastMap from type {0} to type {1}", leftType.FullName, rightType.FullName);
            retval.AppendLine();

            foreach (var mapping in Mappings())
            {
                retval.Append(string.Format("Property {0} maps to property {1}", mapping.Item1, mapping.Item2));
                retval.AppendLine("");
            }

            return retval.ToString();
        }

        internal static void DeletePropertyMapper<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, bool callReverse)
        {
            var leftPropertyMemberExpression = leftProperty.Body as MemberExpression;
            if (leftPropertyMemberExpression == null)
            {
                throw new ArgumentException("Must contain a MemberExpression", "leftProperty");
            }


            if (leftPropertyMemberExpression.Expression is ParameterExpression)
            {
                var matchingMaps = propertyMaps.Where(v => v.Item1 == (PropertyInfo)leftPropertyMemberExpression.Member).ToList();

                foreach (var map in matchingMaps)
                {
                    var rightProperty = map.Item2;
                    if (callReverse && IsMappingBidirectional)
                    {
                        var rightParameterExpression = Expression.Parameter(typeof(TRight));
                        var rightPropertyExpression = Expression.Property(rightParameterExpression, rightProperty);
                        var rightPropertyLambdaExpression = Expression.Lambda<Func<TRight, TPropertyType>>(rightPropertyExpression, rightParameterExpression);
                        fFastMapperInternal<TRight, TLeft>.DeletePropertyMapper<TPropertyType>(rightPropertyLambdaExpression, fFastMap.CallReverseFalse);
                    }

                    propertyMaps.Remove(map);
                }
            }
            else
            {
                var matchingMaps = propertyExpressionMaps.Where(v => MemberExpressionEquivalent(leftProperty, v.Item1)).ToList();

                foreach (var map in matchingMaps)
                {
                    var rightProperty = map.Item2;
                    if (callReverse && IsMappingBidirectional)
                    {
                        var rightParameterExpression = Expression.Parameter(typeof(TRight));
                        var rightPropertyLambdaExpression = Expression.Lambda<Func<TRight, TPropertyType>>(rightProperty, rightParameterExpression);
                        fFastMapperInternal<TRight, TLeft>.DeletePropertyMapper<TPropertyType>(rightPropertyLambdaExpression, fFastMap.CallReverseFalse);
                    }

                    propertyExpressionMaps.Remove(map);
                }
            }

            CompileMapper();
        }

        static bool MemberExpressionEquivalent<TPropertyType>(Expression<Func<TLeft, TPropertyType>> leftProperty, Expression rightExpression)
        {
            var left = (MemberExpression)leftProperty.Body;
            var right = (MemberExpression)rightExpression;

            while (true)
            {
                if (left.Member != right.Member)
                {
                    return false;
                }

                if (left.Expression is ParameterExpression && right.Expression is ParameterExpression && ((ParameterExpression)left.Expression).Type == ((ParameterExpression)right.Expression).Type)
                {
                    return true;
                }

                left = (MemberExpression)left.Expression;
                right = (MemberExpression)right.Expression;
            }
        }
    }
}
