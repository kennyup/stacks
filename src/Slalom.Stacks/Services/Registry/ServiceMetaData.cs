using System;
using System.Collections.Generic;
using System.Linq;
using Slalom.Stacks.Messaging;

namespace Slalom.Stacks.Services.Registry
{
    public class ServiceMetaData
    {
        public ServiceMetaData(Type service, string rootPath)
        {
            this.Path = service.GetPath();
            this.RootPath = rootPath;
            this.EndPoints = EndPointMetaData.Create(service).ToList();
        }

        public ServiceMetaData()
        {
        }

        /// <summary>
        /// Gets or sets the end points.
        /// </summary>
        /// <value>The end points.</value>
        public List<EndPointMetaData> EndPoints { get; set; } = new List<EndPointMetaData>();

        public string Path { get; set; }

        public string RootPath { get; set; }

        public ServiceMetaData Copy(string path)
        {
            return new ServiceMetaData
            {
                Path = this.Path,
                RootPath = path,
                EndPoints = this.EndPoints.Select(e => e.Copy(path)).ToList()
            };
        }
    }
}