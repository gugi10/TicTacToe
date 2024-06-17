using System;

namespace Auxiliary.Observables
{
    public class Observer<T> : IObserver<T>
    {
        public bool IsObserving => _cancellation != null;

        protected Action<T> OnNextEvent;
        protected Action OnCompletedEvent;
        protected Action<Exception> OnErrorEvent;
        protected Action OnSubscribedEvent;
        protected Action OnUnsubscribedEvent;

        protected IDisposable _cancellation;

        public void OnCompleted()
        {
            OnCompletedEvent?.Invoke();
        }

        public void OnError(Exception error)
        {
            OnErrorEvent?.Invoke(error);
        }

        public void OnNext(T value)
        {
            OnNextEvent?.Invoke(value);
        }

        public Observer<T> Observe(ObservableProperty<T> observableProperty, bool notifyOnSubscribe = true)
        {
            if (IsObserving)
            {
                throw new InvalidOperationException("Already observing - observing another Observable is not allowed.");
            }

            _cancellation = observableProperty.Subscribe(this, notifyOnSubscribe);
            OnSubscribedEvent?.Invoke();
            return this;
        }

        public virtual void StopObserving()
        {
            _cancellation?.Dispose();
            _cancellation = null;

            OnUnsubscribedEvent?.Invoke();

            OnNextEvent = null;
            OnCompletedEvent = null;
            OnErrorEvent = null;
            OnSubscribedEvent = null;
            OnUnsubscribedEvent = null;
        }

        public Observer<T> AddOnNextListener(Action<T> listener)
        {
            OnNextEvent += listener;
            return this;
        }

        public Observer<T> AddOnCompletedListener(Action listener)
        {
            OnCompletedEvent += listener;
            return this;
        }

        public Observer<T> AddOnErrorListener(Action<Exception> listener)
        {
            OnErrorEvent += listener;
            return this;
        }

        public Observer<T> AddOnSubscribedListener(Action listener)
        {
            OnSubscribedEvent += listener;
            return this;
        }

        public Observer<T> AddOnUnsubscribedListener(Action listener)
        {
            OnUnsubscribedEvent += listener;
            return this;
        }
    }
}