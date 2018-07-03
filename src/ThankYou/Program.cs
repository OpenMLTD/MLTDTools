using System;
using System.IO;
using System.Reflection;
using CommandLine;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OpenMLTD.MiriTore.Logging;

namespace OpenMLTD.ThankYou {
    internal static class Program {

        [MTAThread]
        private static void Main(string[] args) {
            var commandlineParser = new CommandLine.Parser(settings => {
                settings.CaseInsensitiveEnumValues = true;
                settings.CaseSensitive = false;
                settings.IgnoreUnknownArguments = true;
            });

            var parsed = commandlineParser.ParseArguments<Options>(args);

            if (parsed.Tag == ParserResultType.NotParsed) {
                PrintUsage(true);
                return;
            }

            var options = ((Parsed<Options>)parsed).Value;

            if (options.ShowInfo) {
                PrintIPInfo();
                return;
            }

            IdolLog.Initialize(options.IsDebug ? "thankyou-debug" : "thankyou");

            var configFile = new FileInfo(DnsConfigFilePath);
            if (!configFile.Exists) {
                PrintUsage(true);
                return;
            }

            DnsConfig dnsConfig;
            try {
                var jsonData = File.ReadAllText(DnsConfigFilePath);
                dnsConfig = JsonConvert.DeserializeObject<DnsConfig>(jsonData);
            } catch (Exception) {
                PrintUsage(true);
                return;
            }

            RunConsoleProgram(dnsConfig);
        }

        private static void RunConsoleProgram([NotNull] DnsConfig dnsConfig) {
            Console.TreatControlCAsInput = true;
            Console.CancelKeyPress += OnConsoleCancelKeyPressed;

            IdolLog.InfoFormat(Lang.Get("upstream_dns_address+tpl"), dnsConfig.UpstreamDns);
            IdolLog.InfoFormat(Lang.Get("processing_host_names+tpl"), dnsConfig.LocalIP, string.Join(Environment.NewLine, dnsConfig.RedirectPatterns));

            var controller = new DnsController();

            controller.StartDns(dnsConfig);

            while (true) {
                var key = Console.ReadKey(true);
                if ((key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control) || key.Key == ConsoleKey.Enter) {
                    break;
                }
            }

            controller.StopDns();

            Console.CancelKeyPress -= OnConsoleCancelKeyPressed;

            IdolLog.Info(Lang.Get("dns_stopped"));
        }

        private static void PrintIPInfo() {
            var ss = IPAddressHelper.GetAvailableIPAddresses();
            foreach (var s in ss) {
                Console.WriteLine("{0} ({1})", s.Key, s.Value);
            }
        }

        private static void PrintUsage(bool wait) {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var usage =
                $@"
Usage:
        {assemblyName.Name}.exe [options]

Options:

        -i           Display a list of machine IP addresses.
        --help       Display this message.
        --debug      Log to debug log file.

    When started without arguments, the program tries to load configuration
    from `{DnsConfigFilePath}`.
    If the configuration file is not found or any error occurs while loading,
    this message is displayed.";
            Console.WriteLine(usage);

            if (wait) {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void OnConsoleCancelKeyPressed(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            Console.WriteLine("Please press Enter or Ctrl-C to stop the proxy.");
        }

        private static readonly string DnsConfigFilePath = "Resources/Config/dns.json";

    }
}