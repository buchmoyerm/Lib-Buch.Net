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

    public interface IAsyncTcpConnection
    {
        /// <summary>
        /// Returns the IP address and port on this side of the connection.
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Returns the IP address and port on the remote side of the connection.
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// True if the Nagle algorithm has been disabled.
        /// </summary>
        /// <remarks>
        /// <para>The default is false. Generally, this should be left to its default value.</para>
        /// </remarks>
        bool NoDelay { get; set; }

        /// <summary>
        /// If and how long a graceful shutdown will be performed in the background.
        /// </summary>
        /// <remarks>
        /// <para>Setting LingerState to enabled with a 0 timeout will make all calls to <see cref="Close"/> act as though <see cref="AbortiveClose"/> was called. Generally, this should be left to its default value.</para>
        /// </remarks>
        LingerOption LingerState { get; set; }

        /// <summary>
        /// Initiates a shutdown operation. Once a shutdown operation is initiated, only the shutdown operation will complete.
        /// </summary>
        /// <remarks>
        /// <para>The shutdown operation will complete by invoking <see cref="ShutdownCompleted"/>.</para>
        /// <para>Shutdown operations are never cancelled.</para>
        /// </remarks>
        void Shutdown();

        /// <summary>
        /// Indicates the completion of a shutdown operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Generally, a shutdown completing with error is handled the same as a shutdown completing successfully: the normal response in both situations is to <see cref="Close"/> the socket.</para>
        /// </remarks>
        event EventHandler<SocketAsyncEventArgs> ShutdownCompleted;

        /// <summary>
        /// Gracefully or abortively closes the socket. Once this method is called, no operations will complete.
        /// </summary>
        /// <remarks>
        /// <para>This method performs a graceful shutdown of the underlying socket; however, this is performed in the background, so the application never receives notification of its completion. <see cref="ShutdownAsync"/> performs a graceful shutdown with completion.</para>
        /// <para>Note that exiting the process after calling this method but before the background shutdown completes will result in an abortive close.</para>
        /// <para><see cref="LingerState"/> will determine whether this method will perform a graceful or abortive close.</para>
        /// </remarks>
        void Close();

        /// <summary>
        /// Abortively closes the socket. Once this method is called, no operations will complete.
        /// </summary>
        /// <remarks>
        /// <para>This method provides the fastest way to reclaim socket resources; however, its use is not generally recommended; <see cref="Close"/> should usually be used instead of this method.</para>
        /// </remarks>
        void AbortiveClose();

        /// <summary>
        /// Sets a flag in the socket to indicate that the close (performed by <see cref="Dispose"/>) should be done abortively.
        /// </summary>
        // Always runs in User thread
        void SetAbortive();
    }

    public interface IAsyncTcpData
    {
        /// <summary>
        /// Indicates the completion of a read operation, either successfully or with error.
        /// </summary>
        event EventHandler<SocketAsyncEventArgs> ReadCompleted;

        /// <overloads>
        /// <summary>
        /// Initiates a write operation.
        /// </summary>
        /// <remarks>
        /// <para>Multiple write operations may be active at the same time.</para>
        /// <para>The write operation will complete by invoking <see cref="WriteCompleted"/>, unless the socket is shut down (<see cref="Shutdown"/>), closed (<see cref="Close"/>), or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        /// </overloads>
        /// <summary>
        /// Initiates a write operation.
        /// </summary>
        /// <remarks>
        /// <para>Multiple write operations may be active at the same time.</para>
        /// <para>The write operation will complete by invoking <see cref="WriteCompleted"/>, unless the socket is shut down (<see cref="ShutdownAsync"/>), closed (<see cref="Close"/>), or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        /// <param name="buffer">The data to write to the socket.</param>
        void Write(byte[] buffer);

        /// <inheritdoc cref="Write(byte[])" />
        /// <remarks>
        /// <inheritdoc cref="WriteA(byte[])" />
        /// </remarks>
        /// <param name="buffer">The data to write to the socket, but only invokes <see cref="WriteCompleted"/> on an error</param>
        void WriteCallbackOnErrorOnly(byte[] buffer);

        /// <summary>
        /// Indicates the completion of a write operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Write operations will not complete if the socket is shut down (<see cref="Shutdown"/>), closed (<see cref="Close"/>), or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>Note that even though a write operation completes, the data may not have been received by the remote end. However, it is still important to handle <see cref="WriteCompleted"/>, because errors may be reported.</para>
        /// <para>If a write operation completes with error, the socket should be closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        event EventHandler<SocketAsyncEventArgs> WriteCompleted;
    }

    public interface IAsyncTcpAccepter
    {
        /// <overloads>
        /// <summary>
        /// Binds to a local endpoint and begins listening and accepting connections.
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Binds to a local endpoint and begins listening and accepting connections.
        /// </summary>
        /// <param name="bindTo">The local endpoint.</param>
        /// <param name="backlog">The number of backlog connections for listening.</param>
        void Listen(IPEndPoint bindTo, int backlog);

        /// <inheritdoc cref="Listen(IPEndPoint, int)" />
        /// <param name="address">The address of the local endpoint.</param>
        /// <param name="port">The port of the local endpoint.</param>
        /// <param name="backlog">The number of backlog connections for listening.</param>
        void Listen(IPAddress address, int port, int backlog);


        /// <inheritdoc cref="Listen(IPEndPoint, int)" />
        /// <param name="bindTo">The local endpoint.</param>
        void Listen(IPEndPoint bindTo);

        /// <inheritdoc cref="Listen(IPEndPoint, int)" />
        /// <param name="address">The address of the local endpoint.</param>
        /// <param name="port">The port of the local endpoint.</param>
        void Listen(IPAddress address, int port);

        /// <inheritdoc cref="Listen(IPEndPoint, int)" />
        /// <param name="port">The port of the local endpoint.</param>
        /// <param name="backlog">The number of backlog connections for listening.</param>
        void Listen(int port, int backlog);

        /// <inheritdoc cref="Listen(IPEndPoint, int)" />
        /// <param name="port">The port of the local endpoint.</param>
        void Listen(int port);

        /// <summary>
        /// Delegate to invoke when the accept operation completes.
        /// </summary>
        EventHandler<SocketAsyncEventArgs> AcceptCompleted { get; set; }
    }

    public interface IAsyncTcpConnecter
    {
        /// <overloads>
        /// <summary>
        /// Initiates a connect operation.
        /// </summary>
        /// <remarks>
        /// <para>There may be only one connect operation for a client socket, and it must be the first operation performed.</para>
        /// <para>The connect operation will complete by invoking <see cref="ConnectCompleted"/>, unless the socket is closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>Connect operations are never cancelled.</para>
        /// </remarks>
        /// </overloads>
        /// <summary>
        /// Initiates a connect operation.
        /// </summary>
        /// <remarks>
        /// <para>There may be only one connect operation for a client socket, and it must be the first operation performed.</para>
        /// <para>The connect operation will complete by invoking <see cref="ConnectCompleted"/>, unless the socket is closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>Connect operations are never cancelled.</para>
        /// </remarks>
        /// <param name="server">The address and port of the server to connect to.</param>
        void Connect(IPEndPoint server);

        /// <inheritdoc cref="Connect(IPEndPoint)" />
        /// <param name="address">The address of the server to connect to.</param>
        /// <param name="port">The port of the server to connect to.</param>
        void Connect(IPAddress address, int port);

        /// <summary>
        /// Indicates the completion of a connect operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Connect operations are never cancelled.</para>
        /// <para>Connect operations will not complete if the socket is closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>A handler of this event should assign handlers to recurring events (<see cref="PacketArrived"/> and <see cref="WriteCompleted"/>) if they do not already have handlers. Otherwise, data and error events may be lost.</para>
        /// <para>If a connect operation completes with error, the socket should be closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        event EventHandler<SocketAsyncEventArgs> ConnectCompleted;
    }

    internal class AsyncTcpConnection : IAsyncTcpConnection
    {
        protected Socket _socket;
        protected Socket UnderlyingSocket { get { return _socket; } }

        private SocketAsyncEventArgs _shutdownArgs;

        internal AsyncTcpConnection(Socket socket)
        {
            _socket = socket;
            InitAsyncArgs();
        }

        public AsyncTcpConnection(SocketInformation socketInformation)
        {
            _socket = new Socket(socketInformation);
            InitAsyncArgs();
        }

        public AsyncTcpConnection(AddressFamily addressFamily, SocketType socketType)
        {
            _socket = new Socket(addressFamily, socketType, ProtocolType.Tcp);
            InitAsyncArgs();
        }

        public IPEndPoint LocalEndPoint { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public bool NoDelay { get; set; }
        public LingerOption LingerState { get; set; }

        protected virtual void InitAsyncArgs()
        {
            _shutdownArgs = SaeaFactory.CreateSaea(SocketShutdownComplete);
        }

        public void Shutdown()
        {
            if (!_socket.DisconnectAsync(_shutdownArgs))
            {
                SocketShutdownComplete(_socket, _shutdownArgs);
            }
        }

        public event EventHandler<SocketAsyncEventArgs> ShutdownCompleted;
        public void Close()
        {
            _socket.Close();
        }

        public void AbortiveClose()
        {
            SetAbortive();   
            _socket.Close();
        }

        /// <summary>
        /// Sets a flag in the socket to indicate that the close (performed by <see cref="Dispose"/>) should be done abortively.
        /// </summary>
        // Always runs in User thread
        public void SetAbortive()
        {
            LingerState = new LingerOption(true, 0);
        }

        protected void SocketShutdownComplete(object sender, SocketAsyncEventArgs args)
        {
            ShutdownCompleted.Raise(this, args);
        }
    }

    internal class AsyncTcpData :AsyncTcpConnection, IAsyncTcpData
    {
        private SocketAsyncEventArgs _writeArgs;
        private SocketAsyncEventArgs _readArgs;
        private SocketAsyncEventArgs _writeCallbackOnErrorOnlyArgs;
        

        internal AsyncTcpData(Socket socket) : base(socket)
        {
        }

        public AsyncTcpData(SocketInformation socketInformation) : base (socketInformation)
        {
        }

        public AsyncTcpData(AddressFamily addressFamily, SocketType socketType) : base (addressFamily, socketType)
        {
        }

        protected override void InitAsyncArgs()
        {
            base.InitAsyncArgs();
            _writeArgs = SaeaFactory.CreateSaea(SocketWriteComplete);
            _readArgs = SaeaFactory.CreateSaea(SocketReadComplete);
            _writeCallbackOnErrorOnlyArgs = SaeaFactory.CreateSaea(SocketWriteComplete);
            _writeCallbackOnErrorOnlyArgs.UserToken = new CallbackOnErrorsOnly();
        }

        public event EventHandler<SocketAsyncEventArgs> ReadCompleted;
        protected void SocketReadComplete(object sender, SocketAsyncEventArgs args)
        {
            ReadCompleted.Raise(this, args);
        }

        public void Write(byte[] buffer)
        {
            if (!UnderlyingSocket.SendAsync(_writeArgs))
            {
                SocketWriteComplete(UnderlyingSocket, _writeArgs);
            }
        }

        public void WriteCallbackOnErrorOnly(byte[] buffer)
        {
            if (!UnderlyingSocket.SendAsync(_writeCallbackOnErrorOnlyArgs))
            {
                SocketWriteComplete(UnderlyingSocket, _writeCallbackOnErrorOnlyArgs);
            }
        }

        public event EventHandler<SocketAsyncEventArgs> WriteCompleted;
        protected void SocketWriteComplete(object sender, SocketAsyncEventArgs args)
        {
            // If there's no error and the user state is CallbackOnErrorsOnly, then don't issue the callback
            if (args.SocketError != SocketError.Success && args.UserToken is CallbackOnErrorsOnly)
                return;
            WriteCompleted.Raise(this, args);
        }
    }

    internal class AsyncTcpSocket : TcpSocketOld
    {
        private Socket _socket;
        private SocketAsyncEventArgs _sendArgs;
        private SocketAsyncEventArgs _receiveArgs;

        public EventHandler<AsyncResultEventArgs<byte[]>> bytesArrived;
        public EventHandler<AsyncResultEventArgs<byte[]>> bytesSent;
        public EventHandler<AsyncCompletedEventArgs> shutdownComplete;

        internal override Socket UnderlyingSocket { get { return _socket; } }

        private AsyncTcpSocket(Socket socket)
        {
            _socket = socket;
            InitAsyncArgs();
            StartReceiving();
        }

        public AsyncTcpSocket(SocketInformation socketInformation)
        {
            _socket = new Socket(socketInformation);
            InitAsyncArgs();
        }

        public AsyncTcpSocket(AddressFamily addressFamily, SocketType socketType)
        {
            _socket = new Socket(addressFamily, socketType, ProtocolType.Tcp);
            InitAsyncArgs();
        }

        private void InitAsyncArgs()
        {
            _sendArgs = SaeaFactory.CreateSaea(SocketSendComplete);
            _receiveArgs = SaeaFactory.CreateSaea(SocketReceiveComplete);
        }

        //public void Connect(IPAddress address, int port)
        //{
        //    Connect(new IPEndPoint(address, port));
        //}

        //public void Connect(EndPoint remoteEP)
        //{
        //    var a = SaeaFactory.CreateSaea(SocketConnectedComplete);
        //    a.RemoteEndPoint = remoteEP;
        //    if (!_socket.ConnectAsync(a))
        //    {
        //        SocketConnectedComplete(this, a);
        //    }
        //}

        public override void StartReceiving()
        {
            ContinueReceiving();
        }

        private void ContinueReceiving()
        {
            if (!_socket.ReceiveAsync(_receiveArgs))
            {
                SocketReceiveComplete(this, _receiveArgs);
            }
        }

        protected void SocketAcceptComplete(object sender, SocketAsyncEventArgs e)
        {

        }

        public override void Send(byte[] bytes)
        {
            if (!_socket.SendAsync(_sendArgs))
            {
                SocketSendComplete(this, _sendArgs);
            }
        }
    }
}
