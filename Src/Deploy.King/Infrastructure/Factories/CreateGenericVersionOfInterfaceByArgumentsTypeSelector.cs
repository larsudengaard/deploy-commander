using System;
using System.Linq;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Deploy.Utilities;

namespace Deploy.King.Infrastructure.Factories
{
    public class CreateGenericVersionOfInterfaceByArgumentsTypeSelector : DefaultTypedFactoryComponentSelector
    {
        protected override Type GetComponentType(MethodInfo method, object[] arguments)
        {
            var genericInterfaces = method.ReturnType.Assembly.GetTypes().Where(x =>
                                                                                x.IsInterface &&
                                                                                x.IsGenericType &&
                                                                                x.GetGenericArguments().Length == arguments.Length &&
                                                                                x.IsA(method.ReturnType)).ToList();

            if (genericInterfaces.Count != 1)
                throw new InvalidOperationException("Zero or more than one generic interface with " + arguments.Length + " generic type argument(s) derive from " + method.ReturnType);

            var typeArguments = arguments.Select(x => x is Type
                                                          ? x
                                                          : x.GetType()).Cast<Type>().ToArray();
            return genericInterfaces.Single().MakeGenericType(typeArguments);
        }
    }
}