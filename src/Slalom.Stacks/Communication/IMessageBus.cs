﻿using System.Threading.Tasks;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Communication
{
    /// <summary>
    /// Defines an Application Message Bus.  An Application Message bus is an addition to and not a replacement to a typical Service Bus.  
    /// It runs in memory and is intended to send only to in-memory receivers.
    /// </summary>
    /// <seealso href="[Documentation URL]"/>
    public interface IMessageBus
    {
        /// <summary>
        /// Sends the specified command to the service bus and attaches a <seealso cref="IExecutionContextResolver">context</seealso> before multi-threading.
        /// </summary>
        /// <typeparam name="TResult">The return type of the command.</typeparam>
        /// <param name="command">The command to send and execute.</param>
        /// <returns>A task for asynchronous programming.</returns>
        Task<CommandResult<TResult>> Send<TResult>(Command<TResult> command);
    }
}