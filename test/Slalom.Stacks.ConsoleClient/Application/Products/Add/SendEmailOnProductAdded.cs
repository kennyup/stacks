using System;
using Slalom.Stacks.Messaging;

namespace Slalom.Stacks.ConsoleClient.Application.Products.Add
{
    public class SendEmailOnProductAdded : UseCase<AddProductEvent>
    {
        public override void Execute(AddProductEvent message)
        {
            Console.WriteLine("Sending mail.");

            throw new Exception("SS");
        }
    }
}