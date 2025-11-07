using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped( provider =>
    new RSSConsumer("https://feeds.bbci.co.uk/news/world/rss.xml"));

var app = builder.Build();

app.MapGet("/bbcConsumer", async (RSSConsumer rssConsumer) =>
{
    var items = await rssConsumer.GetFeedItemsAsync();
    Console.WriteLine(items); 
   
    return Results.Ok(items);
});



app.Run();
