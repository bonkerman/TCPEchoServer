using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
  
// State object for reading client data asynchronously  
public class StateObject {  
    public Socket workSocket = null;  
    public const int BufferSize = 1024;  
    public byte[] buffer = new byte[BufferSize];  
}  
  
public class AsynchronousSocketListener {  
    public static ManualResetEvent allDone = new ManualResetEvent(false);  
    public static void StartListening() {    
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, 10000);  
        Socket listener = new Socket(IPAddress.IPv6Loopback.AddressFamily,  
            SocketType.Stream, ProtocolType.Tcp );  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(100);  
            while (true) {   
                allDone.Reset();    
                Console.WriteLine("Waiting for a connection...");  
                listener.BeginAccept(   
                    new AsyncCallback(AcceptCallback),  
                    listener );  
                allDone.WaitOne();  
            }  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
        Console.WriteLine("\nPress ENTER to continue...");  
        Console.Read();  
    }  
  
    public static void AcceptCallback(IAsyncResult ar) {  
        allDone.Set();   
        Socket listener = (Socket) ar.AsyncState;  
        Socket handler = listener.EndAccept(ar);  
        System.Console.WriteLine("Connection recieved from "+ handler.RemoteEndPoint);  
        StateObject state = new StateObject();  
        state.workSocket = handler;  
        handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);  
    }  
  
    public static void ReadCallback(IAsyncResult ar) {  
        StateObject state = (StateObject) ar.AsyncState; 
        int bytesRead = state.workSocket.EndReceive(ar);
        Send(state, state.buffer);  
    } 
  
    private static void Send(StateObject state, byte[] data) {  
        state.workSocket.BeginSend(state.buffer, 0, data.Length, 0, new AsyncCallback(SendCallback), state);  
    }  
  
    private static void SendCallback(IAsyncResult ar) {  
        try {  
            var state = (StateObject) ar.AsyncState;
            int bytesSent = state.workSocket.EndSend(ar); 
            state.workSocket.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state); 
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    public static int Main(String[] args) {  
        StartListening();  
        return 0;  
    }  
}using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
  
// State object for reading client data asynchronously  
public class StateObject {  
    public Socket workSocket = null;  
    public const int BufferSize = 1024;  
    public byte[] buffer = new byte[BufferSize];  
}  
  
public class AsynchronousSocketListener {  
    public static ManualResetEvent allDone = new ManualResetEvent(false);  
    public static void StartListening() {    
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, 10000);  
        Socket listener = new Socket(IPAddress.IPv6Loopback.AddressFamily,  
            SocketType.Stream, ProtocolType.Tcp );  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(100);  
            while (true) {   
                allDone.Reset();    
                Console.WriteLine("Waiting for a connection...");  
                listener.BeginAccept(   
                    new AsyncCallback(AcceptCallback),  
                    listener );  
                allDone.WaitOne();  
            }  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
        Console.WriteLine("\nPress ENTER to continue...");  
        Console.Read();  
    }  
  
    public static void AcceptCallback(IAsyncResult ar) {  
        allDone.Set();   
        Socket listener = (Socket) ar.AsyncState;  
        Socket handler = listener.EndAccept(ar);  
        System.Console.WriteLine("Connection recieved from "+ handler.RemoteEndPoint);  
        StateObject state = new StateObject();  
        state.workSocket = handler;  
        handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);  
    }  
  
    public static void ReadCallback(IAsyncResult ar) {  
        StateObject state = (StateObject) ar.AsyncState; 
        int bytesRead = state.workSocket.EndReceive(ar);
        Send(state, state.buffer);  
    } 
  
    private static void Send(StateObject state, byte[] data) {  
        state.workSocket.BeginSend(state.buffer, 0, data.Length, 0, new AsyncCallback(SendCallback), state);  
    }  
  
    private static void SendCallback(IAsyncResult ar) {  
        try {  
            var state = (StateObject) ar.AsyncState;
            int bytesSent = state.workSocket.EndSend(ar); 
            state.workSocket.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state); 
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    public static int Main(String[] args) {  
        StartListening();  
        return 0;  
    }  
}
