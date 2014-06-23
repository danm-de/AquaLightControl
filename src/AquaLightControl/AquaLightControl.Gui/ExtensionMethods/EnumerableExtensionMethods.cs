using System;
using System.Collections.Generic;
using AquaLightControl.ClientApi.Annotations;

namespace AquaLightControl.Gui
{
    public static class EnumerableExtensionMethods
    {
        public static void ForEach<T>([NotNull] this IEnumerable<T> enumerable, [NotNull] Action<T> action) {
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