using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace CompetitiveComparisonTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class NamedPropertyRecursionPoc
    {


        //void FindMatches(Expression leftExpression, Expression rightExpression, IList<Tuple<Expression, Expression>> matches)
        //{
        //    var leftType = GetTypeFromExpression(leftExpression);
        //    var rightType = GetTypeFromExpression(rightExpression);

        //    foreach (var leftProperty in leftType.GetProperties())
        //    {
        //        var exactMatchRightProperty = rightType.GetProperty(leftProperty.Name);

        //        if (exactMatchRightProperty != null)
        //        {
        //            matches.Add(Tuple.Create(leftExpression, rightExpression));
        //        }

        //        //foreach (var rightProperty in rightType.GetProperties().Where(v => v.Name.StartsWith(leftProperty.Name)))
        //        //{
        //        //    FindMatches(
        //        //}

        //        //foreach (var rightProperty in rightType.GetProperties().Where(v => leftProperty.Name.StartsWith(v.Name)))
        //        //{
        //        //}
        //    }
        //}

        //IEnumerable<MemberExpression> DeepGetAllProperties()
        //{
        //}

        Type GetTypeFromExpression(Expression expression)
        {
            var propertyExpression = expression as MemberExpression;

            if (propertyExpression == null)
            {
                var parameterExpression = expression as ParameterExpression;

                if (parameterExpression == null)
                {
                    throw new ArgumentException("Expression must be either a MemberExpression or a ParameterExpression", "expression");
                }

                return parameterExpression.Type;
            }

            return ((PropertyInfo)propertyExpression.Member).PropertyType;
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    //var leftPropertyName = "SubParentSubSubSubMyString";
        //    //Debug.Print(MatchPath(leftPropertyName, typeof(Right), null, Expression.Parameter(typeof(Right))).ToString());

        //    var leftParam = Expression.Parameter(typeof(Left));
        //    var rightParam = Expression.Parameter(typeof(Right));

        //    var leftProp = typeof(Left).GetProperty("SubParentSubSubSubMyString");
        //    var rightProp = typeof(Right).GetProperty("SubParent");

        //    string leftName = null;
        //    string rightName = null;

        //    var leftProperties = new List<PropertyInfo> { leftProp };
        //    var rightProperties = new List<PropertyInfo> { rightProp };

        //    bool doneMatch = false;

        //    while (!doneMatch)
        //    {
        //        leftName = GetNameFromPropertyInfoList(leftProperties);
        //        rightName = GetNameFromPropertyInfoList(rightProperties);

        //        if (leftName.Length == rightName.Length)
        //        {
        //            break;
        //        }

        //        if (leftName.Length > rightName.Length)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }

        //    Debug.Print("{0} == {1}", leftName, rightName);
        //}


        //public static string GetNameFromPropertyInfoList(IList<PropertyInfo> propertyInfos, bool addPeriods = false)
        //{
        //    var retval = new StringBuilder();
        //    var divider = (addPeriods ? "." : string.Empty);

        //    foreach (var propInfo in propertyInfos)
        //    {
        //        retval.Append(propInfo + divider);
        //    }

        //    return retval.ToString().TrimEnd('.');
        //}

        //public ExpressionMatch MatchPath(string leftPropertyName, Type baseRightType, PropertyInfo propInfo, Expression returnExpression)
        //{
        //    var thisType = baseRightType;

        //    if (propInfo != null)
        //    {
        //        thisType = propInfo.PropertyType;
        //    }

        //    foreach (var propertyInfo in thisType.GetProperties().Where(v => leftPropertyName.StartsWith(v.Name)))
        //    {
        //        if (propertyInfo.Name == leftPropertyName)
        //        {
        //            return new ExpressionMatch { Expression = Expression.Property(returnExpression, propertyInfo) };
        //        }

        //        return MatchPath(
        //            leftPropertyName.Substring(propertyInfo.Name.Length),
        //            null,
        //            propertyInfo,
        //            Expression.Property(returnExpression, propertyInfo));
        //    }

        //    return null;
        //}

        class Left
        {
            public string SubParentSubSubSubMyString { get; set; }
        }

        class Right
        {
            public SubType SubParent { get; set; }
        }

        class SubType
        {
            public SubType Sub { get; set; }

            public string MyString { get; set; }
        }
    }
}
