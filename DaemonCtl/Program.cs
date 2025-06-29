using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("daemonctl.json", optional: true, reloadOnChange: true);

IConfiguration config = builder.Build();

while (true)
{
    List<string> daemons = config.GetSection("Daemons").Get<List<string>>();

    foreach (var daemon in daemons)
    {
        string output = RunLocalCommand(daemon);
        var result = ParseServiceStatus(output);

        if (!result)
        {
            output = RunLocalCommand(daemon, "restart");

            var color = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"{DateTime.Now} > {daemon} service has been restarted");

            Console.ForegroundColor = color;
        }
    }

    await Task.Delay(TimeSpan.FromMinutes(1));
}

static bool ParseServiceStatus(string output)
{
    var regex = new Regex(@"Active:\s+active \(running\)");
    return regex.IsMatch(output);
}

static string RunLocalCommand(string serviceName, string execType = "status")
{
    string result = "";
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "systemctl",
                Arguments = $"{execType} {serviceName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        var color = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;

        Console.Error.WriteLine($"{DateTime.Now} > error while executing systemctl: {ex}");

        Console.ForegroundColor = color;
    }
    return result;
}