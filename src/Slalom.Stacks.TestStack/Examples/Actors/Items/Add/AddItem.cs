using System;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.TestStack.Examples.Domain;

namespace Slalom.Stacks.TestStack.Examples.Actors.Items.Add
{
    [Path("items/add")]
    public class AddItem : Actor<AddItemCommand, AddItemEvent>
    {
        public override async Task<AddItemEvent> ExecuteAsync(AddItemCommand message)
        {
            if (message.Name == "error")
            {
                throw new Exception("Throwing an example error.");
            }

            var target = Item.Create(message.Name);

            await this.Domain.AddAsync(target);

            return new AddItemEvent(target);
        }
    }
}