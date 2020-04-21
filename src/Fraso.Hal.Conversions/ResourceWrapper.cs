using Fraso.Hal.Primitives;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fraso.Hal.Conversions
{
    public static class ResourceWrapper
    {
        /// <summary>
        /// Creates <see cref="Resource"/> from object of type <typeparamref name="T"/> using supplied policy, ignoring any linking rules.
        /// </summary>
        public static Resource WrapUsing<T>(
            this T obj,
            WrapPolicy<T> policy)
            => obj.WrapUsing(
                policy,
                null);

        /// <summary>
        /// Creates <see cref="Resource"/> from object of type <typeparamref name="T"/> using supplied policy.
        /// </summary>
        public static Resource WrapUsing<T>(
            this T obj,
            WrapPolicy<T> policy,
            IUrlHelper urlHelper)
        {
            var resource =
                new Resource();

            ResolveLinks(
                obj,
                policy,
                resource,
                urlHelper);

            ResolveProperties(
                obj,
                policy,
                resource);

            return 
                resource;
        }

        /// <summary>
        /// Creates collection of <see cref="Resource"/> from collection of objects of type <typeparamref name="T"/> using supplied policy.
        /// </summary>
        public static IEnumerable<Resource> WrapUsing<T>(
            this IEnumerable<T> collection,
            WrapPolicy<T> policy)
            => collection.WrapUsing(
                policy,
                null);

        /// <summary>
        /// Creates collection of <see cref="Resource"/> from collection of objects of type <typeparamref name="T"/> using supplied policy.
        /// </summary>
        public static IEnumerable<Resource> WrapUsing<T>(
            this IEnumerable<T> collection,
            WrapPolicy<T> policy,
            IUrlHelper urlHelper)
        {
            var getters =
                policy
                .WrappingRules
                .ToDictionary(
                    r => ResolvePropertyName(r, policy.UseCamelCase),
                    r => r.Getter.Compile());

            return
                collection
                    .Select(item =>
                    {
                        var resource =
                            CreateResource(item);

                        ResolveLinks(
                            item,
                            policy,
                            resource,
                            urlHelper);

                        return
                            resource;
                    });

            Resource CreateResource(T item)
            {
                var resource = new Resource();

                foreach (var kvp in getters)
                    resource[kvp.Key] = kvp.Value(item);

                return resource;
            }
        }

        /// <summary>
        /// Creates <see cref="Resource"/> out of <paramref name="collection"/> embeding its content as a nested resources ignoring linking rules.
        /// </summary>
        public static Resource WrapUsing<TCollection, TContent>(
            this TCollection collection,
            CollectionWrapPolicy<TCollection, TContent> policy)
            where TCollection : IEnumerable<TContent>
            => collection.WrapUsing(policy, null);

        /// <summary>
        /// Creates <see cref="Resource"/> out of <paramref name="collection"/> embeding its content as a nested resources.
        /// </summary>
        public static Resource WrapUsing<TCollection, TContent>(
            this TCollection collection,
            CollectionWrapPolicy<TCollection, TContent> policy,
            IUrlHelper urlHelper)
            where TCollection : IEnumerable<TContent>
        {
            var resource =
                collection
                    .WrapUsing(
                        policy.CollectionPolicy,
                        urlHelper);

            var nestedResources =
                collection
                    .WrapUsing(
                        policy.ContentPolicy,
                        urlHelper);

            foreach (var nestedResource in nestedResources)
                resource.Embed(nestedResource);

            return
                resource;
        }

        private static void ResolveLinks<T>(
            T obj,
            WrapPolicy<T> policy,
            Resource resource,
            IUrlHelper urlHelper)
        {
            if (urlHelper == null)
                return;

            var applicableRules =
                policy
                    .LinkingRules
                    .Where(r => r.AddPredicate(obj));

            foreach (var rule in applicableRules)
            {
                var methodExpression =
                    rule.MethodCall.Body as MethodCallExpression
                        ?? throw new ApplicationException($"Only MethodCallExpression is supported. Given is: '{rule.MethodCall.Body.GetType()}'");

                var url =
                    Uri.UnescapeDataString(
                        urlHelper.Action(EvaluateActionContext()));

                resource.Link(
                    new NamedLink(
                        rule.Name,
                        new Link(url)));

                UrlActionContext EvaluateActionContext()
                {
                    return
                        new UrlActionContext()
                        {
                            Protocol =
                                urlHelper
                                    .ActionContext
                                    .HttpContext
                                    .Request
                                    .Scheme,

                            Host =
                                urlHelper
                                    .ActionContext
                                    .HttpContext
                                    .Request
                                    .Host
                                    .Host,

                            Controller =
                                rule.ControllerName,

                            Action =
                                methodExpression.Method.Name,

                            Values =
                                EvaluateValues(),
                        };
                }

                IDictionary<string, object> EvaluateValues()
                {
                    var values =
                        new Dictionary<string, object>();

                    foreach (var argument in GetMethodCallArguments())
                    {
                        var paramT =
                            TypedParameterExpressionFinder
                            .OfType<T>(argument.value)
                            .SingleOrDefault();

                        var value =
                            paramT == null
                                ? Expression
                                    .Lambda(argument.value)
                                    .Compile()
                                    .DynamicInvoke()
                                : Expression
                                    .Lambda(argument.value, paramT)
                                    .Compile()
                                    .DynamicInvoke(obj);

                        values.Add(
                            argument.parameter.Name,
                            value);
                    }

                    foreach (var (name, value) in rule.QueryParameters)
                        values[name] = value;

                    foreach (var (name, value) in rule.GetQueryParametersCallback(obj))
                        values[name] = value;

                    return
                        values;
                }

                IEnumerable<(ParameterInfo parameter, Expression value)> GetMethodCallArguments()
                    => methodExpression
                        .Method
                        .GetParameters()
                        .Zip(
                            methodExpression.Arguments,
                            (parameter, argument) => (parameter, argument));
            }
        }

        private static void ResolveProperties<T>(
            T obj,
            WrapPolicy<T> policy,
            Resource resource)
        {
            foreach (var rule in policy)
            {
                var name =
                    ResolvePropertyName(
                        rule, 
                        policy.UseCamelCase);

                var getterFunc =
                    rule.Getter.Compile();

                resource[name] =
                    getterFunc(obj);
            }
        }

        private static string ResolvePropertyName<T>(
            PropertyWrap<T> rule,
            bool useCamelCase)
        {
            var name =
                string.IsNullOrEmpty(rule.Name)
                    ? NameResolver.ResolveFrom(rule.Getter)
                    : rule.Name;

            return
                useCamelCase
                    ? name.ToCamelCase()
                    : name;
        }
    }
}
