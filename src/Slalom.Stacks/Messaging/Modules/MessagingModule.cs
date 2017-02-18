﻿using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Slalom.Stacks.Messaging.Validation;
using Slalom.Stacks.Reflection;
using Module = Autofac.Module;

namespace Slalom.Stacks.Messaging.Modules
{
    /// <summary>
    /// An Autofac module to configure the communication dependencies.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    internal class MessagingModule : Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingModule"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies used to probe.</param>
        public MessagingModule(Assembly[] assemblies)
        {
            this._assemblies = assemblies;
        }

        private Assembly[] _assemblies;

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new CommandCoordinator(c.Resolve<IComponentContext>()))
                   .As<ICommandCoordinator>()
                   .SingleInstance();

            builder.Register(c => new EventStream(c.Resolve<IComponentContext>()))
                   .As<IEventStream>()
                   .SingleInstance();

            builder.RegisterGeneric(typeof(CommandValidator<>));

            builder.RegisterModule(new MessagingTypesModule(this._assemblies));
        }
    }
}