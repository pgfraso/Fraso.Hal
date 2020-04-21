using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fraso.Hal.Conversions
{
    public sealed class LinkingRule<T>
    {
        #region Fields
        public readonly string Name;

        public readonly string ControllerName;

        public readonly LambdaExpression MethodCall;

        public readonly (string, object)[] QueryParameters;

        public readonly Func<T, IEnumerable<(string, object)>> GetQueryParametersCallback;

        public readonly Func<T, bool> AddPredicate;
        #endregion // Fields

        internal LinkingRule(
            string name,
            string controllerName,
            LambdaExpression methodCall,
            (string, object)[] queryParameters = null,
            Func<T, IEnumerable<(string, object)>> getQueryParametersCallback = null,
            Func<T, bool> addPredicate = null)
        {
            Name = name;
            ControllerName = controllerName;
            MethodCall = methodCall;

            QueryParameters =
                queryParameters
                    ?? new (string, object)[0];

            GetQueryParametersCallback =
                getQueryParametersCallback
                    ?? (instance => Enumerable.Empty<(string, object)>());

            AddPredicate =
                addPredicate
                    ?? (instance => true);
        }
    }

    public static class LinkingRule
    {
        public static LinkingRule<T> FromMethod<T, TController>(
            Expression<Action<T, TController>> methodCall)
        {
            var methodExpression =
                   methodCall.Body as MethodCallExpression
                       ?? throw new ApplicationException($"Only MethodCallExpression is supported. Given is: '{methodCall.Body.GetType()}'");

            return
                new LinkingRule<T>(
                    methodExpression.Method.Name,
                    GetControllerName(),
                    methodCall);

            static string GetControllerName()
            {
                const string
                    ControllerSufix = "Controller";

                var controllerTypeName =
                    typeof(TController).Name;

                return
                    controllerTypeName
                        .Substring(0, controllerTypeName.Length - ControllerSufix.Length);
            }
        }
    }
}
