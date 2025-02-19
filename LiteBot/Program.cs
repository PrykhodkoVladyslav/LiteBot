using LiteBot;
using LiteBot.HostedServices;
using LiteBot.Interfaces;
using LiteBot.Options;
using LiteBot.Services;
using LiteBot.Services.CommandHandlers;
using LiteBot.StableDiffusion;
using LiteBot.StableDiffusion.DTOAccessors;
using LiteBot.StableDiffusion.UserRequests;
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

builder.Services.AddScoped<CommandController>();

builder.Services.AddScoped<HelpHandler>();
builder.Services.AddScoped<StableDiffusionHandler>();
builder.Services.AddScoped<TimeHandler>();
builder.Services.AddScoped<RandomHandler>();
builder.Services.AddScoped<AuthorHandler>();
builder.Services.AddScoped<ArtHandler>();

builder.Services.AddSingleton<ICommandAnalizer, CommandAnalizer>();
builder.Services.AddScoped<ISocketMessageAccessor, SocketMessageAccessor>();
builder.Services.AddSingleton<StableDiffusionApi>();
builder.Services.AddSingleton<StableDiffusionQueue>();
builder.Services.AddSingleton<Txt2imgAccessor>();

builder.Services.AddScoped<GenerationRequest>();
builder.Services.AddScoped<SetPropertyRequest>();
builder.Services.AddScoped<ResetPropertyRequest>();


builder.Services.AddLogging(logging => {
	logging.ClearProviders();
	logging.AddConsole();
	logging.AddDebug();
});

var app = builder.Build();

await app.RunAsync();
