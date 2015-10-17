using System;

namespace Uno
{
    public class OneSubscriberEvent<T> where T: EventArgs
    {
        private EventHandler<T> _event;

        public void Subscribe(EventHandler<T> handler)
        {
            _event = handler;
        }

        public void Unsubscribe()
        {
            _event = null;
        }

        public void Invoke(object sender, T eventArgs)
        {
            _event?.Invoke(sender, eventArgs);
        }

        public static OneSubscriberEvent<T> operator+(OneSubscriberEvent<T> @event, EventHandler<T> handler)
        {
            @event.Subscribe(handler);
            return @event;
        }
    }

    public class OneSubscriberEvent
    {
        private EventHandler _event;
        public void Subscribe(EventHandler handler)
        {
            _event = handler;
        }
        public void Unsubscribe()
        {
            _event = null;
        }
        public void Invoke(object sender)
        {
            _event?.Invoke(sender, EventArgs.Empty);
        }
        public static OneSubscriberEvent operator +(OneSubscriberEvent @event, EventHandler handler)
        {
            @event.Subscribe(handler);
            return @event;
        }
    }
}