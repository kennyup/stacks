﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Search;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Validation
{
    /// <summary>
    /// Base class for business validation rule.
    /// </summary>
    /// <typeparam name="TValue">The message type.</typeparam>
    /// <seealso cref="Slalom.Stacks.Messaging.Validation.IBusinessRule{TValue}" />
    /// <seealso cref="IBusinessRule{TCommand}" />
    public abstract class BusinessRule<TValue> : IBusinessRule<TValue>, IUseExecutionContext
    {
        /// <summary>
        /// Gets the configured <see cref="IDomainFacade"/>.
        /// </summary>
        /// <value>The configured <see cref="IDomainFacade"/>.</value>
        public IDomainFacade Domain { get; protected set; }

        /// <summary>
        /// Validates the specified message instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A task for asynchronous programming.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="instance"/> argument is null.</exception>
        /// <exception cref="System.NotImplementedException">Thrown when neither validate methods are implemented.</exception>
        public virtual IEnumerable<ValidationError> Validate(TValue instance)
        {
            Argument.NotNull(instance, nameof(instance));

            var result = this.ValidateAsync(instance).Result;
            if (result == null)
            {
                return Enumerable.Empty<ValidationError>();
            }
            return new[] { result };
        }


        /// <summary>
        /// Validates the specified message instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A task for asynchronous programming.</returns>
        /// <exception cref="System.NotImplementedException">Thrown when neither validate methods are implemented.</exception>
        public virtual Task<ValidationError> ValidateAsync(TValue instance)
        {
            throw new NotImplementedException();
        }

        public void UseContext(ExecutionContext context)
        {
            this.Context = context;
        }

        public ExecutionContext Context { get; set; }
    }
}