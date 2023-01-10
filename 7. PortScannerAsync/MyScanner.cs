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

    private static async Task ProcessIp(IPAddress ip, int[] ports)
    {
        var status = await PingIpAsync(ip);
        if (status != IPStatus.Success)
            return;

        await Task.WhenAll(ports.Select(port => CheckPortAsync(ip, port)));
    }

    private static async Task CheckPortAsync(IPAddress ip, int port)
    {
        Console.WriteLine($"Checking {ip}:{port}");
        using (var tcpClient = new TcpClient())
        {
            var result = await tcpClient.ConnectAsync(ip, port, 3000);

            Console.WriteLine($"Checking {ip}:{port} - {result}");
        }
    }

    private static async Task<IPStatus> PingIpAsync(IPAddress ip, int timeout = 3000)
    {
        Console.WriteLine($"Pinging {ip}");
        using (var ping = new Ping())
        {
            var result = await ping.SendPingAsync(ip, timeout);

            Console.WriteLine($"Pinged {ip}: {result.Status}");
            return result.Status;
        }
    }
}