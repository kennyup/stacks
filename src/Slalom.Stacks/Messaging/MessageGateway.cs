﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Events;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// A default <see cref="IMessageGateway" /> implementation.
    /// </summary>
    public class MessageGateway : IMessageGateway
    {
        private readonly Lazy<ILocalMessageDispatcher> _dispatcher;
        private readonly Lazy<IRequestContext> _requestContext;
        private readonly Lazy<IRequestLog> _requests;
        private readonly Lazy<ServiceRegistry> _services;
        private Lazy<IEnumerable<IRemoteMessageDispatcher>> _dispatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageGateway" /> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public MessageGateway(IComponentContext components)
        {
            Argument.NotNull(components, nameof(components));

            _services = new Lazy<ServiceRegistry>(components.Resolve<ServiceRegistry>);
            _requestContext = new Lazy<IRequestContext>(components.Resolve<IRequestContext>);
            _requests = new Lazy<IRequestLog>(components.Resolve<IRequestLog>);
            _dispatcher = new Lazy<ILocalMessageDispatcher>(components.Resolve<ILocalMessageDispatcher>);
            _dispatchers = new Lazy<IEnumerable<IRemoteMessageDispatcher>>(components.ResolveAll<IRemoteMessageDispatcher>);
        }

        /// <inheritdoc />
        public virtual async Task Publish(EventMessage instance, ExecutionContext context)
        {
            Argument.NotNull(instance, nameof(instance));

            var request = _requestContext.Value.Resolve(instance, context.Request);
            await _requests.Value.Append(request);

            //var endPoints = _services.Value.Find(instance);
            //foreach (var endPoint in endPoints)
            //{
            //    _dispatchers.Value.ToList().ForEach(e => e.Dispatch(request, endPoint, context));
            //}
            //_dispatchers.Value.ToList().ForEach(e => e.Dispatch(request, context));
        }

        /// <inheritdoc />
        public async Task Publish(IEnumerable<EventMessage> instances, ExecutionContext context = null)
        {
            Argument.NotNull(instances, nameof(instances));

            foreach (var item in instances)
            {
                await this.Publish(item, context);
            }
        }

        /// <inheritdoc />
        public Task<MessageResult> Send(object message, ExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            return this.Send((string) null, message, parentContext, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<MessageResult> Send(string path, object instance, ExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var endPoint = _services.Value.Find(path, instance);
            if (endPoint != null)
            {
                var request = _requestContext.Value.Resolve(instance, endPoint, parentContext?.Request);
                return await _dispatcher.Value.Dispatch(request, endPoint, parentContext, timeout);
            }
            else
            {
                var request = _requestContext.Value.Resolve(path, instance, parentContext?.Request);
                var dispatcher = _dispatchers.Value.FirstOrDefault(e => e.CanDispatch(request));
                if (dispatcher != null)
                {
                    return await dispatcher.Dispatch(request, parentContext, timeout);
                }
            }
            throw new InvalidOperationException("No endpoint could be found for the request.");
        }

        /// <inheritdoc />
        public virtual async Task<MessageResult> Send(string path, string command, ExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var endPoint = _services.Value.Find(path);
            if (endPoint != null)
            {
                var request = _requestContext.Value.Resolve(command, endPoint, parentContext?.Request);
                return await _dispatcher.Value.Dispatch(request, endPoint, parentContext, timeout);
            }
            else
            {
                var request = _requestContext.Value.Resolve(path, command, parentContext?.Request);
                var dispatcher = _dispatchers.Value.FirstOrDefault(e => e.CanDispatch(request));
                if (dispatcher != null)
                {
                    return await dispatcher.Dispatch(request, parentContext, timeout);
                }
            }
            throw new InvalidOperationException("No endpoint could be found for the request.");
        }
    }
}