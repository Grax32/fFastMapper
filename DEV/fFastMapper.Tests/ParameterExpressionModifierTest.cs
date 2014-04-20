using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Grax.fFastMapper;

namespace fFastMapper.Tests
{
    [TestClass]
    public class ParameterExpressionModifierTest
    {
        [TestMethod]
        public void ParameterExpressionModifierUsageTest()
        {
            Expression<Func<TestClass, string>> expression = v => v.TestProp.TestProp.TestProp.SomeString;
            var parmExpression = Expression.Parameter(typeof(TestClass), "TC");

            var result = ParameterExpressionModifier.ReplaceAllParameterExpression(expression, parmExpression);

            var test = new TestClass();
            test.TestProp = test;
            test.SomeString = "FOUND";

            var originalFunc = expression.Compile();
            var resultFunc = result.Compile();

            Assert.AreEqual("FOUND", originalFunc.Invoke(test));
            Assert.AreEqual("FOUND", resultFunc.Invoke(test));

            Expression loopExpression = result.Body;
            while (!(loopExpression is ParameterExpression))
            {
                loopExpression = ((MemberExpression)loopExpression).Expression;
            }

            Assert.AreEqual(parmExpression, loopExpression);
        }

        [TestMethod]
        public void ParameterExpressionModifierExceptionTest()
        {
            Exception exception = null;
            try
            {
                ParameterExpressionModifier.ReplaceAllParameterExpression<Expression<Func<TestClass, string>>>(null, null);
            }
            catch (fFastMap.fFastMapException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            var errMessage = exception.Message;

            Expression<Func<TestClass, string>> expression = v => v.TestProp.TestProp.TestProp.SomeString;
            try
            {
                ParameterExpressionModifier.ReplaceAllParameterExpression<Expression<Func<TestClass, string>>>(expression, null);
            }
            catch (fFastMap.fFastMapException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.AreNotEqual(errMessage, exception.Message);
        }

        class TestClass
        {
            public TestClass TestProp { get; set; }
            public string SomeString { get; set; }
        }
    }
}
