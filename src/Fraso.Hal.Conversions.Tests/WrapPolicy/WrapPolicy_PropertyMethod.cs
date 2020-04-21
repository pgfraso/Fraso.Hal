using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Fraso.Hal.Conversions.Tests.WrapPolicyTests
{
    public class WrapPolicy_PropertyMethod
    {
        [Fact]
        public void GivenPropertyGetter_ReturnsWrapRuleWithoutName()
        {
            // Arrange
            Expression<Func<ToWrap, object>> getter =
                i => i.Text;

            // Act
            WrapPolicy<ToWrap> policy =
                WrapPolicy
                .For<ToWrap>()
                .Property(getter);

            // Assert
            policy
                .WrappingRules
                .Should()
                .ContainSingle(r =>
                    r.Name == null && r.Getter == getter);
        }

        [Fact]
        public void GivenPropertyGetterAndExplictName_ReturnsWrapRuleWithName()
        {
            var name =
                "name";

            Expression<Func<ToWrap, object>> getter =
                i => i.Text;

            // Act
            WrapPolicy<ToWrap> policy =
                WrapPolicy
                .For<ToWrap>()
                .Property(getter)
                    .As(name);

            // Assert
            policy
                .WrappingRules
                .Should()
                .ContainSingle(r =>
                    r.Name == name && r.Getter == getter);
        }
    }
}
