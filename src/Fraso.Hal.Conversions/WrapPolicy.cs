using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fraso.Hal.Conversions
{
    /// <summary>
    /// Defines policy used to wrap instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type for which wrap policy is defined.</typeparam>
    public sealed class WrapPolicy<T>
        : IEnumerable<PropertyWrap<T>>
    {
        #region Fields
        public readonly bool UseCamelCase;

        public readonly LinkingRule<T>[] LinkingRules;

        public readonly PropertyWrap<T>[] WrappingRules;
        #endregion //Fields

        #region Ctors
        internal WrapPolicy()
            : this(false, null, null) { }

        internal WrapPolicy(
            bool useCamelCase,
            LinkingRule<T>[] linkingRules = null,
            PropertyWrap<T>[] wrappingRules = null)
        {
            UseCamelCase = 
                useCamelCase;

            LinkingRules = 
                linkingRules 
                    ?? new LinkingRule<T>[0];

            WrappingRules = 
                wrappingRules 
                    ?? new PropertyWrap<T>[0];

        }
        #endregion // Ctors

        #region IEnmuerable
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerator<PropertyWrap<T>> GetEnumerator()
            => WrappingRules
                .Cast<PropertyWrap<T>>()
                .GetEnumerator();
        #endregion //IEnumerable

        public static implicit operator WrapPolicy<T>(PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling)
        {
            var policy =
                ruleCoupling.Policy;

            var linkingRules =
                policy
                    .LinkingRules
                    .AddToTail(ruleCoupling.Rule);

            return
                new WrapPolicy<T>(
                    policy.UseCamelCase,
                    linkingRules,
                    policy.WrappingRules);
        }

        public static implicit operator WrapPolicy<T>(PolicyRuleCoupling<T, PropertyWrap<T>> ruleCoupling)
        {
            var policy =
                ruleCoupling.Policy;

            var wrappingRules =
                policy
                    .WrappingRules
                    .AddToTail(ruleCoupling.Rule);

            return
                new WrapPolicy<T>(
                    policy.UseCamelCase,
                    policy.LinkingRules,
                    wrappingRules);
        }
    }

    public static class WrapPolicy
    {
        /// <summary>
        /// Creates empty wrap policy for <typeparamref name="T"/>.
        /// </summary>
        public static WrapPolicy<T> For<T>()
            => new WrapPolicy<T>();

        /// <summary>
        /// Returns new <see cref="WrapPolicy{T}"/> that has <see cref="WrapPolicy{T}.UseCamelCase"/> flag set.
        /// </summary>
        public static WrapPolicy<T> UseCamelCase<T>(this WrapPolicy<T> policy)
            => new WrapPolicy<T>(
                useCamelCase: true,
                linkingRules: policy.LinkingRules,
                wrappingRules: policy.WrappingRules);

        public static PolicyRuleCoupling<T, LinkingRule<T>> Link<T, TController>(
                this WrapPolicy<T> policy,
                Expression<Action<T, TController>> controllerAction)
            => PolicyRuleCoupling.From(
                policy,
                LinkingRule.FromMethod(controllerAction));

        /// <summary>
        /// Creates new <see cref="PolicyRuleCoupling{TWrappedObject, TRule}"/> from <paramref name="policy"/> policy and new <see cref="LinkingRule{T}"/> rule based on the <paramref name="controllerAction"/>
        /// </summary>
        public static PolicyRuleCoupling<T, LinkingRule<T>> Link<T, TController>(
                this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
                Expression<Action<T, TController>> controllerAction)
        {
            var policy =
                ruleCoupling.Policy;

            var linkingRules =
                policy
                    .LinkingRules
                    .AddToTail(ruleCoupling.Rule);

            var wrapPolicy =
                new WrapPolicy<T>(
                    policy.UseCamelCase,
                    linkingRules,
                    policy.WrappingRules);

            return
                PolicyRuleCoupling.From(
                    wrapPolicy,
                    LinkingRule.FromMethod(controllerAction));
        }

        /// <summary>
        /// Adds query parameters to current <see cref="LinkingRule{T}"/>.
        /// </summary>
        public static PolicyRuleCoupling<T, LinkingRule<T>> WithParameters<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            IDictionary<string, object> queryParameters)
            => ruleCoupling
                .WithParameters(
                    queryParameters
                        .Select(kvp => (kvp.Key, kvp.Value))
                        .ToArray());

        /// <summary>
        /// Adds query parameters to current <see cref="LinkingRule{T}"/>.
        /// </summary>
        public static PolicyRuleCoupling<T, LinkingRule<T>> WithParameters<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            params (string param, object value)[] queryParameters)
            => new PolicyRuleCoupling<T, LinkingRule<T>>(
                ruleCoupling.Policy,
                    new LinkingRule<T>(
                        ruleCoupling.Rule.Name,
                        ruleCoupling.Rule.ControllerName,
                        ruleCoupling.Rule.MethodCall,
                        queryParameters,
                        ruleCoupling.Rule.GetQueryParametersCallback,
                        ruleCoupling.Rule.AddPredicate));

        public static PolicyRuleCoupling<T, LinkingRule<T>> WithParameters<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            Func<T, IEnumerable<(string, object)>> getQueryParametersCallback)
            => new PolicyRuleCoupling<T, LinkingRule<T>>(
                ruleCoupling.Policy,
                    new LinkingRule<T>(
                        ruleCoupling.Rule.Name,
                        ruleCoupling.Rule.ControllerName,
                        ruleCoupling.Rule.MethodCall,
                        ruleCoupling.Rule.QueryParameters,
                        getQueryParametersCallback,
                        ruleCoupling.Rule.AddPredicate));

        public static PolicyRuleCoupling<T, LinkingRule<T>> When<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            Func<T, bool> predicate)
            => new PolicyRuleCoupling<T, LinkingRule<T>>(
                ruleCoupling.Policy,
                    new LinkingRule<T>(
                        ruleCoupling.Rule.Name,
                        ruleCoupling.Rule.ControllerName,
                        ruleCoupling.Rule.MethodCall,
                        ruleCoupling.Rule.QueryParameters,
                        ruleCoupling.Rule.GetQueryParametersCallback,
                        predicate));

        /// <summary>
        /// Adds link name to current <see cref="LinkingRule{T}"/>.
        /// </summary>
        public static PolicyRuleCoupling<T, LinkingRule<T>> As<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            string linkName)
            => new PolicyRuleCoupling<T, LinkingRule<T>>(
                ruleCoupling.Policy,
                new LinkingRule<T>(
                    linkName,
                    ruleCoupling.Rule.ControllerName,
                    ruleCoupling.Rule.MethodCall,
                    ruleCoupling.Rule.QueryParameters,
                    ruleCoupling.Rule.GetQueryParametersCallback,
                    ruleCoupling.Rule.AddPredicate));

        /// <summary>
        /// Creates new <see cref="PolicyRuleCoupling{TWrappedObject, TRule}"/> from <paramref name="policy"/> policy and new <see cref="PropertyWrap{T}"/> rule based on the <paramref name="getter"/>
        /// </summary>
        public static PolicyRuleCoupling<T, PropertyWrap<T>> Property<T>(
            this WrapPolicy<T> policy,
            Expression<Func<T, object>> getter)
            => PolicyRuleCoupling.From(
                policy,
                Wrap.Property(getter));

        /// <summary>
        /// Adds current <see cref="LinkingRule{T}"/> rule to policy, creates new <see cref="PropertyWrap{T}"/> rule based on supplied <paramref name="propertyGetter"/> and creates new <see cref="PolicyRuleCoupling{TWrappedObject, TRule}"/> using those
        /// </summary>
        public static PolicyRuleCoupling<T, PropertyWrap<T>> Property<T>(
            this PolicyRuleCoupling<T, LinkingRule<T>> ruleCoupling,
            Expression<Func<T, object>> propertyGetter)
        {
            var policy =
                ruleCoupling.Policy;

            var linkingRules =
                policy
                    .LinkingRules
                    .AddToTail(ruleCoupling.Rule);

            var wrapPolicy =
                new WrapPolicy<T>(
                    policy.UseCamelCase,
                    linkingRules,
                    policy.WrappingRules);

            return
                PolicyRuleCoupling.From(
                    wrapPolicy,
                    Wrap.Property(propertyGetter));
        }

        /// <summary>
        /// Adds current <see cref="PropertyWrap{T}"/> rule to policy, creates new <see cref="PropertyWrap{T}"/> rule based on supplied <paramref name="propertyGetter"/> and creates new <see cref="PolicyRuleCoupling{TWrappedObject, TRule}"/> using those
        /// </summary>
        public static PolicyRuleCoupling<T, PropertyWrap<T>> Property<T>(
            this PolicyRuleCoupling<T, PropertyWrap<T>> ruleCoupling,
            Expression<Func<T, object>> propertyGetter)
        {
            var policy =
                ruleCoupling.Policy;

            var wrappingRules =
                policy
                    .WrappingRules
                    .AddToTail(ruleCoupling.Rule);

            var wrapPolicy =
                new WrapPolicy<T>(
                    policy.UseCamelCase,
                    policy.LinkingRules,
                    wrappingRules);

            return
                PolicyRuleCoupling.From(
                    wrapPolicy,
                    Wrap.Property(propertyGetter));
        }

        /// <summary>
        /// Adds property name override to current <see cref="PropertyWrap{T}"/> rule.
        /// </summary>
        public static PolicyRuleCoupling<T, PropertyWrap<T>> As<T>(
            this PolicyRuleCoupling<T, PropertyWrap<T>> coupling,
            string propertyName)
            => new PolicyRuleCoupling<T, PropertyWrap<T>>(
                coupling.Policy,
                coupling.Rule.As(propertyName));

        /// <summary>
        /// Creates new <see cref="CollectionWrapPolicy{TCollection, TContent}"/> that can be used to wrap collections using current <see cref="WrapPolicy{TColllection}"/>
        /// into resource with its items nested using the <paramref name="contentPolicy"/>
        /// </summary>
        public static CollectionWrapPolicy<TCollection, TContent> EmbedContentUsing<TCollection, TContent>(
                this WrapPolicy<TCollection> collectionPolicy,
                WrapPolicy<TContent> contentPolicy)
            where TCollection : IEnumerable<TContent>
            => new CollectionWrapPolicy<TCollection, TContent>(
               collectionPolicy,
               contentPolicy);

        /// <summary>
        /// Creates new <see cref="CollectionWrapPolicy{TCollection, TContent}"/> that can be used to wrap collections using current <see cref="WrapPolicy{TColllection}"/>
        /// into resource with its items nested using the <paramref name="contentPolicy"/>
        /// </summary>
        public static CollectionWrapPolicy<TCollection, TContent> EmbedContentUsing<TCollection, TContent>(
                this PolicyRuleCoupling<TCollection, PropertyWrap<TCollection>> ruleCoupling,
                WrapPolicy<TContent> contentPolicy)
            where TCollection : IEnumerable<TContent>
            => new CollectionWrapPolicy<TCollection, TContent>(
                ruleCoupling,
                contentPolicy);

        /// <summary>
        /// Creates new <see cref="CollectionWrapPolicy{TCollection, TContent}"/> that can be used to wrap collections using current <see cref="WrapPolicy{TColllection}"/>
        /// into resource with its items nested using the <paramref name="contentPolicy"/>
        /// </summary>
        public static CollectionWrapPolicy<TCollection, TContent> EmbedContentUsing<TCollection, TContent>(
                this PolicyRuleCoupling<TCollection, LinkingRule<TCollection>> ruleCoupling,
                WrapPolicy<TContent> contentPolicy)
            where TCollection : IEnumerable<TContent>
            => new CollectionWrapPolicy<TCollection, TContent>(
                ruleCoupling,
                contentPolicy);
    }
}
