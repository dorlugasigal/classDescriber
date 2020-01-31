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
    public class Describer
    {
        private int _level;
        private readonly StringBuilder _output;
        private readonly HashSet<object> _propsUsed;

        private Describer()
        {
            _output = new StringBuilder();
            _propsUsed = new HashSet<object>();
        }

        public static string Describe(object element)
        {
            try
            {
                var instance = new Describer();
                return instance.DescribeElement(element);
            }
            catch (Exception)
            {
                return "Error Parsing Object";
            }
        }

        private string DescribeElement(object element)
        {
            if (IsSimple(element))
            {
                AppendOutput(ObjectSegregation(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    AppendOutput($"Object of Class {objectType.Name}");
                    _propsUsed.Add(element);
                    _level++;
                }

                if (element is IEnumerable enumerableElement)
                {
                    foreach (var item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _level++;
                            DescribeElement(item);
                            _level--;
                        }
                        else
                        {
                            if (!AlreadyTouched(item))
                            {
                                DescribeElement(item);
                            }
                        }
                    }
                }
                else
                {
                    var members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                        {
                            continue;
                        }

                        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        var value = fieldInfo != null
                            ? fieldInfo.GetValue(element)
                            : propertyInfo.GetValue(element, null);

                        if (type.IsValueType || type == typeof(string))
                        {
                            AppendOutput($"{memberInfo.Name}: {ObjectSegregation(value)}");
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            var addition = isEnumerable ? "Enumerable :" : "";
                            AppendOutput($"{memberInfo.Name} {addition}");

                            var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                            _level++;
                            if (!alreadyTouched)
                            {
                                DescribeElement(value);
                            }
                            _level--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    _level--;
                }
            }

            return _output.ToString();
        }

        private static bool IsSimple(object element)
        {
            return element == null || element is ValueType || element is string;
        }

        private bool AlreadyTouched(object value)
        {
            return value != null && _propsUsed.Select(x => x == value).Any();
        }

        private void AppendOutput(string value)
        {
            _output.AppendLine(' '.Repeat(_level * 2) + value);
        }

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