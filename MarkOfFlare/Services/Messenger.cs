using System;
using System.Collections.Generic;

namespace MarkOfFlare.Services
{
    public interface IMessenger
    {
        void Send<TMessage>(TMessage message);

        void Register<TMessage>(Action<TMessage> onMessageReceived);

    }

    public class Messenger: IMessenger
    {
        private readonly Dictionary<Type, object> _mappings = new Dictionary<Type, object>();

        public Messenger()
        {
        }

        public void Register<TMessage>(Action<TMessage> onMessageReceived)
        {
            HashSet<Action<TMessage>> actions;
            if (!_mappings.TryGetValue(typeof(TMessage), out object @object))
            {
                actions = new HashSet<Action<TMessage>>();
                _mappings[typeof(TMessage)] = actions;
            }
            else
            {
                actions = (HashSet<Action<TMessage>>)@object;
            }

            actions.Add(onMessageReceived);
        }

        public void Send<TMessage>(TMessage message)
        {
            if (!_mappings.TryGetValue(typeof(TMessage), out object @object))
            {
                return;
            }

            var actions = (HashSet<Action<TMessage>>)@object;

            foreach (var action in actions)
            {
                action(message);
            }
        }
    }
}
