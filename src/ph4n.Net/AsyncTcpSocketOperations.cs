using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using ph4n.Common;
using ph4n.Containers;

//
// See this http://stackoverflow.com/a/17094024/351028
// 



//
// I really like how well IAsyncTcpOperations and IAsyncTcpOperationsEx compliment each other
// The idea here is that I can create classes that implement IAsyncTcpOperations using different async
// techniques (i.e. Begin/End methods, Async methods, and Awaitable tasks) in the smallest
// way possible. This interface can then be wrapped into a very basic socket class that 
// will expose only the necessary components publicly. The basic socket class can also take
// a protocol class that will could handle continuously reading from the socket and framing
// the messages.
//
// I also like the idea of the IAsyncTcpOperations being the most basic of operations and OnComplete
// EventHandlers. This is allows for a more intelligent socket to wrap up multiple types of async
// types. For instance a class BasicSocket could handle the async read call back by automatically
// invoking ReadAsync() on the async operations.
//
// What I don't like is how ugly the pooling ended up being. I had to use the SocketAsyncEventArgs' UserToken
// store a reference to the pooled object wrapper so that the EventArg could be recycled for use later.
// I'm also not sure that they pooled technique is needed for more than just the write args.
// Writing is the only one that could be happening from multiple threads at once and is the only one where
// multiple args would be needed. The other args could just have boolean flags indicating that the
// socket is already doing the async operation. (Reading from multiple threads would also destroy the order
// of the bytes received).
//

namespace ph4n.Net
{
    public static class SaeaFactory
    {
        [NotNull]
        public static SocketAsyncEventArgs CreateSaea([NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            Validate.ArgumentNotNull(complete, "complete");
            var args = new CustomSocketAsyncEventArgs();
            args.Completed += complete;
            return args;
        }

        [NotNull]
        public static SocketAsyncEventArgs CreateCallbackOnErrorOnlySaea(
            [NotNull] EventHandler<SocketAsyncEventArgs> complete)
        {
            var args = (CustomSocketAsyncEventArgs) CreateSaea(complete);
            args.CallbackOnErrorOnly = true;
            return args;
        }
    }

    internal class CustomSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public CustomSocketAsyncEventArgs()
        {
            CallbackOnErrorOnly = false;
        }

        public bool CallbackOnErrorOnly { get; set; }

        public void DisposeToken()
        {
            var d = this.UserToken as IDisposable;
            UserToken = null;
            if (d != null)
            {
                d.Dispose();
            }
        }
    }

    /// <summary>
    /// basic Async socket operations and callbacks
    /// Any implementation of this should do no more than
    /// perform the operation invoke the complete event when
    /// it has completed
    /// </summary>
    internal interface IAsyncTcpOperations
    {
        Socket Socket { get; }

        #region accepter operations
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
        void ListenAsync(IPEndPoint bindTo, int backlog);

        /// <summary>
        /// Delegate to invoke when the accept operation completes.
        /// </summary>
        EventHandler<AsyncResultEventArgs<IAsyncTcpOperations>> AcceptCompleted { get; set; }
        #endregion accepter operations

        #region initiator operations

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
        void ConnectAsync(IPEndPoint server);

        /// <summary>
        /// Indicates the completion of a connect operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Connect operations will not complete if the socket is closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>A handler of this event should assign handlers to recurring events (<see cref="PacketArrived"/> and <see cref="WriteCompleted"/>) if they do not already have handlers. Otherwise, data and error events may be lost.</para>
        /// <para>If a connect operation completes with error, the socket should be closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        event EventHandler<AsyncCompletedEventArgs> ConnectCompleted;

        #endregion initiator operations

        #region shutdown operations

