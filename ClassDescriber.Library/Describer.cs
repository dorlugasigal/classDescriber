using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using ClassDescriber.Library.Extensions;

namespace ClassDescriber.Library
{
    /// <summary>
    /// Describer class, purpose of the class is to Serialize any struct object
    /// with Reflection API
    /// </summary>
    public class Describer
    {
        private int _indentationLevel;
        private readonly StringBuilder _output;
        private readonly HashSet<object> _propsUsed;

        /// <summary>
        /// private constructor
        /// </summary>
        private Describer()
        {
            _output = new StringBuilder();
            _propsUsed = new HashSet<object>();
        }

        /// <summary>
        /// activator function, creates an instance and begin parsing the object
        /// </summary>
        /// <param name="prop"> the required element</param>
        /// <returns>the string required</returns>
        public static string Describe(object prop)
        {
            try
            {
                return new Describer().DescribeProp(prop);
            }
            catch (Exception)
            {
                return "Error Parsing Object";
            }
        }
        /// <summary>
        /// check whether the item is a struct or a simple element,
        /// and print it
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string DescribeProp(object input)
        {
            if (IsSimple(input))
            {
                AppendOutput(ObjectSegregation(input));
            }
            else
            {
                var inputType = input.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(inputType))
                {
                    AppendOutput($"Object of Class {inputType.Name}");
                    _propsUsed.Add(input);
                    _indentationLevel++;
                }

                if (input is IEnumerable sameItemButEnumerable)
                {
                    foreach (var innerProp in sameItemButEnumerable)
                    {
                        if (innerProp is IEnumerable _ && !(innerProp is string))
                        {
                            _indentationLevel++;
                            DescribeProp(innerProp);
                            _indentationLevel--;
                        }
                        else
                        {
                            if (WasShownBefore(innerProp)) continue;
                            DescribeProp(innerProp);
                        }
                    }
                }
                else
                {
                    var innerProps = input.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var innerProp in innerProps)
                    {
                        var fieldInfo = innerProp as FieldInfo;
                        var propertyInfo = innerProp as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                        {
                            continue;
                        }

                        var fieldType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        var fieldValue = fieldInfo != null
                            ? fieldInfo.GetValue(input)
                            : propertyInfo.GetValue(input, null);
                        if (fieldValue == null) throw new ArgumentNullException(nameof(fieldValue));

                        if (fieldType.IsValueType || fieldType == typeof(string))
                        {
                            AppendOutput($"{innerProp.Name}: {ObjectSegregation(fieldValue)}");
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(fieldType);
                            var addition = isEnumerable ? "Enumerable :" : "";
                            AppendOutput($"{innerProp.Name} {addition}");

                            var wasShownBefore = !isEnumerable && WasShownBefore(fieldValue);
                            _indentationLevel++;
                            if (!wasShownBefore)
                            {
                                DescribeProp(fieldValue);
                            }
                            _indentationLevel--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(inputType))
                {
                    _indentationLevel--;
                }
            }

            return _output.ToString();
        }

        /// <summary>
        /// check if item is simple (has to string)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool IsSimple(object element)
        {
            return element == null || element is ValueType || element is string;
        }

        /// <summary>
        /// check if the item was shown before, to avoid inner references loops
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns></returns>
        private bool WasShownBefore(object value)
        {
            return value != null && _propsUsed.Select(x => x == value).Any();
        }

        /// <summary>
        /// append new line to input
        /// </summary>
        /// <param name="value">data to write</param>
        private void AppendOutput(string value)
        {
            _output.AppendLine(' '.Repeat(_indentationLevel * 2) + value);
        }

        /// <summary>
        /// separate types of objects 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjectSegregation(object obj)
        {
            switch (obj)
            {
                case null:
                    return ("null");
                case DateTime time:
                    return (time.ToShortDateString());
                case string _:
                    return $"\"{obj}\"";
                case char c when c == '\0':
                    return string.Empty;
                case ValueType _:
                    return (obj.ToString());
                case IEnumerable _:
                    return ("...");
                default:
                    return ("{ }");
            }
        }
    }
}