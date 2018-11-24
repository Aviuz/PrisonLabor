using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    public class SimpleTimer : IExposable
    {
        private bool _IsActive;
        public bool IsActive
        {
            get => _IsActive;
            set => _IsActive = value;
        }

        private int _Ticks;
        public int Ticks
        {
            get => _Ticks;
            set => _Ticks = value;
        }

        public void Start() => IsActive = true;

        public void Stop() => IsActive = false;

        public void Reset() => Ticks = 0;

        public void Tick()
        {
            if (_IsActive)
            {
                Ticks++;
            }
        }

        public void ResetAndStop()
        {
            IsActive = false;
            Ticks = 0;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<bool>(ref _IsActive, nameof(IsActive));
            Scribe_Values.Look<int>(ref _Ticks, nameof(Ticks));
        }
    }
}
