using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
    //class TimerCountDown : Timer {

    //    private float _countFrom;

    //    public TimerCountDown(Func<float> funcTime) : base(funcTime) { }

    //    public void Start(float countFrom) {
    //        _countFrom = countFrom;
    //        base.Start();
    //    }
    //    public override float GetTime() {
    //        float time = _countFrom - (FuncGetTime() - timeStart);
    //        if (time < 0) {
    //            Stop();
    //            time = 0f;
    //        }
    //        return time;
    //    }
    //}

    //class TimerCountUp : Timer {

    //    private float _countTo;

    //    public TimerCountUp(Func<float> funcTime) : base(funcTime) { }

    //    public void Start(float countTo) {
    //        base.Start();
    //        _countTo = countTo + base.GetTime();
    //    }
    //    public override float GetTime() {

    //        float time = base.GetTime();


    //        if (time >= _countTo) {
    //            Stop();
    //            time = _countTo;
    //        }

    //        return time;

    //    }

    //}

    class Timer {
        public bool IsStop { get; protected set; }
        protected float timeStart;
        protected float timeStop;

        public Timer(Func<float> timeFunction) {
            FuncGetTime = timeFunction;
        }

        public int Seconds {
            get {
                return GetSeconds(GetTime());
            }
        }
        public int Minutes {
            get {
                return GetMinutes(GetTime());
            }
        }
        public int Hours {
            get {
                return GetHours(GetTime());
            }
        }
        protected Func<float> FuncGetTime;

        public static int GetMinutes(float time) {
            return (int)(time / 60) % 60;
        }
        public static int GetSeconds(float time) {
            return (int)(time % 60);
        }
        public static int GetHours(float time) {
            return (int)(time / 3600) % 24;
        }

        public void Start() {
            timeStart = FuncGetTime();
            IsStop = false;
        }
        public void Stop() {

            if (!IsStop) {
                
                timeStop = GetTime();
                IsStop = true;
            }
        }
        public void Reset() {
            timeStart = 0f;
            IsStop = true;
        }
        public virtual float GetTime() {

            if (!IsStop) {
                return FuncGetTime() - timeStart;
            }
            else {
                return timeStop;
            }

            
        }
        public string To_hhmmss() {
            return Hours.ToString("00") + ":" + Minutes.ToString("00") + ":" + Seconds.ToString("00");
        }
    }
}
