﻿using System;

namespace Slalom.Stacks.Serialization
{
    /// <summary>
    /// Indicates that a property should be handled securely when logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SecureAttribute : Attribute
    {
        /// <summary>
        /// The default display text.
        /// </summary>
        public const string DefaultDisplayText = "[SECURE]";
    }
}