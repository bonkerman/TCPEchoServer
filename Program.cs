using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP
{
    class Program
    {
        private static CancellationTokenSource cancellation;
        private static int totalTaskNr = 0;
        private static async Task Main(string[] args)
        {
        var listener = new TcpListener(IPAddress.IPv6Any, 10000);  
        
        listener.Start();  
        try
        {   
           cancellation = new CancellationTokenSource();
           await Task.WhenAll(
                Enumerable.Range(0, Environment.ProcessorCount)
                    .Select( _ => BeginConversation(listener.AcceptTcpClientAsync()))
            );
            
        }
       finally
       {
           listener.Stop();
           System.Console.WriteLine("Done");
       }
    }

        private static async Task BeginConversation(Task<TcpClient> clientTask){
            var tasknr=++totalTaskNr;
            var client = await clientTask;
            Console.WriteLine("{0} Begun with " + client.Client.RemoteEndPoint.ToString(), tasknr);
            using(var ns = client.GetStream()){
                try
                {
                    while(!cancellation.Token.IsCancellationRequested){
                        var buffer = new byte[client.ReceiveBufferSize];
                        var size =await Task.Run(()=>ns.ReadAsync(buffer, 0, client.ReceiveBufferSize), cancellation.Token);
                            if(size==0){
                                System.Console.WriteLine(tasknr+") recieved data is null; Exiting;");
                                break;
                            }
                        await Task.Run(()=>ns.WriteAsync(buffer, 0, size), cancellation.Token);
                    }
                }
                finally
                {
                    System.Console.WriteLine(tasknr+" done");
                    client.GetStream().Close();
                    client.Close();
                }
                
            }
            
        }
    }
}
