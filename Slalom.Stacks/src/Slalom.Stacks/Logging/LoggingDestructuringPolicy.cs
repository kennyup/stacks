﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Slalom.Stacks.Communication;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Serialization;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Logging
{
    internal class LoggingDestructuringPolicy : IDestructuringPolicy
    {
        private readonly Dictionary<Type, Func<object, ILogEventPropertyValueFactory, LogEventPropertyValue>> _cache = new Dictionary<Type, Func<object, ILogEventPropertyValueFactory, LogEventPropertyValue>>();
        private readonly object _cacheLock = new object();
        private readonly HashSet<Type> _ignored = new HashSet<Type>();

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            var type = value.GetType();
            lock (_cacheLock)
            {
                if (_ignored.Contains(type))
                {
                    result = null;
                    return false;
                }

                Func<object, ILogEventPropertyValueFactory, LogEventPropertyValue> cached;
                if (_cache.TryGetValue(type, out cached))
                {
                    result = cached(value, propertyValueFactory);
                    return true;
                }
            }

            var properties = type.GetPropertiesRecursive()
                                 .ToList();

            lock (_cacheLock)
            {
                _cache[type] = (o, f) => MakeStructure(o, properties, f, type);
            }

            return this.TryDestructure(value, propertyValueFactory, out result);
        }

        private static LogEventPropertyValue MakeStructure(object value, IEnumerable<PropertyInfo> properties, ILogEventPropertyValueFactory propertyValueFactory, Type type)
        {
            var structureProperties = new List<LogEventProperty>();
            foreach (var pi in properties)
            {
                if (pi.GetCustomAttributes<IgnoreAttribute>().Any())
                {
                    continue;
                }

                if (pi.GetCustomAttributes<SecureAttribute>().Any())
                {
                    structureProperties.Add(new LogEventProperty(pi.Name, new ScalarValue("[SECURE]")));
                    continue;
                }
                if (typeof(ClaimsPrincipal).IsAssignableFrom(pi.PropertyType))
                {
                    var user = pi.GetValue(value) as ClaimsPrincipal;
                    if (user != null)
                    {
                        structureProperties.Add(new LogEventProperty(pi.Name, new ScalarValue(user.Identity?.Name)));
                    }
                    continue;
                }

                if (typeof(IEnumerable<IEvent>).IsAssignableFrom(pi.PropertyType))
                {
                    var builder = new StringBuilder();
                    var events = (IEnumerable<IEvent>)pi.GetValue(value);
                    foreach (var instance in events)
                    {
                        builder.AppendLine(instance.EventName + ": " + instance.Id);
                    }
                    structureProperties.Add(new LogEventProperty(pi.Name, new ScalarValue(builder.ToString())));
                    continue;
                }

                if (typeof(IEnumerable<ValidationError>).IsAssignableFrom(pi.PropertyType))
                {
                    var builder = new StringBuilder();
                    var errors = (IEnumerable<ValidationError>)pi.GetValue(value);
                    foreach (var error in errors)
                    {
                        builder.AppendLine(error.ErrorType + ": " + error.Message);
                    }
                    structureProperties.Add(new LogEventProperty(pi.Name, new ScalarValue(builder.ToString())));
                    continue;
                }

                object propValue;
                try
                {
                    propValue = pi.GetValue(value);
                }
                catch (TargetInvocationException ex)
                {
                    SelfLog.WriteLine("The property accessor {0} threw exception {1}", pi, ex);
                    propValue = "The property accessor threw an exception: " + ex.InnerException.GetType().Name;
                }

                LogEventPropertyValue pv;

                if (propValue == null)
                {
                    pv = new ScalarValue(null);
                }
                else
                {
                    pv = propertyValueFactory.CreatePropertyValue(propValue, true);
                }

                structureProperties.Add(new LogEventProperty(pi.Name, pv));
            }

            return new StructureValue(structureProperties, type.Name);
        }
    }
}