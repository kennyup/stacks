﻿using System;

namespace Slalom.Stacks.Communication
{
    /// <summary>
    /// An imperative message to perform an action.
    /// </summary>
    /// <seealso href="http://bit.ly/2d01rc7"/>
    public interface ICommand : IMessage
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        string CommandName { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        Guid Id { get; }
    }
}