        /// <summary>
        /// Initiates a shutdown operation. Once a shutdown operation is initiated, only the shutdown operation will complete.
        /// </summary>
        /// <remarks>
        /// <para>The shutdown operation will complete by invoking <see cref="ShutdownCompleted"/>.</para>
        /// <para>Shutdown operations are never cancelled.</para>
        /// </remarks>
        /// <param name="reuseSocket">sets the resuse on disconnect parameter</param>
        void ShutdownAsync(bool reuseSocket);

        /// <summary>
        /// Indicates the completion of a shutdown operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Generally, a shutdown completing with error is handled the same as a shutdown completing successfully: the normal response in both situations is to <see cref="Close"/> the socket.</para>
        /// </remarks>
        event EventHandler<AsyncCompletedEventArgs> ShutdownCompleted;
        #endregion shutdown operations

        #region Read operations
        /// <summary>
        /// Indicates the completion of a read operation, either successfully or with error.
        /// </summary>
        event EventHandler<AsyncResultEventArgs<byte[]>> ReadCompleted;

        void ReadAsync();
        #endregion

        #region write operations
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
        void WriteAsync(byte[] buffer);

        /// <inheritdoc cref="Write(byte[])" />
        /// <remarks>
        /// <inheritdoc cref="WriteA(byte[])" />
        /// </remarks>
        /// <param name="buffer">The data to write to the socket, but only invokes <see cref="WriteCompleted"/> on an error</param>
        void WriteAsyncCallbackOnErrorOnly(byte[] buffer);

        /// <summary>
        /// Indicates the completion of a write operation, either successfully or with error.
        /// </summary>
        /// <remarks>
        /// <para>Write operations will not complete if the socket is shut down (<see cref="Shutdown"/>), closed (<see cref="Close"/>), or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// <para>Note that even though a write operation completes, the data may not have been received by the remote end. However, it is still important to handle <see cref="WriteCompleted"/>, because errors may be reported.</para>
        /// <para>If a write operation completes with error, the socket should be closed (<see cref="Close"/>) or abortively closed (<see cref="AbortiveClose"/>).</para>
        /// </remarks>
        event EventHandler<AsyncCompletedEventArgs> WriteCompleted;
        #endregion write operations
    }

    internal static class IAsyncTcpOperationsEx
    {
        private const int DefaultBacklog = 20;

        /// <inheritdoc cref="IAsyncTcpOperations.Listen(IPEndPoint, int)" />
        /// <param name="bindTo">The local endpoint.</param>
        public static void ListenAsync(this IAsyncTcpOperations socketOp, IPEndPoint bindTo)
        {
            socketOp.ListenAsync(bindTo, DefaultBacklog);
        }

        /// <inheritdoc cref="IAsyncTcpOperations.Listen(IPEndPoint, int)" />
        /// <param name="address">The address of the local endpoint.</param>
        /// <param name="port">The port of the local endpoint.</param>
        public static void ListenAsync(this IAsyncTcpOperations socketOp, IPAddress address, int port)
        {
            socketOp.ListenAsync(new IPEndPoint(address, port), DefaultBacklog);
        }

        /// <inheritdoc cref="IAsyncTcpOperations.Listen(IPEndPoint, int)" />
        /// <param name="port">The port of the local endpoint.</param>
        /// <param name="backlog">The number of backlog connections for listening.</param>
        public static void ListenAsync(this IAsyncTcpOperations socketOp, int port, int backlog)
        {
            socketOp.ListenAsync(new IPEndPoint(IPAddress.Any, port), backlog);
        }

        /// <inheritdoc cref="IAsyncTcpOperations.Listen(IPEndPoint, int)" />
        /// <param name="port">The port of the local endpoint.</param>
        public static void ListenAsync(this IAsyncTcpOperations socketOp, int port)
        {
            socketOp.ListenAsync(new IPEndPoint(IPAddress.Any, port), DefaultBacklog);
        }

