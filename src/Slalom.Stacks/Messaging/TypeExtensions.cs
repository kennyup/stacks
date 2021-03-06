﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Xml.XPath;
using System.Linq;
using System.Xml.Linq;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging
{

    public class Comments
    {
        public string Summary { get; set; }

        public string Value { get; set; }

        public Comments(XNode node)
        {
            this.Summary = node.XPathSelectElement("summary")?.Value.Trim();
            this.Value = node.XPathSelectElement("value")?.Value.Trim();
        }
    }

    /// <summary>
    /// Extensions for types within messaging.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Assembly, XDocument> _commentsCache = new ConcurrentDictionary<Assembly, XDocument>();

        public static XDocument GetComments(this Assembly assembly)
        {
            return _commentsCache.GetOrAdd(assembly, a =>
            {
                var path = Path.Combine(Path.GetDirectoryName(a.Location), Path.GetFileNameWithoutExtension(a.Location) + ".xml");
                if (File.Exists(path))
                {
                    return XDocument.Load(path);
                }
                return null;
            });
        }

        public static Comments GetComments(this Type type)
        {
            var document = type.GetTypeInfo().Assembly.GetComments();
            if (document != null)
            {
                var node = document.XPathSelectElement("//member[@name=\"T:" + type.FullName + "\"]");
                if (node != null)
                {
                    return new Comments(node);
                }
            }
            return null;
        }

        public static Comments GetComments(this PropertyInfo property)
        {
            var document = property.DeclaringType.GetTypeInfo().Assembly.GetComments();
            if (document != null)
            {
                var node = document.XPathSelectElement("//member[@name=\"P:" + property.DeclaringType.FullName + "." + property.Name + "\"]");
                if (node != null)
                {
                    return new Comments(node);
                }
            }
            return null;
        }

        public static IEnumerable<EndPointProperty> GetInputProperties(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                yield return new EndPointProperty(property);
            }
        }

        /// <summary>
        /// Gets the path for the endPoint.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the path for the type.</returns>
        public static string GetPath(this Type type)
        {
            return type.GetAllAttributes<EndPointAttribute>().Select(e => e.Path).FirstOrDefault();
        }


        public static Type[] GetRules(this Type type)
        {
            return type.GetTypeInfo().Assembly.SafelyGetTypes(typeof(IValidate<>).MakeGenericType(type));
        }

        /// <summary>
        /// Gets the timeout for the endPoint.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the timeout for the type.</returns>
        public static TimeSpan? GetTimeout(this Type type)
        {
            var attribute = type.GetAllAttributes<EndPointAttribute>().FirstOrDefault();
            if (attribute != null && attribute.Timeout > 0)
            {
                return TimeSpan.FromMilliseconds(attribute.Timeout);
            }
            return null;
        }

        /// <summary>
        /// Gets the version for the endPoint.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the path for the type.</returns>
        public static int GetVersion(this Type type)
        {
            return type.GetAllAttributes<EndPointAttribute>().FirstOrDefault()?.Version ?? 1;
        }

        /// <summary>
        /// Determines whether the specified type is dynamic.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is dynamic; otherwise, <c>false</c>.</returns>
        public static bool IsDynamic(this Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }
    }
}