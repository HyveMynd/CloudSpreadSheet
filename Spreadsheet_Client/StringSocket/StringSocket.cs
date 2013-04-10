using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CustomNetworking
{
    
    public class StringSocket
    {

        private class SendPackage
        {
            public string SendString { get; set; }
            public SendCallback Callback { get; set; }
            public object Payload { get; set; }

            public SendPackage(string s, SendCallback callback, object payload)
            {
                this.SendString = s;
                this.Callback = callback;
                this.Payload = payload;
            }
        }

        private class ReceivePackage
        {
            public ReceiveCallback Callback { get; set; }
            public object Payload { get; set; }

            public ReceivePackage(ReceiveCallback c, object p)
            {
                this.Callback = c;
                this.Payload = p;
            }
        }

        // These delegates describe the callbacks that are used for sending and receiving strings.
        public delegate void SendCallback(Exception e, object payload);
        public delegate void ReceiveCallback(String s, Exception e, object payload);

        private Socket socket;
        private Encoding encoding;
        private Queue<ReceivePackage> receiveQueue; // queue for items to be received
        private Queue<SendPackage> sendQueue; // queue for items to be sent;
        private string incoming, outgoing;
        private readonly object sendSync, receiveSync;

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// LineSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        public StringSocket(Socket s, Encoding e)
        {
            //initialize variables
            socket = s;
            encoding = e;
            sendQueue = new Queue<SendPackage>();
            receiveQueue = new Queue<ReceivePackage>();
            incoming = "";
            outgoing = "";
            sendSync = new object();
            receiveSync = new object();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket, or failed in the 
        /// attempt, it invokes the callback.  The parameters to the callback are a
        /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
        /// the Exception that caused the send attempt to fail. 
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            //lock the enqueue so only one thread is queueing at a time
            lock (sendSync)
            {
                sendQueue.Enqueue(new SendPackage(s, callback, payload));
                if(sendQueue.Count == 1)
                    ProcessSendQueue();
            }
        }

        private void ProcessSendQueue()
        {
            if (sendQueue.Count > 0)
            {
                outgoing = sendQueue.Peek().SendString; //add the string to be sent to outgoing
                SendBytes();
            }
        }

        /// <summary>
        /// Attempts to send the entire outgoing string.
        /// </summary>
        private void SendBytes()
        {
            byte[] outgoingBuffer = encoding.GetBytes(outgoing);
            socket.BeginSend(outgoingBuffer, 0, outgoingBuffer.Length, SocketFlags.None, MessageSent, outgoingBuffer);
        }


        /// <summary>
        /// Called when a message has been successfully sent
        /// </summary>
        private void MessageSent(IAsyncResult result)
        {
            // Find out how many bytes were actually sent
            int bytes = socket.EndSend(result);

            lock (sendSync)
            {
                //if the number of bytes send is the same as the length of outgoing everything was sent
                if (bytes == outgoing.Length)
                {
                    SendPackage sendPackage = sendQueue.Dequeue();
                    outgoing = "";
                    //fire callback
                    ThreadPool.QueueUserWorkItem(new WaitCallback((e) => sendPackage.Callback(null, sendPackage.Payload)));
                    //dequeue more items
                    ProcessSendQueue();
                }
                else
                {
                    // Prepend the unsent bytes and try sending again.
                    outgoing = outgoing.Substring(bytes);
                    SendBytes();
                }
            }
        }

        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, or
        /// failed in the attempt, it invokes the callback.  The parameters to the callback are
        /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
        /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
        /// it is the requested string (with the newline removed).  If the Exception is non-null, 
        /// it is the Exception that caused the send attempt to fail.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload)
        {
            lock (receiveSync)
            {
                receiveQueue.Enqueue(new ReceivePackage(callback, payload));  //lock enqueue to allow only one thread to queue at a time
                if (receiveQueue.Count == 1)
                    ReceiveMessage();
            }
        }

        private void ReceiveMessage()
        {
            //process any newlines already in incoming
            ProcessNewLine();

            //process any remaining callbacks by asking for more bytes
            if (receiveQueue.Count > 0)
            {
                byte[] incomingBuffer = new byte[1024];
                socket.BeginReceive(incomingBuffer, 0, incomingBuffer.Length, SocketFlags.None, MessageReceived, incomingBuffer);
            }
        }

        private void MessageReceived(IAsyncResult result)
        {
            // Get the buffer to which the data was written.
            byte[] buffer = (byte[])result.AsyncState;

            // Figure out how many bytes have come in
            int bytes = socket.EndReceive(result);

            lock (receiveSync)
            {
                // Convert the bytes into a string
                incoming += encoding.GetString(buffer, 0, bytes);

                // fire callbacks for any newline character
                ProcessNewLine();
                
                //listen for more if there are callbacks in the queue
                ReceiveMessage();
            }
        }

        private void ProcessNewLine ()
        {
            int index;
            while ((index = incoming.IndexOf('\n')) >= 0 && receiveQueue.Count > 0)
            {
                ReceivePackage receivePackage = receiveQueue.Dequeue();
                String line = incoming.Substring(0, index);
                incoming = incoming.Substring(index + 1);

                //fire callbacks for each newline
                ThreadPool.QueueUserWorkItem(new WaitCallback((e) => receivePackage.Callback(line, null, receivePackage.Payload)));
            }
        }
    }
 }