        /// <inheritdoc cref="IAsyncTcpOperations.Listen(IPEndPoint, int)" />
        /// <param name="address">The address of the local endpoint.</param>
        /// <param name="port">The port of the local endpoint.</param>
        /// <param name="backlog">The number of backlog connections for listening.</param>
        public static void ListenAsync(this IAsyncTcpOperations socketOp, IPAddress address, int port, int backlog)
        {
            socketOp.ListenAsync(new IPEndPoint(address, port), backlog);
        }

        /// <inheritdoc cref="IAsyncTcpOperations.ConnectAsync(IPEndPoint)" />
        /// <param name="address">The address of the server to connect to.</param>
        /// <param name="port">The port of the server to connect to.</param>
        public static void ConnectAsync(this IAsyncTcpOperations socketOp, IPAddress address, int port)
        {
            socketOp.ConnectAsync(new IPEndPoint(address, port));
        }

        public static void ShutdownAsync(this IAsyncTcpOperations socketOp)
        {
            socketOp.ShutdownAsync(false);
        }
    }

    internal sealed class SaeaPool
    {
        private LocklessPool<SocketAsyncEventArgs> _pool;

        public SaeaPool(Func<SocketAsyncEventArgs> factory)
        {
            _pool = new LocklessPool<SocketAsyncEventArgs>(3, factory, AccessMode.FIFO);
        }

        public IPooledItem<SocketAsyncEventArgs> GetArgs()
        {
            return _pool.Acquire();
        }
    }

    internal sealed class AsyncTcpSocketOperations35Saea : IAsyncTcpOperations
    {
        public Socket Socket { get; private set; }

        private SocketAsyncEventArgs _connectArgs;
        private SocketAsyncEventArgs _shutdownArgs;

        private SaeaPool _writePool;
        private SaeaPool _writeCallbackOnErrorOnlyPool;
        private SaeaPool _readPool;
        private SaeaPool _acceptPool;

        private SaeaPool WritePool
        {
            get
            {
                return _writePool ?? (_writePool = new SaeaPool(() => SaeaFactory.CreateSaea(SocketWriteComplete)));
            }
        }

        private SaeaPool WriteCallBackOnErrorOnlyPool
        {
            get
            {
                return _writeCallbackOnErrorOnlyPool ?? (_writeCallbackOnErrorOnlyPool = new SaeaPool(() => SaeaFactory.CreateCallbackOnErrorOnlySaea(SocketWriteComplete)));
            }
        }

        private SaeaPool ReadPool
        {
            get
            {
                return _readPool ?? (_readPool = new SaeaPool(() => SaeaFactory.CreateSaea(SocketReadComplete)));
            }
        }

        private SaeaPool AcceptPool
        {
            get
            {
                return _acceptPool ?? (_acceptPool = new SaeaPool(() => SaeaFactory.CreateSaea(SocketAcceptComplete)));
            }
        }

        internal AsyncTcpSocketOperations35Saea([NotNull] Socket socket)
        {
            Validate.ArgumentNotNull(socket, "socket");

            Socket = socket;

            InitAsyncArgs();
        }

        private void InitAsyncArgs()
        {
            _connectArgs = SaeaFactory.CreateSaea(SocketConnectComplete);
            _shutdownArgs = SaeaFactory.CreateSaea(SocketShutdownComplete);
        }

        #region Async callbacks

        private void SocketAcceptComplete(object sender, SocketAsyncEventArgs args)
        {
            var customArgs = (CustomSocketAsyncEventArgs)args;
            var err = customArgs.SocketError;
            var socket = customArgs.AcceptSocket;
            customArgs.DisposeToken();
            if (err == SocketError.Success)
                AcceptCompleted.Raise(Socket, new AsyncResultEventArgs<IAsyncTcpOperations>(new AsyncTcpSocketOperations35Saea(customArgs.AcceptSocket)));
            else
                AcceptCompleted.Raise(Socket, new AsyncResultEventArgs<IAsyncTcpOperations>(new SocketException()));
        }

