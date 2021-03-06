﻿using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Pipeline
{
    /// <summary>
    /// The log completion step of the use case execution pipeline.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Messaging.Pipeline.IMessageExecutionStep" />
    public class LogCompletion : IMessageExecutionStep
    {
        private readonly IResponseLog _actions;
        private readonly IEnvironmentContext _environmentContext;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCompletion"/> class.
        /// </summary>
        /// <param name="components">The component context.</param>
        public LogCompletion(IComponentContext components)
        {
            _actions = components.Resolve<IResponseLog>();
            _logger = components.Resolve<ILogger>();
            _environmentContext = components.Resolve<IEnvironmentContext>();
        }

        /// <inheritdoc />
        public Task Execute(IMessage instance, ExecutionContext context)
        {
            var tasks = new List<Task> { _actions.Append(new ResponseEntry(context, _environmentContext.Resolve())) };

            var name = context.Request.Message.Name;
            if (!context.IsSuccessful)
            {
                if (context.Exception != null)
                {
                    _logger.Error(context.Exception, "An unhandled exception was raised while executing \"" + name + "\".", context);
                }
                else if (context.ValidationErrors?.Any() ?? false)
                {
                    _logger.Error("Execution completed with validation errors while executing \"" + name + "\": " + string.Join("; ", context.ValidationErrors.Select(e => e.Type + ": " + e.Message)), context);
                }
                else
                {
                    _logger.Error("Execution completed unsuccessfully while executing \"" + name + "\".", context);
                }
            }
            else
            {
                _logger.Verbose("Successfully executed \"" + name + "\".", context);
            }

            return Task.WhenAll(tasks);
        }
    }
}