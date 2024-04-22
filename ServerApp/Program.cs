using MsgInfo;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class ChatServer
{
    const short port = 4040;
    const string JOIN = "$<Join>";
    UdpClient server;
    IPEndPoint clientEndPoint = null;
    Dictionary<string, IPEndPoint> members;

    public ChatServer()
    {
        server = new UdpClient(port);
        members = new Dictionary<string, IPEndPoint>();
    }
    public void Start()
    {
        while (true)
        {
            byte[] data = server.Receive(ref clientEndPoint);
            string message = Encoding.Unicode.GetString(data);
            MessageInfo res = JsonSerializer.Deserialize<MessageInfo>(message);

            switch (res.Connection)
            {
                case JOIN:
                    AddMember(clientEndPoint, res.Name);
                    break;
                default:
                    if (res.Flag)
                    {
                        var user = members[res.Name];
                        members.Clear();
                        members.Add(res.Name, user);
                        members.Add($" ", clientEndPoint);
                    }
                    else
                    {
                        Console.WriteLine($"Got message {message,-20} from : {clientEndPoint} at {DateTime.Now.ToShortTimeString()}");
                        SendToAll(data);
                    }
                    break;
            }
        }
    }
    private void AddMember(IPEndPoint clientEndPoint, string name)
    {
        members.Add(name, clientEndPoint);
        Console.WriteLine("Member was added");
    }
    private async void SendToAll(byte[] data)
    {
        foreach (var member in members)
        {
            await server.SendAsync(data, data.Length, member.Value);
        }
    }
}
internal class Program
{

    private static void Main(string[] args)
    {
        ChatServer server = new ChatServer();
        server.Start();
    }
}