using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Fraso.Hal.Conversions.Tests.ResourceWrapper
{
    public class ResourceWrapper_WrapUsingMethod
    {
        #region Data
        private ToWrap WrappedObject { get; }
            = new ToWrap()
            {
                Text = "Fact",
                Number = 13,
                Nested =
                    new Bar()
                    {
                        Text = "NestedFact",
                        Number = 8
                    }
            };
        #endregion // Data

        [Fact]
        public void GivenPolicyWithLinkingWithoutUrlHelper_ReturnsResourceWithoutLinks()
        {
            // Arrange
            var policy =
                   WrapPolicy
                       .For<ToWrap>()
                       .Link((ToWrap obj, DummyController c) => c.GetNull());

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Links
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenPolicyWithLinkingPredicateReturningFalse_ReturnsResourceWithoutLinks()
        {
            // Arrange
            var policy =
                   WrapPolicy
                   .For<ToWrap>()
                   .Link((ToWrap obj, DummyController c) => c.GetNull())
                   .When(obj => false);

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Links
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenSingleObjectAndPolicyWithProperties_ThenReturnsResourceWithPropertiesAssigned()
        {
            // Arrange
            var policy =
                WrapPolicy
                    .For<ToWrap>()
                    .Property(i => i.Text)
                    .Property(i => i.Number);

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Should()
                .NotBeNull();

            resource
                .Properties
                .Should()
                .BeEquivalentTo(nameof(ToWrap.Text), nameof(ToWrap.Number));

            resource[nameof(ToWrap.Text)]
                .Should()
                .Be(WrappedObject.Text);

            resource[nameof(ToWrap.Number)]
                .Should()
                .Be(WrappedObject.Number);
        }

        [Fact]
        public void GivenSingleObjectAndPolicyWithNestedProperty_ThenReturnsResourceWith()
        {
            // Arrange
            var propertyName = nameof(ToWrap.Nested) + nameof(Bar.Text);

            var policy =
                WrapPolicy
                    .For<ToWrap>()
                    .Property(i => i.Nested.Text);

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Should()
                .NotBeNull();

            resource
                .Properties
                .Should()
                .BeEquivalentTo(propertyName);

            resource[propertyName]
                .Should()
                .Be(WrappedObject.Nested.Text);
        }

        [Fact]
        public void GivenSingleObjectAndUseCamelCasePolicy_ReturnsResourceWithCamelCasePropertyNames()
        {
            // Arrange
            var propertyName =
                nameof(ToWrap.Text)
                    .ToCamelCase();

            var policy =
                WrapPolicy
                    .For<ToWrap>()
                    .UseCamelCase()
                    .Property(i => i.Text);

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Should()
                .NotBeNull();

            resource
                .Properties
                .Should()
                .BeEquivalentTo(propertyName);

            resource[propertyName]
                .Should()
                .Be(WrappedObject.Text);
        }

        [Fact]
        public void GivenSingleObjectAndPolicyWithExplicitPropertyName_ThenReturnsResourceWithPropertyExplicitlyNamed()
        {
            // Arrange
            var newPropertyName =
                "name";

            var policy =
                WrapPolicy
                    .For<ToWrap>()
                    .UseCamelCase()
                    .Property(i => i.Text)
                        .As(newPropertyName);

            // Act
            var resource =
                WrappedObject.WrapUsing(policy);

            // Assert
            resource
                .Should()
                .NotBeNull();

            resource
                .Properties
                .Should()
                .BeEquivalentTo(newPropertyName);

            resource[newPropertyName]
                .Should()
                .Be(WrappedObject.Text);
        }

        [Fact]
        public void GivenObjectCollectionAndPolicyWithProperties_ThenReturnsResourceCollection()
        {
            // Arrange
            var wrapArray =
                new[]
                {
                    new ToWrap() { Text = "Foo"},
                    new ToWrap() { Text = "Bar"}
                };

            var policy =
                WrapPolicy
                    .For<ToWrap>()
                    .Property(i => i.Text);

            // Act
            var resources =
                wrapArray.WrapUsing(policy)
                .ToArray();

            // Assert
            resources
                .Should()
                .NotBeNull()
                .And
                .HaveCount(2);

            resources[0]
                .Properties
                .Should()
                .BeEquivalentTo(
                    resources[1].Properties,
                    because: "same policy should be used to create each resource");

            resources
                .Select(r => r[nameof(ToWrap.Text)])
                .Should()
                .BeEquivalentTo(
                    new[] { "Foo", "Bar" },
                    because: "each resources should have properties assigned based on the original item");
        }

        [Fact]
        public void GivenObjectCollectionAndPolicyWithContentWrap_ThenReturnsResourceWithCollectionItemsAsEmbedResources()
        {
            // Arrange
            var wrapArray =
                new[]
                {
                    new ToWrap() { Text = "Foo"},
                    new ToWrap() { Text = "Bar"}
                };

            var policy =
                WrapPolicy
                    .For<ToWrap[]>()
                    .EmbedContentUsing<ToWrap[], ToWrap>(
                        WrapPolicy
                            .For<ToWrap>()
                            .Property(i => i.Text));

            // Act
            var resource =
                wrapArray
                    .WrapUsing(policy);

            // Assert
            resource
                .Should()
                .NotBeNull();

            resource
                .Embedded
                .Should()
                .HaveCount(2);

            var resources =
                resource
                .Embedded
                .ToArray();

            resources[0]
                .Properties
                .Should()
                .BeEquivalentTo(
                    resources[1].Properties,
                    because: "same policy should be used to create each resource");

            resources
                .Select(r => r[nameof(ToWrap.Text)])
                .Should()
                .BeEquivalentTo(
                    new[] { "Foo", "Bar" },
                    because: "each resources should have properties assigned based on the original item");
        }
    }
}
