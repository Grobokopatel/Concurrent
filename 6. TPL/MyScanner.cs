using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TPL;

public class MyScanner : IPScanner
{
    public Task Scan(IPAddress[] ips, int[] ports)
    {
        return Task.WhenAll(ips.Select(ip => ProcessIp(ip, ports)));
    }

    private static Task ProcessIp(IPAddress ip, int[] ports)
    {
        return PingIp(ip).ContinueWith(CheckPorts);

        void CheckPorts(Task<IPStatus> pingTask)
        {
            if (pingTask.Result != IPStatus.Success)
                return;

            Task.WhenAll(ports.Select(port => CheckPortAsync(ip, port)))
                .ContinueWith(_ => _, TaskContinuationOptions.AttachedToParent);
        }
    }
    
    private static Task CheckPortAsync(IPAddress ip, int port)
    {
        var tcpClient = new TcpClient();
        Console.WriteLine($"Checking {ip}:{port}");

        return tcpClient
            .ConnectAsync(ip, port, 3000)
            .ContinueWith(DisposeTcpClient);

        void DisposeTcpClient(Task<PortStatus> task)
        {
            Console.WriteLine($"Checked {ip}:{port} - {task.Result}");
            tcpClient.Dispose();
        }
    }
    
    private static Task<IPStatus> PingIp(IPAddress ip, int timeout = 3000)
    {
        Console.WriteLine($"Pinging {ip}");
        var ping = new Ping();
        return ping
            .SendPingAsync(ip, timeout)
            .ContinueWith(DisposePing);

        IPStatus DisposePing(Task<PingReply> task)
        {
            ping.Dispose();
            var status = task.Result.Status;
            Console.WriteLine($"Pinged {ip}: {status}");
            return status;
        }
    }
}