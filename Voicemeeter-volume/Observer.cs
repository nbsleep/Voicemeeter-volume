using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voicemeeter_volume
{
    public class Observer : IObserver<float[]>
    {
        public void OnCompleted()
        {
            Console.WriteLine("CONA");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(float[] value)
        {
            Console.WriteLine(value[0]);
        }
    }
}
