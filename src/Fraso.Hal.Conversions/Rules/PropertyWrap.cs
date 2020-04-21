using System;
using System.Linq.Expressions;

namespace Fraso.Hal.Conversions
{
    public sealed class PropertyWrap<T>
    {
        public readonly string Name;

        public readonly Expression<Func<T, object>> Getter;

        #region Ctors
        internal PropertyWrap(Expression<Func<T, object>> getter)
            : this(null, getter) { }

        internal PropertyWrap(
            string name, 
            Expression<Func<T, object>> getter)
        {
            Name = name;
            Getter = getter;
        }
        #endregion //Ctors
    }

    internal static class Wrap
    {
        public static PropertyWrap<T> Property<T>(Expression<Func<T, object>> getter)
            => new PropertyWrap<T>(getter);

        public static PropertyWrap<TIn> As<TIn>(this PropertyWrap<TIn> wrap, string name)
           => new PropertyWrap<TIn>(name, wrap.Getter);
    }
}
