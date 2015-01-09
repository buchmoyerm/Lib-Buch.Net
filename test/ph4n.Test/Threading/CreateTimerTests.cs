using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Threading;

namespace ph4n.Test.Threading
{
    [TestClass]
    public class CreateTimerTests
    {
        private ManualResetEvent _timeElapsed;

        [TestInitialize]
        public void InitManualResetEvent()
        {
            _timeElapsed = new ManualResetEvent(false);    
        }

        private void SetAction()
        {
            _timeElapsed.Set();
        }

        private void SetCallBack(object state)
        {
            SetAction();
        }

        [TestMethod]
        public void For_Action_TimeSpan()
        {
            var timer = CreateTimer.For(SetAction, TimeSpan.FromMilliseconds(30));
            Assert.IsTrue(_timeElapsed.WaitOne(TimeSpan.FromMilliseconds(50)), "Timer did not fire");
        }

        [TestMethod]
        public void For_Action_DateTime()
        {
            var timer = CreateTimer.For(SetAction, DateTime.Now.AddMilliseconds(30));
            Assert.IsTrue(_timeElapsed.WaitOne(TimeSpan.FromMilliseconds(50)), "Timer did not fire");
        }

        [TestMethod]
        public void For_callback_TimeSpan()
        {
            var timer = CreateTimer.For(SetCallBack, TimeSpan.FromMilliseconds(30));
            Assert.IsTrue(_timeElapsed.WaitOne(TimeSpan.FromMilliseconds(50)), "Timer did not fire");
        }

        [TestMethod]
        public void For_callback_DateTime()
        {
            var timer = CreateTimer.For(SetCallBack, DateTime.Now.AddMilliseconds(30));
            Assert.IsTrue(_timeElapsed.WaitOne(50), "Timer did not fire");
        }

        [TestMethod]
        public void For_Action_TimeSpan_Repeat()
        {
            var timer = CreateTimer.For(SetAction, TimeSpan.FromMilliseconds(30), 30);
            Assert.IsTrue(_timeElapsed.WaitOne(TimeSpan.FromMilliseconds(50)), "Timer missed initial fire");
            _timeElapsed.Reset();
            Assert.IsTrue(_timeElapsed.WaitOne(TimeSpan.FromMilliseconds(50)), "Timer missed repeat");
        }

        [TestMethod]
        public void For_Action_DateTime_Repeat()
        {
            var timer = CreateTimer.For(SetAction, DateTime.Now.AddMilliseconds(30), 30);
            Assert.IsTrue(_timeElapsed.WaitOne(50), "Timer missed initial fire");
            _timeElapsed.Reset();
            Assert.IsTrue(_timeElapsed.WaitOne(50), "Timer missed repeate");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void For_action_null_throws_ArgumentNullException()
        {
            Action action = null;
            var timer = CreateTimer.For(action, TimeSpan.FromMilliseconds(10));
        }
    }
}
