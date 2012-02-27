using System;
using System.Collections.Generic;
using System.Linq;

namespace Deploy.King.Utilities
{
    public static class TypeExtensions
    {
        public static bool IsA<T>(this Type type)
        {
            return typeof (T).IsAssignableFrom(type);
        }

        public static bool IsA(this Type type, Type typeToBe)
        {
            if (!typeToBe.IsGenericTypeDefinition)
                return typeToBe.IsAssignableFrom(type);

            var toCheckTypes = new List<Type> { type };
            if (typeToBe.IsInterface)
                toCheckTypes.AddRange(type.GetInterfaces());

            var basedOn = type;
            while (basedOn.BaseType != null)
            {
                toCheckTypes.Add(basedOn.BaseType);
                basedOn = basedOn.BaseType;
            }

            return toCheckTypes.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeToBe);
        }

        public static bool IsA(this object instance, Type typeToBe)
        {
            if (instance == null)
                return false;
            return instance.GetType().IsA(typeToBe);
        }

        public static bool IsA<T>(this object instance)
        {
            return IsA(instance, typeof (T));
        }

        public static bool IsAGenericOf(this Type implementation, Type openGenericInterface, Type firstGenericArgument)
        {
            return implementation.IsA(openGenericInterface) && implementation.GetClosedGenericInterfaceFromImplementation(openGenericInterface).GetGenericArguments()[0].IsA(firstGenericArgument);
        }

        public static bool IsAGenericOf(this object instance, Type openGenericInterface, Type firstGenericArgument)
        {
            if (instance == null)
                return false;

            return IsAGenericOf(instance.GetType(), openGenericInterface, firstGenericArgument);
        }

        public static bool IsIEnumerableOf<T>(this Type type)
        {
            return type.IsIEnumerableOf(typeof (T));
        }

        public static bool IsIEnumerableOf(this Type type, Type firstGenericArgument)
        {
            return IsAGenericOf(type, typeof (IEnumerable<>), firstGenericArgument);
        }

        public static bool IsIEnumerableOf(this object instance, Type firstGenericArgument)
        {
            if (instance == null)
                return false;
            return IsIEnumerableOf(instance.GetType(), firstGenericArgument);
        }

        public static Type GetClosedGenericTypeFromImplementation(this Type implementation, Type openGenericType)
        {
            if (!openGenericType.IsGenericTypeDefinition)
                throw new ArgumentException("openGenericType is not generic", "openGenericType");

            var toCheckTypes = new List<Type> { implementation };
            if (openGenericType.IsInterface)
                toCheckTypes.AddRange(implementation.GetInterfaces());

            var basedOn = implementation;
            while (basedOn.BaseType != null)
            {
                toCheckTypes.Add(basedOn.BaseType);
                basedOn = basedOn.BaseType;
            }

            return toCheckTypes.Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericType);
        }

        public static Type GetClosedGenericInterfaceFromImplementation(this Type implementation, Type openGenericInterface)
        {
            if (!openGenericInterface.IsInterface)
                throw new NotSupportedException(string.Format("Method only supports interfaces, the supplied type was not an interface: {0}", openGenericInterface.FullName));

            return implementation.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericInterface);
        }

        public static Type[] GetConcreteGenericArgumentsFromGenericInterfaceImplementation(this Type implementation, Type openGenericInterface)
        {
            return implementation.GetClosedGenericInterfaceFromImplementation(openGenericInterface).GetGenericArguments();
        }

        public static bool HasInterface(this Type type, Type @interface)
        {
            return type.GetInterfaces().Any(x => x == @interface);
        }
    }
}