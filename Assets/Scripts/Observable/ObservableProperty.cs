using System;
using System.Collections.Generic;
using System.Linq;

namespace Auxiliary.Observables
{
    public class ObservableProperty<T>
    {
        public T Value { get; internal set; }
        private readonly List<IObserver<T>> _observers = new();

        public ObservableProperty()
        {

        }

        public ObservableProperty(T initialValue)
        {
            Set(initialValue);
        }

        public IDisposable Subscribe(IObserver<T> observer, bool notifyOnSubscribe = true)
        {
            // Check whether observer is already registered. If not, add it
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                // Provide observer with existing data.
                if (notifyOnSubscribe)
                    observer.OnNext(Value);
            }
            return new Unsubscriber<T>(_observers, observer);
        }

        public void Set(T value, bool notify = true)
        {
            Value = value;

            if (notify)
            {
                Notify();
            }
        }

        public void Notify()
        {
            var observers = _observers.ToList();

            foreach (var observer in observers)
                observer.OnNext(Value);
        }
    }
}