        private void SocketReadComplete(object sender, SocketAsyncEventArgs args)
        {
            var customArgs = (CustomSocketAsyncEventArgs) args;
            var buffer = customArgs.Buffer;
            var err = customArgs.SocketError;
            customArgs.DisposeToken();
            if (err == SocketError.Success)
                ReadCompleted.Raise(Socket, new AsyncResultEventArgs<byte[]>(buffer));
            else
                ReadCompleted.Raise(Socket, new AsyncResultEventArgs<byte[]>(new SocketException()));
        }

        private void SocketWriteComplete(object sender, SocketAsyncEventArgs args)
        {
            var customArgs = (CustomSocketAsyncEventArgs) args;
            var err = customArgs.SocketError;
            var callbackOnErrorOnly = customArgs.CallbackOnErrorOnly;
            customArgs.DisposeToken();
            if (err == SocketError.Success && callbackOnErrorOnly)
                return;
            if (err == SocketError.Success)
                WriteCompleted.Raise(Socket, new AsyncCompletedEventArgs(null, false, null));
            else
                WriteCompleted.Raise(Socket, new AsyncCompletedEventArgs(new SocketException(), false, null ));
        }

        private void SocketConnectComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
                ConnectCompleted.Raise(Socket, new AsyncCompletedEventArgs(null, false, null));
            else
                ConnectCompleted.Raise(Socket, new AsyncCompletedEventArgs(new SocketException(), false, null));
        }

        private void SocketShutdownComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
                ShutdownCompleted.Raise(Socket, new AsyncCompletedEventArgs(null, false, null));
            else
                ShutdownCompleted.Raise(Socket, new AsyncCompletedEventArgs(new SocketException(), false, null));
        }


        #endregion Async callbacks

        private void AcceptAsync()
        {
            var args = AcceptPool.GetArgs();
            args.Target.UserToken = args;
            if (!Socket.AcceptAsync(args.Target))
            {
                SocketAcceptComplete(Socket, args.Target);
            }
        }

        #region IAsyncTcpOperations Implementation

        public void ListenAsync(IPEndPoint bindTo, int backlog)
        {
            Socket.Bind(bindTo);
            Socket.Listen(backlog);
            AcceptAsync();
        }

        public EventHandler<AsyncResultEventArgs<IAsyncTcpOperations>> AcceptCompleted { get; set; }
        public void ConnectAsync(IPEndPoint server)
        {
            _connectArgs.RemoteEndPoint = server;
            Socket.ConnectAsync(_connectArgs);
        }

        public event EventHandler<AsyncCompletedEventArgs> ConnectCompleted;

        public void ShutdownAsync(bool resuseSocket)
        {
            _shutdownArgs.DisconnectReuseSocket = resuseSocket;
            Socket.DisconnectAsync(_shutdownArgs);
        }

        public event EventHandler<AsyncCompletedEventArgs> ShutdownCompleted;

        public event EventHandler<AsyncResultEventArgs<byte[]>> ReadCompleted;

        public void ReadAsync()
        {
            var args = ReadPool.GetArgs();
            args.Target.UserToken = args;
            if (!Socket.ReceiveAsync(args.Target))
            {
                SocketReadComplete(Socket, args.Target);
            }
        }

        public void WriteAsync(byte[] buffer)
        {
            var args = WritePool.GetArgs();
            args.Target.UserToken = args;
            if (!Socket.SendAsync(args.Target))
            {
                SocketWriteComplete(Socket, args.Target);       
            }
        }

        public void WriteAsyncCallbackOnErrorOnly(byte[] buffer)
        {
            var args = WriteCallBackOnErrorOnlyPool.GetArgs();
            args.Target.UserToken = args;
            if (!Socket.SendAsync(args.Target))
            {
                SocketWriteComplete(Socket, args.Target);
            }
        }

        public event EventHandler<AsyncCompletedEventArgs> WriteCompleted;

        #endregion IAsyncTcpOperations Implementation
    }
}
