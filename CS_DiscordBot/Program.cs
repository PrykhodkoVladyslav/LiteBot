using LiteBot.CommandHandlers.Commands;
using LiteBot.HostedServices;
using LiteBot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var config = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.Configure<BotOptions>(builder.Configuration.GetRequiredSection("Bot"));
builder.Services.Configure<WhiteListOptions>(builder.Configuration.GetRequiredSection("Bot:WhiteList"));



builder.Services.AddHostedService<DiscordBot>();

builder.Services.AddScoped<BotCommandHandler>();



builder.Services.AddLogging(logging => {
	logging.ClearProviders();
	logging.AddConsole();
	logging.AddDebug();
});

var app = builder.Build();

await app.RunAsync();
