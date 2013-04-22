using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomNetworking;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SpreadsheetModel
{
    public class SSModel
    {
        private StringSocket socket;
        public event Action<string> CreateOK;
        public event Action<string,string> CreateFail;
        public event Action<string,string,string,string,string> JoinOK;
        public event Action<string,string> JoinFail;
        public event Action<string,string> ChangeOk;
        public event Action<string,string> ChangeWait;
        public event Action<string,string> ChangeFail;
        public event Action<string,string,string,string,string> UndoOk;
        public event Action<string, string> UndoEnd;
        public event Action<string,string> UndoWait;
        public event Action<string,string> UndoFail;
        public event Action<string,string,string,string,string> Update;
        public event Action<string> SaveOk;
        public event Action<string,string> SaveFail;
        public event Action<string> Error;
        public event Action<string> Test;

        public void Connect(string host, int port)
        {
            if (socket == null)
            {
                TcpClient client = new TcpClient(host, port);
                socket = new StringSocket(client.Client, UTF8Encoding.Default);
            }
            //string name = "bill";
            //string password = "bill";
            //if (socket.Connected)
            //{
            //    socket.BeginSend("CREATE\nName:" + name + "\nPassword:" + password + "\n", (e, p) => { }, null);
            //}
            socket.BeginReceive(EventRecieved, null);
        }

        public void Create(string name, string password)
        {
            
            if (socket.Connected)
            {
                socket.BeginSend("CREATE\nName:" + name + "\nPassword:" + password + "\n", (e, p) => { }, null);
                //socket.BeginSend("CREATE\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("Password:" + password + "\n", (e, p) => { }, null);
            }
            socket.BeginReceive(EventRecieved, null);
        }
        public void Join(string name, string password)
        {
            if (socket.Connected)
            {
                socket.BeginSend("JOIN\nName:" + name + "\nPassword:" + password + "\n", (e, p) => { }, null);
                //socket.BeginSend("JOIN\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("Password:" + password + "\n", (e, p) => { }, null);
            }
            socket.BeginReceive(EventRecieved, null);
        }
        public void Change(string name, string version, string cell, string length, string content)
        {
            if (socket.Connected)
            {
                socket.BeginSend("CHANGE\nName:" + name + "\nVersion:" + version + "\nCell:" + cell + "\nLength:" + length + "\n"+content+"\n", (e, p) => { }, null);
                //socket.BeginSend("CHANGE\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("Version:" + version + "\n", (e, p) => { }, null);
            }
            socket.BeginReceive(EventRecieved, null);
        }
        public void Undo(string name, string version)
        {
            if (socket.Connected)
            {
                socket.BeginSend("UNDO\nName:" + name + "\nVersion:" + version + "\n", (e, p) => { }, null);
                //socket.BeginSend("UNDO\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("Version:" + version + "\n", (e, p) => { }, null);
            }
            socket.BeginReceive(EventRecieved, null);
        }
        public void Save(string name)
        {
            if (socket.Connected)
            {
                socket.BeginSend("SAVE\nName:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("SAVE\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                
            }
            socket.BeginReceive(EventRecieved, null);
        }
        public void Leave(string name)
        {
            if (socket.Connected)
            {
                socket.BeginSend("LEAVE\nName:" + name + "\n", (e, p) => { }, null);
                //socket.BeginSend("LEAVE\n", (e, p) => { }, null);
                //socket.BeginSend("Name:" + name + "\n", (e, p) => { }, null);
                
            }
        }

        private void EventRecieved(string s, Exception e, object payload)
        {
            if (s == null)
            {
                return;
            }
            if (Test != null)
            {
                Test(s);
            }
            
            if (CreateOK != null && s.StartsWith("CREATE OK"))
            {
               
               
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string password = words[2].TrimStart('P', 'a', 's', 's', 'w', 'o', 'r', 'd', ':');
               
                //Join(name, password);

            }
           
            if (CreateFail != null && s.StartsWith("CREATE FAIL"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string message = words[2];
                CreateFail(name,message);
               
            }
            if (JoinOK != null && s.StartsWith("JOIN OK"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
                string length = words[3].TrimStart('L', 'e', 'n', 'g', 't', 'h', ':');

                TextWriter tw = new StreamWriter("data.ss");

                // write a line of text to the file
                tw.WriteLine(words[4].ToLower());

                // close the stream
                tw.Close();
                

                JoinOK(name,version,length,"data.ss",words[4]);
               
            }
            if (JoinFail != null && s.StartsWith("JOIN FAIL"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string message = words[2];
                JoinFail(name,message);
                
            }
            if (ChangeOk != null && s.StartsWith("CHANGE OK"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
                ChangeOk(name,version);
                
            }
            if (ChangeWait != null && s.StartsWith("CHANGE WAIT"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
                ChangeWait(name,version);
                
            }
            if (ChangeFail != null && s.StartsWith("CHANGE FAIL"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string message = words[2];
                ChangeFail(name,message);
                
            }
            if (UndoOk != null && s.StartsWith("UNDO OK"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
                string cell = words[3].TrimStart('C', 'e', 'l', 'l', ':');
                string length = words[4].TrimStart('L', 'e', 'n', 'g', 't', 'h', ':');
                string content = words[5];
                UndoOk(name,version,cell,length,content);
               
            }
            if (UndoEnd != null && s.StartsWith("UNDO END"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
               
                UndoEnd(name,version);
               
            }
            if (UndoWait != null && s.StartsWith("UNDO WAIT"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
               
                UndoWait(name,version);
                
            }
            if (UndoFail != null && s.StartsWith("UNDO FAIL"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string message = words[2];
                UndoFail(name,message);
                
            }
            if (Update != null && s.StartsWith("UPDATE"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string version = words[2].TrimStart('V', 'e', 'r', 's', 'i', 'o', 'n', ':');
                string cell = words[3].TrimStart('C', 'e', 'l', 'l', ':');
                string length = words[4].TrimStart('L', 'e', 'n', 'g', 't', 'h', ':');
                string content = words[5];
                Update(name,version,cell,length,content);
                
            }
            if (SaveOk != null && s.StartsWith("SAVE OK"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                SaveOk(name);
               
            }
            if (SaveFail != null && s.StartsWith("SAVE FAIL"))
            {
                string[] words = Regex.Split(s, "\n");
                string name = words[1].TrimStart('N', 'a', 'm', 'e', ':');
                string message = words[2];
                SaveFail(name,message);
               
            }
            if (Error != null && s.StartsWith("Error"))
            {
                string[] words = Regex.Split(s, "\n");
                Error(words[0]);
                
            }
            
            if (socket != null)
            {
                socket.BeginReceive(EventRecieved, null);
            }
        }
    }
}
