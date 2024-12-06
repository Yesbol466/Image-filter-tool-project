using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1_ObjectCreator
{
    public static class TypeCrafter
    {
        public static T TypeCraft<T>()
        {
            try
            {
                Type type = typeof(T);
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException($"No parameterless constructor found for {type.Name}");
                T instance = (T)(constructor.Invoke(null) ?? throw new InvalidOperationException($"Cannot create an instance of {type.Name}"));

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (!property.CanWrite) continue; // Skip properties without a setter

                    while (true)
                    {
                        Console.WriteLine($"Enter value for {property.Name} ({property.PropertyType.Name}):");
                        string input = Console.ReadLine() ?? throw new InvalidOperationException("Input cannot be null");

                        try
                        {
                            object value = ParseValue(property.PropertyType, input);
                            property.SetValue(instance, value);
                            break;
                        }
                        catch (ParseException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public static object ParseValue(Type type, string input)
        {
            try
            {
                if (type == typeof(string))
                {
                    return input;
                }
                MethodInfo tryParseMethod = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
                if (tryParseMethod != null)
                {
                    object[] parameters = { input, Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Cannot create an instance of {type.Name}") };
                    bool success = (bool)(tryParseMethod.Invoke(null, parameters) ?? false);
                    if (success)
                    {
                        return parameters[1];
                    }
                    else
                    {
                        throw new ParseException($"Invalid input for type {type.Name}.");
                    }
                }
                if (type.IsClass)
                {
                    Console.WriteLine($"Creating instance of {type.Name}");
                    return TypeCraft(type);
                }
                throw new ParseException($"Unsupported type {type.Name}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while parsing value: {ex.Message}");
                throw;
            }
        }

        public static object TypeCraft(Type type)
        {
            try
            {
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException($"No parameterless constructor found for {type.Name}");
                object instance = (constructor.Invoke(null) ?? throw new InvalidOperationException($"Cannot create an instance of {type.Name}"));

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (!property.CanWrite) continue;

                    while (true)
                    {
                        Console.WriteLine($"Enter value for {property.Name} ({property.PropertyType.Name}):");
                        string input = Console.ReadLine() ?? throw new InvalidOperationException("Input cannot be null");

                        try
                        {
                            object value = ParseValue(property.PropertyType, input);
                            property.SetValue(instance, value);
                            break;
                        }
                        catch (ParseException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
