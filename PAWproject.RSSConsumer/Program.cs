using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped( provider =>
    new RSSConsumer("https://feeds.bbci.co.uk/news"));

var app = builder.Build();

app.MapGet("/bbcConsumer", async (RSSConsumer rssConsumer, string? category, string? location) =>
{
    //var items = await rssConsumer.GetFeedItemsAsync(category, location);
    //Console.WriteLine(items); 
   
    //return Results.Ok(items);
});



app.Run();
