using System;
using System.Threading;
using JetBrains.Annotations;
using ph4n.Common;

namespace ph4n.Threading
{
    public static class CreateTimer
    {
        /// <summary>
        /// Sets a timer for the specified time. If the time has passed it will use the same time tomorrow.
        /// </summary>
        /// <param name="callback">function to call when timer goes off</param>
        /// <param name="time">Time to call function</param>
        /// <param name="period">Milliseconds between invoking callback (Timeout.Infinite to disable periodic signaling)</param>
        /// <returns>The timer that is set</returns>
        [NotNull]
        public static Timer For([NotNull] TimerCallback callback, DateTime time, int period = Timeout.Infinite)
        {
            Validate.ArgumentNotNull(callback, "callback");

            var ret = new Timer(callback);

            DateTime now = DateTime.Now;

            // If it's already past [time], wait until [time] tomorrow    
            if (now > time)
            {
                time = time.AddDays(1.0);
            }

            int msUntilTime = (int)((time - now).TotalMilliseconds);

            // Set the timer to elapse at [time].
            ret.Change(msUntilTime, period);

            return ret;
        }

        /// <summary>
        /// Sets a timer for the specified time. If the time has passed it will use the same time tomorrow.
        /// </summary>
        /// <param name="callback">action to call when timer goes off</param>
        /// <param name="time">Time to call function</param>
        /// <param name="period">Milliseconds between invoking callback (Timeout.Infinite to disable periodic signaling)</param>
        /// <returns>The timer that is set</returns>
        [NotNull]
        public static Timer For([NotNull] Action callback, DateTime time, int period = Timeout.Infinite)
        {
            Validate.ArgumentNotNull(callback, "callback");
            return For(state => callback(), time, period);
        }

        /// <summary>
        /// Sets a timer for the specified timespan. Timer will be invoked after timespan has elapsed
        /// </summary>
        /// <param name="callback">Function To call when timer goes off</param>
        /// <param name="fromNow">Initial timespan to invoke the function after</param>
        /// <param name="period">Milliseconds between invoking callback (Timeout.Infinite to disable periodic signaling)</param>
        /// <returns>The timer that is set</returns>
        [NotNull]
        public static Timer For([NotNull] TimerCallback callback, TimeSpan fromNow, int period = Timeout.Infinite)
        {
            Validate.ArgumentNotNull(callback, "callback");

            var ret = new Timer(callback);

            int msUntilTime = (int)(fromNow.TotalMilliseconds);

            // Set the timer to elapse at [time].
            ret.Change(msUntilTime, period);

            return ret;
        }

        /// <summary>
        /// Sets a timer for the specified timespan. Timer will be invoked after timespan has elapsed
        /// </summary>
        /// <param name="callback">Action To call when timer goes off</param>
        /// <param name="fromNow">Initial timespan to invoke the function after</param>
        /// <param name="period">Milliseconds between invoking callback (Timeout.Infinite to disable periodic signaling)</param>
        /// <returns>The timer that is set</returns>
        [NotNull]
        public static Timer For( [NotNull] Action callback, TimeSpan fromNow, int period = Timeout.Infinite)
        {
            Validate.ArgumentNotNull(callback, "callback");
            return For(state => callback(), fromNow, period);
        }
    }
}
