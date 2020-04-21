using FluentAssertions;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Fraso.Hal.Conversions.Tests.WrapPolicyTests
{
    public class WrapPolicy_ImplicitConversion
    {
        [Fact]
        public void GivenWrapPolicyCoupling_ThenAddsRuleToCurrentPolicyAndReturnsIt()
        {
            // Arrange
            Expression<Func<ToWrap, object>> textGetter =
                i => i.Text;

            Expression<Func<ToWrap, object>> numberGetter =
                i => i.Number;

            var coupling =
                WrapPolicy
                    .For<ToWrap>()
                    .Property(textGetter)
                    .Property(numberGetter);

            // Act
            WrapPolicy<ToWrap> policy =
                coupling;

            // Asset
            policy
                .WrappingRules
                .Select(r => r.Getter)
                .Should()
                .BeEquivalentTo(
                    textGetter,
                    numberGetter);
        }
    }
}
