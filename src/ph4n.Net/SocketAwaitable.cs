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
using ph4n.Common;

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

    public static class SaeaFactory
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
        public EventHandler<AsyncResultEventArgs<AsyncSocket>> connectArrived;
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
