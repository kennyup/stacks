using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Logging;

namespace ConsoleApplication10.Aspects
{
    public class RequestStore : IRequestStore
    {
        public Task AppendAsync(RequestEntry entry)
        {
            Console.WriteLine(JsonConvert.SerializeObject(entry, Formatting.Indented));

            return Task.FromResult(0);
        }
    }
}