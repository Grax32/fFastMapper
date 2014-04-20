using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Grax.fFastMapper
{
    class ParameterExpressionModifier : ExpressionVisitor
    {
        public static TExpression ReplaceAllParameterExpression<TExpression>(TExpression expression, ParameterExpression newParameterExpression)
            where TExpression : Expression
        {
            if (expression == null)
            {
                throw new fFastMap.fFastMapException("expression must not be null.");
            }

            if (newParameterExpression == null)
            {
                throw new fFastMap.fFastMapException("newParameterExpression must not be null.");
            }

            return (TExpression)(new ParameterExpressionModifier()).Modify(expression, newParameterExpression);
        }

        private ParameterExpressionModifier() { }

        ParameterExpression _newParameterExpression;

        public Expression Modify(Expression expression, ParameterExpression newParameterExpression)
        {
            _newParameterExpression = newParameterExpression;
            return Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _newParameterExpression;
        }
    }
}
