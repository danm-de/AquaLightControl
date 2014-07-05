using System.Collections.Generic;

namespace AquaLightControl.Gui
{
    public static class CollectionExtensionMethods
    {
        public static bool IsEmpty<T>(this ICollection<T> list) {
            return list.Count == 0;
        }
    }
}