using System;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;
using PhotoVs.Logic.Events.EventArgs;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    // todo: reimplement as service
    public class GameDate : IHasUpdate
    {
        private TimeSpan _dayLength;
        private TimePhase _lastTimePhase;
        private readonly ISignal _signal;

        private bool _timeIsFlowing;
        public bool TimeFlowing => _timeIsFlowing;

        public GameDate(ISignal signal)
        {
            _signal = signal;
            TimeScale = 0;
            SetDayLength(TimeSpan.FromSeconds(24));
            _timeIsFlowing = true;
        }

        public TimePhase TimePhase
        {
            get
            {
                if (TimeScale >= Normalise(1, 0) && TimeScale < Normalise(5, 0))
                    return TimePhase.EarlyMorning;
                if (TimeScale >= Normalise(5, 0) && TimeScale < Normalise(11, 0))
                    return TimePhase.LateMorning;
                if (TimeScale >= Normalise(11, 0) && TimeScale < Normalise(13, 0))
                    return TimePhase.Noon;
                if (TimeScale >= Normalise(13, 0) && TimeScale < Normalise(17, 0))
                    return TimePhase.Afternoon;
                if (TimeScale >= Normalise(17, 0) && TimeScale < Normalise(20, 0))
                    return TimePhase.EarlyEvening;
                if (TimeScale >= Normalise(20, 0) && TimeScale < Normalise(23, 0))
                    return TimePhase.LateEvening;
                return TimePhase.Midnight;
            }
        }

        public float TimeScale { get; private set; }

        public (int, int) Time
        {
            get
            {
                var normalised = TimeScale * 24f;
                var fraction = normalised - Math.Floor(normalised);
                var hour = (int) Math.Floor(normalised);
                var minute = (int) (fraction * 60f);
                return (hour, minute);
            }
        }

        public Day Day { get; private set; }

        public int UpdatePriority { get; set; } = -999;
        public bool UpdateEnabled { get; set; } = true;

        public void Update(GameTime gameTime)
        {
            if (!_timeIsFlowing)
                return;

            var increment = 1f / (float) _dayLength.TotalSeconds;
            TimeScale += increment * gameTime.GetElapsedSeconds();

            var newTimePhase = TimePhase;
            if (TimePhase != _lastTimePhase) _signal.Notify("TimePhaseChanged", new TimeEventArgs(this, newTimePhase));
            _lastTimePhase = TimePhase;

            if (TimeScale > 1f)
                NextDay();
        }

        private void NextDay()
        {
            TimeScale -= 1f;
            Day = Day switch
            {
                Day.Monday => Day.Tuesday,
                Day.Tuesday => Day.Wednesday,
                Day.Wednesday => Day.Thursday,
                Day.Thursday => Day.Friday,
                Day.Friday => Day.Saturday,
                Day.Saturday => Day.Sunday,
                Day.Sunday => Day.Monday,
                _ => Day
            };

            _signal.Notify("DayChanged", new DayEventArgs(this, Day));
        }

        private float Normalise(int hour, int minute)
        {
            return (hour + minute / 60f) / 24f;
        }

        public void EnableTimeFlow()
        {
            _timeIsFlowing = true;
        }

        public void DisableTimeFlow()
        {
            _timeIsFlowing = false;
        }

        public void SetDayLength(TimeSpan dayLength)
        {
            _dayLength = dayLength;
        }

        public void SetTime(int hour, int minute)
        {
            TimeScale = Normalise(hour, minute);
        }

        public void SetDay(Day day)
        {
            Day = day;
        }
    }
}