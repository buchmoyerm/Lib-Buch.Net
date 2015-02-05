using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;


namespace ph4n.Net45
{
    using ph4n.Net;
    using ph4n.Common;
    using JetBrains.Annotations;

    internal sealed class SocketAwaitable : INotifyCompletion
    {
        private static readonly Action SENTINEL = () => { };

        internal bool m_wasCompleted;
        internal Action m_continuation;
        internal SocketAsyncEventArgs m_eventArgs;

        public SocketAwaitable(SocketAsyncEventArgs eventArgs)
        {
            Validate.ArgumentNotNull(eventArgs, "eventArgs");
            m_eventArgs = eventArgs;
            eventArgs.Completed += delegate
            {
                var prev = m_continuation ?? Interlocked.CompareExchange(
                    ref m_continuation, SENTINEL, null);
                if (prev != null) prev();
            };
        }

        internal void Reset()
        {
            m_wasCompleted = false;
            m_continuation = null;
        }

        public SocketAwaitable GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get { return m_wasCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            if (m_continuation == SENTINEL ||
                Interlocked.CompareExchange(
                    ref m_continuation, continuation, null) == SENTINEL)
            {
                Task.Run(continuation);
            }
        }

        public void GetResult()
        {
            if (m_eventArgs.SocketError != SocketError.Success)
                throw new SocketException((int) m_eventArgs.SocketError);
        }
    }

    internal sealed class ReceiveSocketAwaitable : INotifyCompletion
    {
        private static readonly Action SENTINEL = () => { };

        internal bool m_wasCompleted;
        internal Action m_continuation;
        internal SocketAsyncEventArgs m_eventArgs;

        public ReceiveSocketAwaitable(SocketAsyncEventArgs eventArgs)
        {
            Validate.ArgumentNotNull(eventArgs, "eventArgs");
            m_eventArgs = eventArgs;
            eventArgs.Completed += delegate
            {
                var prev = m_continuation ?? Interlocked.CompareExchange(
                    ref m_continuation, SENTINEL, null);
                if (prev != null) prev();
            };
        }

        internal void Reset()
        {
            m_wasCompleted = false;
            m_continuation = null;
        }

        public ReceiveSocketAwaitable GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get { return m_wasCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            if (m_continuation == SENTINEL ||
                Interlocked.CompareExchange(
                    ref m_continuation, continuation, null) == SENTINEL)
            {
                Task.Run(continuation);
            }
        }

        public int GetResult()
        {
            if (m_eventArgs.SocketError != SocketError.Success)
                throw new SocketException((int) m_eventArgs.SocketError);
            return m_eventArgs.BytesTransferred;
        }
    }


    internal static class SocketExtensions
    {
        public static SocketAwaitable ConnectAsync(this Socket socket,
            SocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.ConnectAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static SocketAwaitable ReceiveAsync(this Socket socket,
            SocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.ReceiveAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static SocketAwaitable SendAsync(this Socket socket,
            SocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.SendAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static SocketAwaitable AcceptAsync(this Socket socket,
            SocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.AcceptAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static ReceiveSocketAwaitable ConnectAsync(this Socket socket,
            ReceiveSocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.ConnectAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static ReceiveSocketAwaitable ReceiveAsync(this Socket socket,
            ReceiveSocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.ReceiveAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static ReceiveSocketAwaitable SendAsync(this Socket socket,
            ReceiveSocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.SendAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }

        public static ReceiveSocketAwaitable AcceptAsync(this Socket socket,
            ReceiveSocketAwaitable awaitable)
        {
            awaitable.Reset();
            if (!socket.AcceptAsync(awaitable.m_eventArgs))
                awaitable.m_wasCompleted = true;
            return awaitable;
        }
    }

    internal class AsyncTcpSocket
    {
        private Socket _socket;
        private SocketAwaitable _sendAwaitable;
        private ReceiveSocketAwaitable _receiveAwaitable;
        private SocketAwaitable _acceptAwaitable;

        public EventHandler<AsyncResultEventArgs<byte[]>> bytesArrived;
        public EventHandler<AsyncResultEventArgs<byte[]>> bytesSent;
        public EventHandler<AsyncResultEventArgs<AsyncTcpSocket>> connectArrived;
        public EventHandler<AsyncCompletedEventArgs> shutdownComplete;

        private AsyncTcpSocket(Socket socket)
        {
            _socket = socket;
            InitAwaitables();
            StartReceiving();
        }

        public AsyncTcpSocket(SocketInformation socketInformation)
        {
            _socket = new Socket(socketInformation);
            InitAwaitables();
        }

        public AsyncTcpSocket(SocketType socketType)
        {
            _socket = new Socket(socketType, ProtocolType.Tcp);
            InitAwaitables();
        }

        public AsyncTcpSocket(AddressFamily addressFamily, SocketType socketType)
        {
            _socket = new Socket(addressFamily, socketType, ProtocolType.Tcp);
            InitAwaitables();
        }

        private void InitAwaitables()
        {
            _acceptAwaitable = SocketAwaitableFactory.CreateSocketAwaitable(SocketAcceptComplete);
            _sendAwaitable = SocketAwaitableFactory.CreateSocketAwaitable(SocketSendComplete);
            _receiveAwaitable = SocketAwaitableFactory.CreateReceiveSocketAwaitable(SocketReceiveComplete);
        }

        public void Bind(IPEndPoint bindTo)
        {
            _socket.Bind(bindTo);
        }

        public void Connect(EndPoint remoteEP)
        {
            var a = SocketAwaitableFactory.CreateSocketAwaitable(SocketConnectedComplete);
            a.m_eventArgs.RemoteEndPoint = remoteEP;
            _socket.ConnectAsync(a);
        }

        public void Connect(IPAddress address, int port)
        {
            Connect(new IPEndPoint(address, port));
        }

        private async Task StartReceiving()
        {
            while ((await _socket.ReceiveAsync(_receiveAwaitable)) > 0)
            {
            }
            shutdownComplete.Raise(this, new AsyncCompletedEventArgs(null, false, null));
        }

        private void SocketConnectedComplete(object sender, SocketAsyncEventArgs e)
        {
            StartReceiving();
        }

        private void SocketReceiveComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        private void SocketSendComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        private void SocketAcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            _socket.AcceptAsync(_acceptAwaitable);
        }

        public void Send(byte[] bytes)
        {
            _socket.SendAsync(_sendAwaitable);
        }
    }

    internal static class SocketAwaitableFactory
    {
        [NotNull]
        public static SocketAwaitable CreateSocketAwaitable([NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            return new SocketAwaitable(SaeaFactory.CreateSaea(complete));
        }

        [NotNull]
        public static ReceiveSocketAwaitable CreateReceiveSocketAwaitable(
            [NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            return new ReceiveSocketAwaitable(SaeaFactory.CreateSaea(complete));
        }
    }
}

 