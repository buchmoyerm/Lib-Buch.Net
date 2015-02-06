using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using ph4n.Common;

namespace ph4n.Net
{
    /// <summary>
    /// This is a special class that may be passed to some WriteAsync methods to indicate that WriteCompleted should not be called on success.
    /// </summary>
    public sealed class CallbackOnErrorsOnly { }

    /// <summary>
    /// TcpSocket will wrap up all of the standard socket opperations
    /// </summary>
    internal abstract class TcpSocketOld
    {
        internal abstract Socket UnderlyingSocket { get; }


        /// <summary>
        /// Closes the underlying socket and frees the socket resources. Be sure to clear all events before calling this method!
        /// The socket will be gracefully closed in the background by the WinSock dll unless <see cref="SetAbortive"/> has been called,
        /// in which case the socket will be abortively closed and immediately freed. If the process exits shortly after calling this
        /// method, the socket will be abortively closed when the WinSock dll is unloaded.
        /// </summary>
        // Always runs in User thread
        public void Dispose()
        {
            // Do the actual close
            UnderlyingSocket.Close();
        }

        /// <summary>
        /// Binds to a local endpoint. This method is not normally used.
        /// </summary>
        /// <param name="bindTo">The local endpoint.</param>
        public void Bind(IPEndPoint bindTo)
        {
            UnderlyingSocket.Bind(bindTo);
        }

        #region Connection properties

        /// <summary>
        /// Sets a flag in the socket to indicate that the close (performed by <see cref="Dispose"/>) should be done abortively.
        /// </summary>
        // Always runs in User thread
        public void SetAbortive()
        {
            UnderlyingSocket.LingerState = new LingerOption(true, 0);
        }

        /// <summary>
        /// Returns the IP address and port on this side of the connection.
        /// </summary>
        public IPEndPoint LocalEndPoint { get { return (IPEndPoint)UnderlyingSocket.LocalEndPoint; } }

        /// <summary>
        /// Returns the IP address and port on the remote side of the connection.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get { return (IPEndPoint)UnderlyingSocket.RemoteEndPoint; } }

        /// <summary>
        /// True if the Nagle algorithm has been disabled. The default is false. Generally, this should be left to its default value.
        /// </summary>
        public bool NoDelay { get { return UnderlyingSocket.NoDelay; } set { UnderlyingSocket.NoDelay = value; } }

        /// <summary>
        /// If and how long the a graceful shutdown will be performed in the background. Setting LingerState to enabled with a 0 timeout will make all calls
        /// to Close act as though AbortiveClose was called. Generally, this should be left to its default value.
        /// </summary>
        public LingerOption LingerState { get { return UnderlyingSocket.LingerState; } set { UnderlyingSocket.LingerState = value; } }

        #endregion

        #region Send events

        public EventHandler<SocketAsyncEventArgs> SendCompleted { get; set; }
        protected void SocketSendComplete(object sender, SocketAsyncEventArgs args)
        {
            // If there's no error and the user state is CallbackOnErrorsOnly, then don't issue the callback
            if (args.SocketError != SocketError.Success && args.UserToken is CallbackOnErrorsOnly)
                return;
            SendCompleted.Raise(this, args);
        }
        public abstract void Send(byte[] bytes);

        #endregion

        #region Recieve events

        /// <summary>
        /// Delegate to invoke when the read operation completes.
        /// </summary>
        public EventHandler<SocketAsyncEventArgs> ReceiveCompleted { get; set; }
        protected void SocketReceiveComplete(object sender, SocketAsyncEventArgs args)
        {
            ReceiveCompleted.Raise(this, args);
        }

        public abstract void StartReceiving();

        #endregion

        #region Shutdown operation

        /// <summary>
        /// Delegate to invoke when the shutdown operation completes.
        /// </summary>
        public EventHandler<AsyncCompletedEventArgs> ShutdownCompleted { get; set; }

        #endregion
    }
}