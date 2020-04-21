using FluentAssertions;
using System.Linq.Expressions;
using Xunit;

namespace Fraso.Hal.Conversions.Tests.TypedParameterExpressionFinderTests
{
    public class TypedParameterExpressionFinder_OfTypeMethod
    {
        [Fact]
        public void GivenExpressionWithoutParameters_ThenReturnsEmptyCollection()
        {
            // Arrange
            var expression =
                Expression.Equal(
                    Expression.Constant(2),
                    Expression.Add(
                        Expression.Constant(1),
                        Expression.Constant(1)));

            // Act
            var parameters =
                TypedParameterExpressionFinder
                .OfType<int>(expression);

            // Assert
            parameters
                .Should()
                .NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GivenExpressionWithMultipleNonMatchingParameters_ThenReturnsEmptyCollection()
        {
            // Arrange
            var param1 =
                Expression.Parameter(typeof(double), "number1");

            var param2 =
                Expression.Parameter(typeof(double), "number2");

            var expression =
                Expression.Equal(
                    Expression.Constant(2D),
                    Expression.Add(
                        param1,
                        param2));

            // Act
            var parameters =
                TypedParameterExpressionFinder
                .OfType<int>(expression);

            // Assert
            parameters
                .Should()
                .NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GivenExpressionWithMultipleMatchingParameters_ThenReturnsThose()
        {
            // Arrange
            var param1 =
                Expression.Parameter(typeof(int), "number1");

            var param2 =
                Expression.Parameter(typeof(int), "number2");

            var expression =
                Expression.Equal(
                    Expression.Constant(2),
                    Expression.Add(
                        param1,
                        param2));

            // Act
            var parameters =
                TypedParameterExpressionFinder
                .OfType<int>(expression);

            // Assert
            parameters
                .Should()
                .BeEquivalentTo(param1, param2);
        }
    }
}
