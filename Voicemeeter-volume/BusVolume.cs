using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voicemeeter_volume
{
    public class BusVolume : IDisposable, IObservable<float[]>
    {
        private readonly List<int> channels = new List<int>();
        private readonly List<IObserver<float[]>> observers = new List<IObserver<float[]>>();
        private readonly IObservable<int> timer;
        private IDisposable timerSubscription;

        public BusVolume(int[] channels, int milliseconds = 20)
        {
            this.channels = new List<int>(channels);
            this.timer = Observable.Interval(TimeSpan.FromMilliseconds(milliseconds)).Select(_ => 1);
            Watch();
        }

        private void Watch()
        {
            timerSubscription = timer.Subscribe(_ =>
            {
                var values = new List<float>(channels.Count);
                foreach (var channel in channels)
                {
                    if(VoiceMeeter.Remote.IsParametersDirty() == 1)
                        values.Add(VoiceMeeter.Remote.GetParameter("Bus[0].Gain"));
                }

                if(values.Count > 0)
                    Notify(values.ToArray());
            });
        }

        public IDisposable Subscribe(IObserver<float[]> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        private void Notify(float[] values)
        {
            foreach (var observer in observers)
                observer.OnNext(values);
        }

        public void Dispose()
        {
            timerSubscription?.Dispose();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<float[]>> _observers;
            private readonly IObserver<float[]> _observer;

            public Unsubscriber(List<IObserver<float[]>> observers, IObserver<float[]> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
