using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public abstract class BackgroundWorker
    {
        private long Interval;
        private Timer Timer;

        public long TimerInterval
        {
            set
            {
                if (value <= 0)
                    throw new IndexOutOfRangeException();

                this.Interval = value;
                if (IsRunning())
                    Start();
            }

            get
            {
                return Interval;
            }
        }

        public BackgroundWorker()
        {
            TimerInterval = 1000;
        }

        public BackgroundWorker(long interval)
        {
            TimerInterval = interval;
        }

        public bool IsRunning()
        {
            return Timer != null && Timer.Enabled;
        }

        public void Run()
        {
            OnTick(null, null);
        }

        public void Start()
        {
            Stop();

            Run();

            Timer = new Timer();
            Timer.Interval = Interval;
            Timer.Elapsed += OnTick;
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        private void OnTick(object sender, EventArgs args)
        {
            try
            {
                OnTick();
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected abstract void OnTick();
    }
}
