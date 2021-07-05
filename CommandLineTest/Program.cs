using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using CommandLine;

using System;
using System.Collections.Generic;
using System.IO;
using Utilities;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLineTest
{
    class Program
    {
        static public Logger logger = null;
        static public int retcode = 0;
        static public bool debug = false;
        static public string ApplicationName = "CommandLineTest";
        static private string LogFileName = String.Empty;
        static public IConfiguration config = null;
        static public Options opt = null;

        //static async Task<int> Main(string[] args)
        //{
        //    int retcode = await GetArgs(args);
            
        //    return retcode;
        //}

        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(async (opts) =>
                {
                    await CreateHostBuilder(args, opts).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Options opts) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(configureLogging => configureLogging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(opts);
                    services.AddHostedService<ImageClassifierWorker>()
                        .Configure<EventLogSettings>(config =>
                        {
                            config.LogName = "Image Classifier Service";
                            config.SourceName = "Image Classifier Service Source";
                        });
                }).UseWindowsService();
    

        static public string FileName = String.Empty;

        static private async Task<int> GetArgs(string[] args)
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(async (opts) =>
                {
                    opt = opts;
                    RunIt(args, opts);
                    return 0;
                },
                errs => Task.FromResult(-1));
        }
        static public int RunIt(string[] args,Options opt)
        { 
            SetupApplication();
            FileName = opt.FileName;
            debug = opt.debug;
 
            LoadConfiguration();

            logger.WriteLine($"FileName: {FileName}");
            logger.WriteLine($"{opt.number} * {opt.number} = {opt.number*opt.number}");

            ExitApplication(retcode);
            return retcode;
        }
        
        private static void SetupApplication()
        {
            config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            LogFileName = config["Logfile"];
            logger = new Logger(LogFileName);
            logger.WriteLine($"Starting {ApplicationName} ...");
            
        }


        //private static void LoadArgs(string[] args)
        //{
        //    Options opts = new Options();
        //    Parser.Default.ParseArguments<Options>(args)
        //        .MapResult(opts,0);

        //    int minimumRequiredArgs = 0;

        //    if (minimumRequiredArgs > args.Length)
        //    {
        //        logger.WriteLine($"Not all required argument have been speciifed");
        //        ExitApplication(retcode = -1);
        //    }
        //    // Place loading required arguments here

        //    int argCount = minimumRequiredArgs;
        //    while (argCount < args.Length)
        //    {
        //        string val = args[argCount].ToLower();

        //        string[] vals = val.Split('=');

        //        switch (vals[0])
        //        {
        //            case "-debug":
        //                debug = true;
        //                break;
        //            default:
        //                logger.WriteLine($"Unknown argument: {args[argCount]}");
        //                retcode = -1;
        //                ExitApplication(retcode);
        //                break;
        //        }
        //        ++argCount;
        //    }
        //}
        private static void LoadConfiguration()
        {
            logger.WriteLine($"This is in LoadConfiguration {opt.FileName}");
        }
        static void ExitApplication(int retcode)
        {
            logger.WriteLine($"Exiting {ApplicationName} Return Code {retcode}");

            if (debug)
            {
                Console.WriteLine($"Press any key to exit ...");
                Console.ReadKey();
            }
            Environment.Exit(retcode);
        }
    }
}
