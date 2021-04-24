using System.Threading;

namespace Tetris
{
    public class Clock
    {
        public static readonly int SECOND_MILI = 2000;
        private readonly Timer _timer;
        private int _bps;

        public Clock(int bps, TimerCallback e)
        {
            _timer = new Timer(e, null, SECOND_MILI / bps, SECOND_MILI / bps);
            _bps = bps;
        }
        
        public void UpdateInterval(int newBPS)
        {
        	_timer.Change(SECOND_MILI / newBPS, SECOND_MILI / newBPS);
            _bps = newBPS;
        }

        public void End()
        {
            _timer.Dispose();
        }
    }
}
