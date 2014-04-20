//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;

//namespace Grax.fFastMapper
//{
//    internal static class PropertyList<T>
//    {
//        static IDictionary<string, MemberExpression> properties = new Dictionary<string, MemberExpression>();
//        static IDictionary<string, string> matchingDictionary = new Dictionary<string, string>();
//        static Type baseType = typeof(T);

//        static PropertyList()
//        {
//            InitializeProperties();
//        }

//        private static void InitializeProperties()
//        {
//            // orderByDescending so that longer-named properties at a low level take priority over shorter-named properties
//            var foundProperties = FindAllProperties(baseType, "", Expression.Parameter(baseType), 0).OrderBy(v => v.Item1);

//            foreach (var property in foundProperties)
//            {
//                var key = property.Item1;
//                var matchingKey = key.Replace(".", string.Empty);

//                if (matchingDictionary.ContainsKey(matchingKey) == false)
//                {
//                    properties.Add(key, property.Item2);
//                    matchingDictionary.Add(matchingKey, key);
//                }
//            }
//        }

//        static IEnumerable<Tuple<string, MemberExpression>> FindAllProperties(Type type, string prefix, Expression baseExpression, int depth)
//        {
//            if (depth > 15)
//            {
//                yield break;
//            }

//            prefix = prefix + ".";

//            foreach (var property in type.GetProperties())
//            {
//                if (property.PropertyType.IsTypeMatchable())
//                {
//                    yield return Tuple.Create(prefix + property.Name, ToMemberExpression(baseExpression, property));
//                }
//                else
//                {
//                    foreach (var returnValue in FindAllProperties(property.PropertyType, prefix + property.Name, ToMemberExpression(baseExpression, property), depth + 1))
//                    {
//                        yield return returnValue;
//                    }
//                }
//            }
//        }

//        static MemberExpression ToMemberExpression(Expression parentExpression, PropertyInfo property)
//        {
//            return Expression.Property(parentExpression, property);
//        }

//        public static IDictionary<string, MemberExpression> Properties { get { return properties; } }
//        public static IDictionary<string, string> MatchingKeys { get { return matchingDictionary; } }

//    }
//}
