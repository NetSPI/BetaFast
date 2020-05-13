using System;

// Code credited to https://www.technical-recipes.com/2016/using-relaycommand-icommand-to-handle-events-in-wpf-and-mvvm/

namespace BetaBank.Event
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}