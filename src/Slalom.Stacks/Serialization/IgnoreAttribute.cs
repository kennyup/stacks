﻿using System;

namespace Slalom.Stacks.Serialization
{
    /// <summary>
    /// Indicates that a property should be ignored when logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}