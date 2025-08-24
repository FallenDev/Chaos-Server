#region
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Type" />.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Recursively enumerates all properties on a complex object, only returning the PropertyInfo objects of primitive or
    ///     string properties
    /// </summary>
    public static IEnumerable<PropertyInfo> EnumerateProperties(this Type type)
    {
        foreach (var property in type.GetProperties())
            if (property.PropertyType is { IsClass: true, IsValueType: false, IsPrimitive: false }
                && (property.PropertyType != typeof(string)))
                foreach (var nestedProperty in property.PropertyType.EnumerateProperties())
                    yield return nestedProperty;
            else
                yield return property;
    }

    /// <summary>
    ///     Extracts the generic base type that use a generic type definition within the hierarchy of the type.
    /// </summary>
    /// <param name="type">
    /// </param>
    /// <param name="genericBaseType">
    ///     A generic type definition (non-interface). (The type of a generic without the type params specified)
    /// </param>
    /// <returns>
    /// </returns>
    public static Type? ExtractGenericBaseType(this Type type, Type genericBaseType)
    {
        var current = type;

        while (current != null)
        {
            if (current.IsGenericType && (current.GetGenericTypeDefinition() == genericBaseType))
                return current;

            current = current.BaseType;
        }

        return null;
    }

    /// <summary>
    ///     Extracts all generic interfaces types from a generic type definition within the hierarchy of the type.
    /// </summary>
    /// <param name="type">
    /// </param>
    /// <param name="genericInterfaceType">
    ///     A generic type definition of an interface. (The type of a generic without the type params specified)
    /// </param>
    public static IEnumerable<Type> ExtractGenericInterfaces(this Type type, Type genericInterfaceType)
    {
        var interfaces = type.GetInterfaces();

        foreach (var @interface in interfaces)
            if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == genericInterfaceType))
                yield return @interface;
    }

    /// <summary>
    ///     Lazily yields all base types of a type
    /// </summary>
    public static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        var current = type.BaseType;

        while (current != null)
        {
            yield return current;

            current = current.BaseType;
        }
    }

    /// <summary>
    ///     Gets a generic method from a type
    /// </summary>
    public static MethodInfo? GetGenericMethod(this Type type, string methodName, Type[] genericTypes)
    {
        var method = type.GetMethod(methodName);

        if (method == null)
            return null;

        return method.MakeGenericMethod(genericTypes);
    }

    /// <summary>
    ///     Determines whether the specified type has a custom attribute of the specified attribute type.
    /// </summary>
    /// <param name="type">
    ///     The type to check for the attribute.
    /// </param>
    /// <param name="attributeType">
    ///     The type of the attribute to search for.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the type has the specified attribute, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool HasAttribute(this Type type, Type attributeType) => type.GetCustomAttribute(attributeType) is not null;

    /// <summary>
    ///     Determines whether a type inherits from the specified base type
    /// </summary>
    public static bool HasBaseType(this Type type, Type baseType)
    {
        var current = type;

        while (current != null)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                if (current.IsGenericType && (current.GetGenericTypeDefinition() == baseType))
                    return true;
            } else if (current == baseType)
                return true;

            current = current.BaseType;
        }

        return false;
    }

    /// <summary>
    ///     Determines whether a type inherits from an interface
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the type implements the interface, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool HasInterface(this Type type, Type interfaceType)
    {
        foreach (var iType in type.GetInterfaces())
            if (interfaceType.IsGenericTypeDefinition)
            {
                if (iType.IsGenericType && (iType.GetGenericTypeDefinition() == interfaceType))
                    return true;
            } else if (iType == interfaceType)
                return true;

        return false;
    }

    /// <summary>
    ///     Determines if a type is a compiler generated type.
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the type is compiler generated, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool IsCompilerGenerated(this Type type) => type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;

    /// <summary>
    ///     Loads all types from the current application domain's assemblies that are annotated with the specified attribute
    ///     type.
    /// </summary>
    /// <param name="type">
    ///     The attribute type used to filter the types in the assemblies.
    /// </param>
    /// <returns>
    ///     An enumerable collection of types annotated with the specified attribute type
    /// </returns>
    public static IEnumerable<Type> LoadAttributedTypes(this Type type)
        => AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a =>
                    {
                        try
                        {
                            return a.GetTypes();
                        } catch
                        {
                            return [];
                        }
                    })
                    .Where(asmType => asmType.HasAttribute(type));

    /// <summary>
    ///     Returns all constructable types that inherit from the specified type.
    /// </summary>
    public static IEnumerable<Type> LoadImplementations(this Type type)
    {
        var assemblyTypes = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .Where(a => !a.IsDynamic)
                                     .SelectMany(a =>
                                     {
                                         try
                                         {
                                             return a.GetTypes();
                                         } catch
                                         {
                                             return [];
                                         }
                                     })
                                     .Where(asmType => asmType is { IsInterface: false, IsAbstract: false });

        if (type.IsGenericTypeDefinition)
            return assemblyTypes.Where(asmType => type.IsInterface ? asmType.HasInterface(type) : asmType.HasBaseType(type));

        return assemblyTypes.Where(asmType => asmType.IsAssignableTo(type));
    }
}