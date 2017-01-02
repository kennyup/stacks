﻿using System;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Utilities.NewId;

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// An imperative message to perform an action.  It can either request to change state, which returns an event message, 
    /// or can request data, which returns a document message.
    /// </summary>
    /// <seealso cref="ICommand" />
    /// <seealso href="http://bit.ly/2d01rc7">Reactive Messaging Patterns with the Actor Model: Applications and Integration in Scala and Akka</seealso>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; } = NewId.NextId();

        /// <summary>
        /// Gets the current execution context.
        /// </summary>
        /// <value>The current execution context.</value>
        public ExecutionContext Context { get; private set; }

        /// <summary>
        /// Sets the current execution context.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        public void SetExecutionContext(ExecutionContext context)
        {
            if (this.Context != null)
            {
                throw new InvalidOperationException("The execution context has already been set and cannot be reset.");
            }
            this.Context = context;
        }

        /// <summary>
        /// Gets the message time stamp.
        /// </summary>
        /// <value>The message time stamp.</value>
        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public virtual string CommandName => this.GetType().Name;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Command)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        protected bool Equals(Command other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}