using System.Net;
using System.Net.Sockets;

namespace CourseWork;

public class Server
{
    private const int Port = 11111;
    private const int ThreadsCount = 32;

    private readonly TcpListener _tcpListener;

    private bool _needToStop;
    private bool _needToStart = true;
    
    public Server()
    {
        InvertedIndex.Initialize(ThreadsCount);
        
        InvertedIndex.NeedToStopServer += () => _needToStop = true;
        InvertedIndex.NeedToStartServer += () => _needToStart = true;
        
        _tcpListener = new TcpListener(IPAddress.Any, Port);

        var thread = new Thread(Process);
        thread.Start();
    }
    
    private void Process()
    {
        _tcpListener.Start();
        
        Console.WriteLine("Server opened");

        while (true)
        {
            if (_tcpListener.Pending() == false) 
                continue;
            
            Console.WriteLine($"Client connected.");
                
            var client = _tcpListener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(HandleClient, client);
        }
    }

    private void HandleClient(object? tcpClient)
    {
        var client = (TcpClient)tcpClient!; 
        var clientStream = client.GetStream();
        
        var reader = new StreamReader(clientStream);
        var writer = new StreamWriter(clientStream);

        var word = reader.ReadLine();
        if (word != null)
        {
            var paths = InvertedIndex.GetPaths(word);

            var response = string.Join(",", paths);
            writer.WriteLine(response);
            
            Console.WriteLine("Response sent");
        }

        writer.Flush();
    }
}