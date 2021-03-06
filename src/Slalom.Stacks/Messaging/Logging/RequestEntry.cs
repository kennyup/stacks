﻿using System;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Serialization;
using Slalom.Stacks.Utilities.NewId;

namespace Slalom.Stacks.Messaging.Logging
{
    /// <summary>
    /// Represents a request log entry - something that tracks the request at the application level.
    /// </summary>
    public class RequestEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEntry" /> class.
        /// </summary>
        public RequestEntry(Request request)
        {
            try
            {
                this.MessageBody = JsonConvert.SerializeObject(request.Message.Body, new JsonSerializerSettings
                {
                    ContractResolver = new CommandContractResolver()
                });
            }
            catch
            {
                this.MessageBody = "{ \"Error\" : \"Serialization failed.\" }";
            }
            if (request.Message is IMessage)
            {
                var message = (IMessage)request.Message;
                this.MessageType = message.MessageType?.FullName;
                this.MessageId = message.Id;
                this.TimeStamp = message.TimeStamp;
            }
            else
            {
                this.MessageType = request.Message.GetType().FullName;
            }
            this.SessionId = request.SessionId;
            this.UserName = request.User?.Identity?.Name;
            this.Path = request.Path;
            this.SourceAddress = request.SourceAddress;
            this.CorrelationId = request.CorrelationId;
            this.Parent = (request.Parent?.Message as IMessage)?.Id;
        }

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the instance identifier.
        /// </summary>
        /// <value>The instance identifier.</value>
        public string Id { get; set; } = NewId.NextId();

        /// <summary>
        /// Gets or sets the request payload.
        /// </summary>
        /// <value>The request payload.</value>
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the name of the request.
        /// </summary>
        /// <value>The name of the request.</value>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the parent request identifier.
        /// </summary>
        /// <value>The parent request identifier.</value>
        public string Parent { get; set; }

        /// <summary>
        /// Gets or sets the request path or URL.
        /// </summary>
        /// <value>The request path or URL.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the user host address.
        /// </summary>
        /// <value>The user host address.</value>
        public string SourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the message time stamp.
        /// </summary>
        /// <value>The the message stamp.</value>
        public DateTimeOffset? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }
}