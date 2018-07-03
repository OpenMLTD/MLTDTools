using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using JetBrains.Annotations;
using OpenMLTD.MiriTore.Core;

namespace OpenMLTD.ThankYou {
    /// <inheritdoc />
    /// <summary>
    /// A simple TCP forwarder.
    /// </summary>
    /// <remarks>
    /// See http://blog.brunogarcia.com/2012/10/simple-tcp-forwarder-in-c.html.
    /// </remarks>
    public sealed class TcpForwarderSlim : DisposableBase {

        public TcpForwarderSlim() {
            MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public event EventHandler<EventArgs> Forwarding;

        public Socket MainSocket { get; }

        public void Start([NotNull] IPEndPoint local, [NotNull] IPEndPoint remote) {
            _thread = new Thread(ThreadProc) {
                IsBackground = true
            };

            _continueWorking = true;

            _thread.Start(new ThreadParam {
                Local = local,
                Remote = remote
            });
        }

        public void Start([NotNull] Lazy<IPEndPoint> localFunc, [NotNull] Lazy<IPEndPoint> remoteFunc) {
            var remote = remoteFunc.Value;
            var local = localFunc.Value;

            Start(local, remote);
        }

        // Note: this method does not really join the thread. It just signals the worker thread.
        public void Stop() {
            _continueWorking = false;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                MainSocket.Dispose();
            }
        }

        private void ThreadProc([NotNull] object param) {
            var p = (ThreadParam)param;

            try {
                MainSocket.Bind(p.Local);
                MainSocket.Listen(10);
                while (_continueWorking) {
                    //                    Console.WriteLine("{0} : Starting forwarding with new tunnel", remote.Address);
                    var source = MainSocket.Accept();
                    var destination = new TcpForwarderSlim();
                    var state = new State(source, destination.MainSocket);
                    destination.Connect(p.Remote, source);
                    source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                    //                    Console.WriteLine("{0} : Request Forwarded", remote.Address);
                    Forwarding?.BeginInvoke(this, EventArgs.Empty, null, null);
                }
            } catch (SocketException) {
                //                Console.WriteLine("{0} :  Closing tunnel", remote.Address);
            }
        }

        private void Connect([NotNull] EndPoint remoteEndPoint, [NotNull] Socket destination) {
            var state = new State(MainSocket, destination);
            MainSocket.Connect(remoteEndPoint);
            MainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        private static void OnDataReceive(IAsyncResult result) {
            var state = (State)result.AsyncState;
            try {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0) {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
            } catch {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private sealed class State {

            public State([NotNull] Socket source, [NotNull] Socket destination) {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[BufferSize];
            }

            [NotNull]
            public Socket SourceSocket { get; }

            [NotNull]
            public Socket DestinationSocket { get; }

            [NotNull]
            public byte[] Buffer { get; }

            private const int BufferSize = 8192;

        }

        private sealed class ThreadParam {

            public IPEndPoint Local { get; set; }

            public IPEndPoint Remote { get; set; }

        }

        private Thread _thread;
        private bool _continueWorking;

    }
}