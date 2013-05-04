using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Grax.fFastMapper
{
    class ParameterExpressionModifier : ExpressionVisitor
    {
        public static Expression ReplaceParameterExpression(Expression expression, ParameterExpression oldParameterExpression, ParameterExpression newParameterExpression)
        {
            return (new ParameterExpressionModifier()).Modify(expression, oldParameterExpression, newParameterExpression);
        }

        public static TExpression ReplaceAllParameterExpression<TExpression>(TExpression expression, ParameterExpression newParameterExpression)
            where TExpression : Expression
        {
            return (TExpression)(new ParameterExpressionModifier()).Modify(expression, newParameterExpression);
        }

        private ParameterExpressionModifier() { }

        ParameterExpression _oldParameterExpression;
        ParameterExpression _newParameterExpression;
        bool _replaceAllParameterExpression = false;

        /// <summary>
        /// Modifiy Expression by replacing old parameter expression with new parameter expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Expression Visit(Expression node)
        {
            if (_replaceAllParameterExpression == false && _oldParameterExpression == null) { throw new fFastMap.fFastMapException("OldParameterExpression must not be null.  Do not call visit directly"); }
            if (_newParameterExpression == null) { throw new fFastMap.fFastMapException("NewParameterExpression must not be null.  Do not call visit directly"); }

            return base.Visit(node);
        }

        public Expression Modify(Expression expression, ParameterExpression newParameterExpression)
        {
            _replaceAllParameterExpression = true;
            _newParameterExpression = newParameterExpression;

            return Visit(expression);
        }

        public Expression Modify(Expression expression, ParameterExpression oldParameterExpression, ParameterExpression newParameterExpression)
        {
            _replaceAllParameterExpression = false;
            _oldParameterExpression = oldParameterExpression;
            _newParameterExpression = newParameterExpression;

            return Visit(expression);
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_replaceAllParameterExpression)
            {
                return _newParameterExpression;
            }
            else
            {
                if (node.Equals(_oldParameterExpression))
                {
                    return _newParameterExpression;
                }
            }

            return base.VisitParameter(node);
        }
    }
}
