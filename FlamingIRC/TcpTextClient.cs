﻿//
// TcpTextClient.cs
//
// Author:
//       Joshua Simmons <simmons.44@gmail.com>
//
// Copyright (c) 2009 Joshua Simmons
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace FlamingIRC
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public enum ConnectError
    {
        ConnectionRefused,
        AuthenticationFailed,
        SocketError,
        Other
    }

    public enum DisconnectReason
    {
        UserInitiated,
        PingTimeout,
        SocketError,
        RemoteHostClosedConnection,
        Other
    }

    public abstract class TcpTextClient
    {
        private const int bufferLength = 512;
        private byte[] byteBuffer;
        private Socket socket;
        private StringBuilder textBuffer;
        private string serverName;
        private SslStream sslStream;
        private Stream stream;
        private bool usesSSL;


        protected TcpTextClient()
        {
            byteBuffer = new byte[bufferLength];
            textBuffer = new StringBuilder();
            TextEncoding = Encoding.UTF8;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Encoding TextEncoding { get; protected set; }

        public bool Connected { get; protected set; }

        /// <summary>
        /// Create a new connection to the server.
        /// Will call OnConnected or OnDisconnected depending on connection attempt outcome.
        /// </summary>
        /// <param name="server">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="port">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="ssl">
        /// A <see cref="System.Boolean"/>
        /// </param>
        protected void Connect(string server, int port, bool ssl)
        {
            usesSSL = ssl;
            serverName = server;
            socket.BeginConnect(server, port, onConnect, null);
        }

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        /// <remarks>
        /// The client need not be actually connected. This method will also "give up" an
        ///  attept to connect or revert to a disconnected state after an error.
        /// </remarks>
        public void Disconnect(DisconnectReason reason)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connected = false;
            OnDisconnect(reason, null);
        }

        /// <summary>
        /// Sends a string message to the server.
        /// </summary>
        /// <param name="message">
        /// A <see cref="System.String"/>
        /// </param>
        public void Send(string message)
        {
            message = message + "\r\n";
            byte[] buffer = TextEncoding.GetBytes(message);
            stream.BeginWrite(buffer, 0, buffer.Length, onSend, null);
            Console.Write("Outgoing: {0}", message);
        }

        private void onConnect(IAsyncResult res)
        {
            try
            {
                socket.EndConnect(res);
                stream = new NetworkStream(socket);

                if (usesSSL)
                {
                    sslStream = new SslStream(stream, false, onCertificateValidate);
                    sslStream.BeginAuthenticateAsClient(serverName, onAuthenticate, null);
                    stream = sslStream;
                }
                else
                {
                    OnConnect();
                    waitForData();
                }
            }
            catch (SocketException e)
            {
                OnConnectFailed(ConnectError.SocketError, e.ErrorCode);
            }
            catch (Exception e)
            {
                OnConnectFailed(ConnectError.SocketError, null);
                System.Diagnostics.Debug.WriteLine("Connect failed: " + e.Message);
                throw;
            }
        }

        private bool onCertificateValidate(object sender, X509Certificate certificate, X509Chain chain,
                                           SslPolicyErrors errors)
        {
            if (errors != SslPolicyErrors.None)
            {
                return OnCertificateValidatecateFailed(certificate, chain, errors);
            }

            return true;
        }

        private void onAuthenticate(IAsyncResult res)
        {
            try
            {
                sslStream.EndAuthenticateAsClient(res);
                OnConnect();
                waitForData();
            }
            catch (AuthenticationException)
            {
                socket.Shutdown(SocketShutdown.Both);
                Connected = false;
                OnConnectFailed(ConnectError.AuthenticationFailed, null);
            }
        }

        private void waitForData()
        {
            try
            {
                stream.BeginRead(byteBuffer, 0, bufferLength, onDataReceived, null);
            }
            catch (Exception)
            {
                socket.Shutdown(SocketShutdown.Both);
                Connected = false;
                OnDisconnect(DisconnectReason.SocketError, null);
                throw; //Wasn't really handled so pass it through
            }
        }

        private void onDataReceived(IAsyncResult res)
        {
            if (!socket.Connected)
                return;

            try
            {
                int bytes = stream.EndRead(res);

                if (bytes == 0)
                {
                    // Connection Closed!
                    socket.Shutdown(SocketShutdown.Both);
                    Connected = false;
                    OnDisconnect(DisconnectReason.RemoteHostClosedConnection, null);
                    return;
                }

                string text =
                    TextEncoding.GetString((bytes == bufferLength) ? byteBuffer : byteBuffer.Slice(0, bytes));
                foreach (char item in text)
                {
                    switch (item)
                    {
                        case '\r':
                            continue;

                        case '\n':
                            OnReceiveLine(textBuffer.ToString());
                            textBuffer.Clear();
                            break;

                        default:
                            textBuffer.Append(item);
                            break;
                    }
                }

                waitForData();
            }
            catch (IOException)
            {
                Disconnect(DisconnectReason.SocketError);
            }
            catch (SocketException) //hack - What kind of exception?
            {
                Disconnect(DisconnectReason.SocketError);
                throw; //Wasn't really handled so pass it through
            }
        }

        private void onSend(IAsyncResult res)
        {
            try
            {
                stream.EndWrite(res);
            }
            catch (Exception)
            {
                socket.Shutdown(SocketShutdown.Both);
                Connected = false;
                OnDisconnect(DisconnectReason.SocketError, null);
                throw; //Wasn't really handled so pass it through
            }
        }

        protected abstract void OnConnect();

        protected abstract bool OnCertificateValidatecateFailed(X509Certificate certificate, X509Chain chain,
                                                                SslPolicyErrors errors);

        protected abstract void OnDisconnect(DisconnectReason reason, int? socketErrorCode);
        protected abstract void OnConnectFailed(ConnectError error, int? socketErrorCode);
        protected abstract void OnReceiveLine(string line);
    }
}