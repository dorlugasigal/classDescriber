using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ClassDescriber.Library
{
    public class Describer
    {
        private int _level;
        private const int IndentationSize = 2;
        private readonly StringBuilder _output;
        private readonly HashSet<object> _propsUsed;

        private Describer()
        {
            _output = new StringBuilder();
            _propsUsed = new HashSet<object>();
        }

        public static string Describe(object element)
        {
            var instance = new Describer();
            return instance.DescribeElement(element);
        }

        private string DescribeElement(object element)
        {
            if (IsSimple(element))
            {
                Write(FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write($"{objectType.Name}");
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
                            Write($"{memberInfo.Name}: {FormatValue(value)}");
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            var addition = isEnumerable ? "..." : "{ }";
                            Write($"{memberInfo.Name}: {addition}");

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
            if (value == null)
            {
                return false;
            }

            foreach (var t in _propsUsed)
            {
                if (t == value)
                {
                    return true;
                }
            }
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * IndentationSize);

            if (args != null)
                value = string.Format(value, args);

            _output.AppendLine(space + value);
        }

        private static string FormatValue(object o)
        {
            switch (o)
            {
                case null:
                    return ("null");
                case DateTime time:
                    return (time.ToShortDateString());
                case string _:
                    return $"\"{o}\"";
                case char c when c == '\0':
                    return string.Empty;
                case ValueType _:
                    return (o.ToString());
                case IEnumerable _:
                    return ("...");
                default:
                    return ("{ }");
            }
        }
    }
}