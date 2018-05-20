using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using console.SqlServer;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace console
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static IServiceProvider _serviceProvider {get; set; }
        
        static void Main(string[] args)
        {
            // Building the database using the Fluent Migrator
            
            BuildConfiguration();

            if(string.IsNullOrWhiteSpace(Configuration.GetConnectionString("DataStore"))){
                Console.WriteLine("WARNING: Be sure to fill in the DataStore Connection String on appsettings.json");
                return;
            }
            
            RunMigrations();

            ShowOptions();

            Console.WriteLine("Migrations done!");
        }

        private static void ShowOptions(){
            string option;
            
            while(true){
                Console.WriteLine("1 - Query data.");
                Console.WriteLine("2 - Remove from cache.");
                Console.WriteLine("X - Exit.");

                PrintBreakLine();

                Console.Write("Type an option: ");
                option = Console.ReadLine();

                switch (option.ToLower()){
                    case "1":
                        QueryData();
                        break;
                    case "2":
                        RemoveFromCache();
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please select a valid option.");
                        PrintBreakLine();
                        break;
                }
            }            
        }

        private static void RemoveFromCache()
        {
            Console.Write("Type the sticker # to be removed: ");
            var stickerNumber = Console.ReadLine();

            var cache = _serviceProvider.GetRequiredService<IDistributedCache>();

            cache.Remove($"Sticker:{stickerNumber}");

            PrintImportantInfo($"Sticker #{stickerNumber} removed from cache.");
            PrintBreakLine();
        }

        private static void QueryData()
        {
            Console.Write("Type the Sticker Number: ");
            var stickerNumber = Console.ReadLine();
            var source = string.Empty;
            
            Sticker sticker;

            var cache = _serviceProvider.GetRequiredService<IDistributedCache>();

            var cachedSticker = cache.GetString($"Sticker:{stickerNumber}");

            var stopWatch = new StopWatch();

            stopWatch.Start();

            if(!string.IsNullOrWhiteSpace(cachedSticker)){
                sticker = JsonConvert.DeserializeObject<Sticker>(cachedSticker);
                source = "Redis Cache";
            } else{
                using(var context = new StickersContext(Configuration.GetConnectionString("DataStore"))){
                    sticker = context.Stickers.FirstOrDefault(x => x.Number == int.Parse(stickerNumber));
                }

                if(sticker != null) {
                    cache.SetString($"Sticker:{stickerNumber}", JsonConvert.SerializeObject(sticker));
                    source = "SQL Database";
                }
            }

            stopWatch.Stop();

            if(sticker != null){
                PrintBreakLine();
                Console.WriteLine("Found the following sticker:");
                PrintBreakLine();
                Console.WriteLine($"Number: {sticker.Number}");
                Console.WriteLine($"Player: {sticker.PlayerName}");
                Console.WriteLine($"Country: {sticker.Country}");
                PrintImportantInfo($"Source: {source}");
                // Console.WriteLine($"Source: {source}");
                // Console.WriteLine($"Execution time: {stopWatch.ElapsedTime().Milliseconds}ms");
                PrintImportantInfo($"Execution time: {stopWatch.ElapsedTime().Milliseconds}ms");
                PrintBreakLine();
            }
        }

        private static void RunMigrations(){
            try{
                using (var scope = _serviceProvider.CreateScope()){
                    Console.WriteLine("Running FluentMigration with the following Connection String:");
                    
                    Console.WriteLine(Configuration.GetConnectionString("DataStore"));
                    
                    UpdateDatabase(scope.ServiceProvider);
                }
            } catch(Exception ex){
                Console.WriteLine($"Exception: {ex.GetType()}");
                Console.WriteLine($"Error while running migrations: {ex.Message}");
            }
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        private static IServiceProvider CreateServices(){
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(Configuration.GetConnectionString("DataStore"))
                .WithMigrationsIn(typeof(AddStickerTable).Assembly))
                .AddDistributedRedisCache(options =>
                {
                    options.Configuration = Configuration.GetConnectionString("Cache");
                    options.InstanceName = "riserrad-cacheaside";
                })
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            _serviceProvider = CreateServices();
        }

        private static void PrintBreakLine(){
            Console.WriteLine("-------");
        }

        private static void PrintImportantInfo(string message){
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
