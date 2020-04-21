using FluentAssertions;
using Xunit;

namespace Fraso.Hal.Conversions.Tests.WrapPolicyTests
{
    public class WrapPolicy_ForMethod
    {
        [Fact]
        public void ReturnsEmptyPolicy()
        {
            // Act
            var policy =
                WrapPolicy
                .For<ToWrap>();

            // Assert
            policy
                .Should()
                .NotBeNull();

            policy
                .UseCamelCase
                .Should()
                .BeFalse();

            policy
                .LinkingRules
                .Should()
                .BeEmpty();

            policy
                .WrappingRules
                .Should()
                .BeEmpty();
        }
    }
}
