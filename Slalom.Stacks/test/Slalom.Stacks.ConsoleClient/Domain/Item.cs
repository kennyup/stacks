﻿using Slalom.Stacks.Domain;

public class Item : Entity, IAggregateRoot
{
    protected Item()
    {
    }

    public static Item Create(string text)
    {
        return new Item
        {
            Text = text
        };
    }

    public string Text { get; private set; }
}