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
using JetBrains.Annotations;

namespace ph4n.Net
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Provides data for the asynchronous event handlers that have one result.
    /// </summary>
    /// <typeparam name="T">The type of the result of the asynchronous operation.</typeparam>
    public class AsyncResultEventArgs<T> : AsyncCompletedEventArgs
    {
        /// <summary>
        /// The result of the asynchronous operation.
        /// </summary>
        private T result;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultEventArgs{T}"/> class.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        /// <param name="error">Any error that occurred. Null if no error.</param>
        /// <param name="cancelled">Whether the operation was cancelled.</param>
        /// <param name="userState">The optional user-defined state object.</param>
        public AsyncResultEventArgs(T result, Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            this.result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultEventArgs{T}"/> class indicating a successful completion.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        public AsyncResultEventArgs(T result)
            : this(result, null, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultEventArgs{T}"/> class indicating an unsuccessful operation.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        public AsyncResultEventArgs(Exception error)
            : this(default(T), error, false, null)
        {
        }

        /// <summary>
        /// Gets the result of the asynchronous operation. This property may only be read if <see cref="AsyncCompletedEventArgs.Error"/> is null.
        /// </summary>
        public T Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return this.result;
            }
        }
    }
}

#if NET_45_OR_GREATER

namespace ph4n.Net
{
    using ph4n.Common;

    internal sealed class SocketAwaitable : INotifyCompletion
    {
        private readonly static Action SENTINEL = () => { };

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

        public SocketAwaitable GetAwaiter() { return this; }

        public bool IsCompleted { get { return m_wasCompleted; } }

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
                throw new SocketException((int)m_eventArgs.SocketError);
        }
    }

    internal sealed class ReceiveSocketAwaitable  : INotifyCompletion
    {
        private readonly static Action SENTINEL = () => { };

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

        public ReceiveSocketAwaitable GetAwaiter() { return this; }

        public bool IsCompleted { get { return m_wasCompleted; } }

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
                throw new SocketException((int)m_eventArgs.SocketError);
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
            while ((await _socket.ReceiveAsync(_receiveAwaitable)) > 0) { }
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
        public static ReceiveSocketAwaitable CreateReceiveSocketAwaitable([NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            return new ReceiveSocketAwaitable(SaeaFactory.CreateSaea(complete));
        }
    }

#endif

    internal static class SaeaFactory
    {
        [NotNull]
        public static SocketAsyncEventArgs CreateSaea([NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            Validate.ArgumentNotNull(complete, "complete");
            var args = new SocketAsyncEventArgs();
            args.Completed += complete;
            return args;
        }
    }

    public abstract class AsyncSocket
    {
        internal abstract Socket UnderlyingSocket { get; }
    }

    public static class SocketOps
    {
        public static void Bind(this AsyncSocket asynsSocket, IPEndPoint bindTo)
        {
            asynsSocket.UnderlyingSocket.Bind(bindTo);
        }
    }

    internal class AsyncTcpSocket35 : AsyncSocket
    {
        private Socket _socket;
        private SocketAsyncEventArgs _sendArgs;
        private SocketAsyncEventArgs _receiveArgs;
        private SocketAsyncEventArgs _acceptArgs;

        public EventHandler<AsyncResultEventArgs<byte[]>> bytesArrived;
        public EventHandler<AsyncResultEventArgs<byte[]>> bytesSent;
        public EventHandler<AsyncResultEventArgs<AsyncTcpSocket>> connectArrived;
        public EventHandler<AsyncCompletedEventArgs> shutdownComplete;

        internal override Socket UnderlyingSocket { get { return _socket; } }

        private AsyncTcpSocket35(Socket socket)
        {
            _socket = socket;
            InitAsyncArgs();
            ContinueReceiving();
        }

        public AsyncTcpSocket35(SocketInformation socketInformation)
        {
            _socket = new Socket(socketInformation);
            InitAsyncArgs();
        }

        public AsyncTcpSocket35(SocketType socketType)
        {
            _socket = new Socket(socketType, ProtocolType.Tcp);
            InitAsyncArgs();
        }

        public AsyncTcpSocket35(AddressFamily addressFamily, SocketType socketType)
        {
            _socket = new Socket(addressFamily, socketType, ProtocolType.Tcp);
            InitAsyncArgs();
        }

        private void InitAsyncArgs()
        {
            _acceptArgs = SaeaFactory.CreateSaea(SocketAcceptComplete);
            _sendArgs = SaeaFactory.CreateSaea(SocketSendComplete);
            _receiveArgs = SaeaFactory.CreateSaea(SocketReceiveComplete);
        }

        public void Connect(IPAddress address, int port)
        {
            Connect(new IPEndPoint(address, port));
        }

        public void Connect(EndPoint remoteEP)
        {
            var a = SaeaFactory.CreateSaea(SocketConnectedComplete);
            a.RemoteEndPoint = remoteEP;
            if (!_socket.ConnectAsync(a))
            {
                SocketConnectedComplete(this, a);
            }
        }

        private void ContinueReceiving()
        {
            if (!_socket.ReceiveAsync(_receiveArgs))
            {
                SocketReceiveComplete(this, _receiveArgs);
            }
        }

        protected void SocketConnectedComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        protected void SocketReceiveComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        protected void SocketSendComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        protected void SocketAcceptComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        public void Send(byte[] bytes)
        {
            if (!_socket.SendAsync(_sendArgs))
            {
                SocketSendComplete(this, _sendArgs);
            }
        }
    }
}
