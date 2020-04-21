using System;
using System.Linq.Expressions;
using System.Text;

namespace Fraso.Hal.Conversions
{
    /// <summary>
    /// Helper class used to find member expression names of given type <typeparamref name="T"/>.
    /// </summary>
    internal class NameResolver
        : ExpressionVisitor
    {
        private readonly StringBuilder NameBuilder =
            new StringBuilder();

        private NameResolver() { }

        protected override Expression VisitMember(MemberExpression node)
        {
            _ = 
                base.Visit(node.Expression);

            var memberInfo =
                node.Member;

            NameBuilder
                .Append(memberInfo.Name);

            return
                node;
        }

        public static string ResolveFrom(LambdaExpression lambda)
        {
            var visitor =
                new NameResolver();

            visitor.Visit(lambda);

            return
                visitor
                    .NameBuilder
                    .ToString()
                        ?? throw new InvalidOperationException("Unable to infer the property name from given expression.");
        }
    }
}
