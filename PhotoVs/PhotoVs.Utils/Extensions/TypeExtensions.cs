using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Utils.Extensions
{
    public static class TypeExtensions
    {
        public static List<Type> GetInterfaces(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsClass == false)
                return new List<Type>();

            var allInterfaces = new HashSet<Type>(type.GetInterfaces());
            var baseType = type.BaseType;
            if (baseType != null)
                allInterfaces.ExceptWith(baseType.GetInterfaces());

            var toRemove = new HashSet<Type>();
            foreach (var implementedByMostDerivedClass in allInterfaces)
                foreach (var implementedByOtherInterfaces in implementedByMostDerivedClass.GetInterfaces())
                    toRemove.Add(implementedByOtherInterfaces);

            allInterfaces.ExceptWith(toRemove);

            return allInterfaces.ToList();
        }
    }
}