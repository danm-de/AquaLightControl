using System;
using System.Collections.Generic;

namespace AquaLightControl.Service.ExtensionMethods
{
    public static class EnumerableExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable,  Action<T> action) {
            if (ReferenceEquals(enumerable, null)) {
                throw new ArgumentNullException("enumerable");
            }
            if (ReferenceEquals(action, null)) {
                throw new ArgumentNullException("action");
            }

            foreach (var element in enumerable) {
                action(element);
            }
        }
    }
}