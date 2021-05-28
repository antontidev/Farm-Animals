using System.Collections.Generic;
using System;
using System.Linq;

namespace SukharevShared {
    public static class EnumResolver<E, T> where E : Enum {

        // This should be too slow
        public static Type GetType(E enumName) {
            Dictionary<E, Type> Types = new Dictionary<E, Type>();

            var list = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .ToList();

            var listEnum = Enum.GetNames(typeof(E));

            foreach (var type in list) {
                var typeName = type.Name;

                if (listEnum.Contains(typeName)) {
                    var enumValue = (E)Enum.Parse(typeof(E), typeName);

                    Types.Add(enumValue, type);
                }
            }

            return Types[enumName];
        }
    }
}
