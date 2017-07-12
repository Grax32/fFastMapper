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

        internal static int MaxRecursionLevel = 8;
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

            var left = new fFastMapperGlobal.TypeMatchData
            {
                Expression = Expression.Parameter(leftType),
                Prefix = "",
                Type = leftType
            };

            var right = new fFastMapperGlobal.TypeMatchData
            {
                Expression = Expression.Parameter(rightType),
                Prefix = "",
                Type = rightType
            };

            RecursiveMapAnalyze(left, right, 0);

            if (callReverse && IsMappingBidirectional)
            {
                fFastMapperInternal<TRight, TLeft>.AddDefaultMappings(fFastMap.CallReverseFalse, quitSilentlyIfMappingStarted);
            }

            var itemsToRemove = new List<Tuple<Expression, Expression>>();

            // remove redundant mappings
            foreach (var groupItem in propertyExpressionMaps
                .GroupBy(v => v.Item2.ToString(), (key, g) => new { Count = g.Count(), Grouping = g })
                .Where(v => v.Count > 1))
            {
                var keepItem = groupItem.Grouping
                    .OrderBy(item => MemberExpressionDepth(item.Item1))
                    .ThenBy(item => MemberExpressionDepth(item.Item2))
                    .First();

                foreach (var item in groupItem.Grouping.Where(v => !v.Equals(keepItem)))
                {
                    itemsToRemove.Add(item);
                }
            }

            itemsToRemove.ForEach(v => propertyExpressionMaps.Remove(v));

            CompileMapper();
        }

        internal static int MemberExpressionDepth(Expression expression)
        {
            if (expression is ParameterExpression)
            {
                return 0;
            }

            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;
                return 1 + MemberExpressionDepth(memberExpression.Expression);
            }

            throw new ArgumentException("This method can only be used with MemberExpression and ParameterExpression", "expression");
        }


        internal static bool NameMatchEquals(string leftName, string rightName)
        {
            var workingLeftName = leftName.Replace(".", "");
            var workingRightName = rightName.Replace(".", "");
            return workingLeftName.Equals(workingRightName);
        }

        /// <summary>
        /// Return true if leftName starts with rightName and left name is not equal to right name
        /// </summary>
        /// <param name="leftName"></param>
        /// <param name="startsWithName"></param>
        /// <returns></returns>
        internal static bool NameMatchStartsWith(string compareName, string startsWithName)
        {
            var workingLeftName = compareName.Replace(".", "");
            var workingRightName = startsWithName.Replace(".", "");
            return workingLeftName != workingRightName && workingLeftName.StartsWith(workingRightName);
        }

        static string NoMatchPrefix = Guid.NewGuid().ToString();

        internal static void RecursiveMapAnalyze(fFastMapperGlobal.TypeMatchData left, fFastMapperGlobal.TypeMatchData right, int recursionLevel)
        {
            Debug.Print("L" + recursionLevel + ": Left: " + left.Prefix + " --- Right: " + right.Prefix);

            if (recursionLevel > MaxRecursionLevel)
            {
                return;
            }

            var leftPrefix = left.Prefix;
            var rightPrefix = right.Prefix;

            if (recursionLevel > 0 && leftPrefix == rightPrefix) return;

            var leftProperties = left.Type.GetProperties().Where(v => v.CanRead).ToList();
            var rightProperties = right.Type.GetProperties().Where(v => v.CanWrite).ToList();

            foreach (var leftProperty in leftProperties)
            {
                var leftPropertyName = leftProperty.Name;
                var leftPropertyType = leftProperty.PropertyType;
                
                var hasMatches = rightProperties
                  .Any(rightProperty => NameMatchStartsWith(rightPrefix + rightProperty.Name, leftPrefix + leftPropertyName));

                if (hasMatches)
                {
                    var leftParm = new fFastMapperGlobal.TypeMatchData
                    {
                        Expression = Expression.Property(left.Expression, leftProperty),
                        Type = leftProperty.PropertyType,
                        Prefix = leftPrefix + "." + leftPropertyName
                    };

                    var rightParm = new fFastMapperGlobal.TypeMatchData(right);

                    Debug.Print("L" + recursionLevel + ":" + "LeftMatch: Left: " + leftPrefix + " Right: " + rightPrefix);// + " Property: " + matchingProperty.Name);
                    RecursiveMapAnalyze(leftParm, rightParm, recursionLevel + 1);
                }

                var rightMatches = rightProperties
                    .Where(rightProperty => NameMatchStartsWith(leftPrefix + leftPropertyName, rightPrefix + rightProperty.Name));

                foreach (var matchingProperty in rightMatches)
                {
                    var leftParm = new fFastMapperGlobal.TypeMatchData(left);
                    var rightParm = new fFastMapperGlobal.TypeMatchData
                    {
                        Expression = Expression.Property(right.Expression, matchingProperty),
                        Type = matchingProperty.PropertyType,
                        Prefix = rightPrefix + "." + matchingProperty.Name
                    };

                    Debug.Print("L" + recursionLevel + ":" + "RightMatch: Left: " + leftPrefix + " Right: " + rightPrefix + " Property: " + matchingProperty.Name);
                    RecursiveMapAnalyze(leftParm, rightParm, recursionLevel + 1);
                }

                // direct match
                var foundRightProperty = rightProperties
                    .WhereAssignableFrom(leftPropertyType)
                    .Where(rightProperty =>
                        NameMatchEquals(rightPrefix + rightProperty.Name, leftPrefix + leftPropertyName))
                        .SingleOrDefault();

                if (foundRightProperty != null) // found match
                {
                    Expression leftExpression = Expression.Property(left.Expression, leftProperty);
                    Expression rightExpression = Expression.Property(right.Expression, foundRightProperty);

                    Debug.Print("L" + recursionLevel + ":" + "Found " + leftExpression.ToString() + " == " + rightExpression.ToString());

                    propertyExpressionMaps.Add(Tuple.Create(leftExpression, rightExpression));
                }
            }
            Debug.Print("L" + recursionLevel + ":" + " Exiting Level ");
        }

        internal static void CompileMapper()
        {
            var leftParam = Expression.Parameter(typeof(TLeft), "left");
            var rightParam = Expression.Parameter(typeof(TRight), "right");
            var parameters = new ParameterExpression[] { leftParam, rightParam };
            var expressions = new List<Expression>();

            expressions.Add(NullCheckExpression(leftParam));
            expressions.Add(NullCheckExpression(rightParam));

            //foreach (var propertyMap in propertyMaps)
            //{
            //    var leftPropertyExpression = Expression.Property(leftParam, propertyMap.Item1);
            //    var rightPropertyExpression = Expression.Property(rightParam, propertyMap.Item2);
            //    var assignmentExpression = Expression.Assign(rightPropertyExpression, leftPropertyExpression);
            //    expressions.Add(assignmentExpression);
            //}

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

        internal static Func<TLeft, TRight, TRight> mapperFunc = (left, right) => { throw new fFastMap.fFastMapException(string.Format("Mapping has not been configured from {0} to {1}", typeof(TLeft).Name, typeof(TRight).Name)); };

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

            // using .ToList to avoid CollectionModified errors
            if (leftPropertyMemberExpression.Expression is ParameterExpression)
            {
                foreach (var map in propertyMaps
                    .Where(v => v.Item1 == (PropertyInfo)leftPropertyMemberExpression.Member)
                    .ToList()
                    )
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

            // using .ToList to avoid CollectionModified errors
            foreach (var map in
                propertyExpressionMaps
                .Where(v => MemberExpressionEquivalent(leftProperty, v.Item1))
                .ToList()
                )
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
