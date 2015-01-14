using System;
using JetBrains.Annotations;

namespace ph4n.Common
{
    /// <summary>
    /// Clever way to handle events using extentions
    /// http://stackoverflow.com/a/340638/351028
    /// </summary>
    public static class EventHandlerEx
    {
        /// <summary>
        /// Will raise an event handler in a thread safe way with a null check
        /// </summary>
        /// <param name="handler">Handler to be raised</param>
        /// <param name="sender">sender raising the event</param>
        /// <param name="e">EventArgs event is being raised with</param>
        public static void Raise([CanBeNull] this EventHandler handler, [CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            if (null != handler) { handler(sender, e); }
        }

        /// <summary>
        /// Will raise an event handler in a thread safe way with a null check
        /// </summary>
        /// <typeparam name="TEventArgs">Type of EventArgs event is being raised with</typeparam>
        /// <param name="handler">Handler to be raised</param>
        /// <param name="sender">sender raising the event</param>
        /// <param name="e">EventArgs event is being raised with</param>
        public static void Raise<TEventArgs>([CanBeNull] this EventHandler<TEventArgs> handler, [CanBeNull] object sender, [CanBeNull] TEventArgs e) where TEventArgs : EventArgs
        {
            if (null != handler) { handler(sender, e); }
        }
    }
}
