using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Fraso.Hal.Conversions
{
    public sealed class TypedParameterExpressionFinder
        : ExpressionVisitor
    {
        #region Properties
        public Type ParameterType { get; }

        public List<ParameterExpression> FoundParameters { get; }
            = new List<ParameterExpression>();
        #endregion // Properties

        #region Ctors
        private TypedParameterExpressionFinder(Type parameterType)
        {
            ParameterType =
                parameterType
                    ?? throw new ArgumentNullException(nameof(parameterType));
        }

        #endregion //Ctors

        public static IEnumerable<ParameterExpression> OfType<T>(Expression expression)
        {
            var finder =
                new TypedParameterExpressionFinder(typeof(T));

            _ = finder.Visit(expression);

            return 
                finder.FoundParameters;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == ParameterType)
                FoundParameters.Add(node);

            return 
                node;
        }
    }
}
