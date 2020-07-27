using System;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Events.EventArgs;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.World
{
    public class GameDate
    {
        private GameEventQueue _eventQueue;

        private float _time;
        private TimeSpan _dayLength;
        private bool _timeIsFlowing;
        private TimePhase _lastTimePhase;

        public TimePhase TimePhase
        {
            get
            {
                if (_time >= Normalise(1, 0) && _time < Normalise(5, 0))
                    return TimePhase.EarlyMorning;
                else if (_time >= Normalise(5, 0) && _time < Normalise(11, 0))
                    return TimePhase.LateMorning;
                else if (_time >= Normalise(11, 0) && _time < Normalise(13, 0))
                    return TimePhase.Noon;
                else if (_time >= Normalise(13, 0) && _time < Normalise(17, 0))
                    return TimePhase.Afternoon;
                else if (_time >= Normalise(17, 0) && _time < Normalise(20, 0))
                    return TimePhase.EarlyEvening;
                else if (_time >= Normalise(20, 0) && _time < Normalise(23, 0))
                    return TimePhase.LateEvening;
                else
                    return TimePhase.Midnight;
            }
        }

        public float TimeScale => _time;

        public (int, int) Time
        {
            get
            {
                var normalised = _time * 24f;
                var fraction = normalised - Math.Floor(normalised);
                var hour = (int) Math.Floor(normalised);
                var minute = (int) (fraction * 60f);
                return (hour, minute);
            }
        }

        public Day Day { get; private set; }

        public GameDate(Services services)
        {
            _eventQueue = services.Get<GameEventQueue>();
            _time = 0;
            SetDayLength(TimeSpan.FromSeconds(24));
            _timeIsFlowing = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!_timeIsFlowing)
                return;

            var increment = 1f / (float)_dayLength.TotalSeconds;
            _time += increment * gameTime.GetElapsedSeconds();

            var newTimePhase = TimePhase;
            if (TimePhase != _lastTimePhase)
            {
                _eventQueue.Notify(GameEvents.TimePhaseChanged, new TimeEventArgs(this, newTimePhase));
            }
            _lastTimePhase = TimePhase;

            if (_time > 1f)
                NextDay();
        }

        private void NextDay()
        {
            _time -= 1f;
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

            _eventQueue.Notify(GameEvents.DayChanged, new DayEventArgs(this, Day));
        }

        private float Normalise(int hour, int minute)
        {
            return (hour + (minute / 60f)) / 24f;
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
            _time = Normalise(hour, minute);
        }

        public void SetDay(Day day)
        {
            Day = day;
        }
    }

    public enum TimePhase
    {
        Midnight,
        EarlyMorning,
        LateMorning,
        Noon,
        Afternoon,
        EarlyEvening,
        LateEvening
    }

    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
