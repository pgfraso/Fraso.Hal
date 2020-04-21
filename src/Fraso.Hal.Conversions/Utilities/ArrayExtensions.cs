using System;

namespace Fraso.Hal.Conversions
{
    public static class ArrayExtensions
    {
        public static T[] AddToTail<T>(
            this T[] array,
            T obj)
        {
            var length = array.Length;

            Array.Resize(
                ref array,
                length + 1);

            array[length] = obj;

            return
                array;
        }
    }
}
