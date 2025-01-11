var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Topic> topics =
[
    new Topic(Guid.Parse("63b4696e-88f5-4411-bc9a-51343c73fa97"), "Cars", "This is for talking all about vehicles."),
    new Topic(Guid.Parse("bc027c5f-5add-491e-878c-e322eea99baf"), "Coffee", "This is for conversations about The Liquid Stuff."),
    new Topic(Guid.Parse("8dc8eda6-7dc8-4b9b-a9e6-fffbf5f025b4"), "Bikes", "Conversations about Biking, urban policy, and other stuff."),
    new Topic(Guid.Parse("ec322b1b-1e2a-4a94-909c-bf94ed738e75"), "Programming", "Software dev convo."),
    new Topic(Guid.Parse("575ced2a-cd43-41f5-b17d-001bbed5fb99"), "Exercise", "Convo about exercise progress."),
];

// Get all subjects

app.MapGet("/topics", () =>
{
    app.Logger.LogInformation("Returning all topics");
    return Results.Ok(topics);
})
.WithName("GetTopics")
.WithOpenApi();

// Get a single subject

app.MapGet("/topic/{topicId:guid}", (Guid topicId) =>
{

    app.Logger.LogInformation("Looking for topic {TopicId}", topicId);
    foreach (Topic topic in topics)
    {
        if (topic.Id == topicId)
        {
            app.Logger.LogInformation("Found topic {TopicId}", topicId);
            return Results.Ok(topic);
        }
    }

    app.Logger.LogWarning("Not found topic {TopicId}", topicId);
    return Results.NotFound();
})
.WithName("GetTopicById")
.WithOpenApi();


app.MapGet("/topic/{topicName:alpha}", (string topicName) =>
{

    app.Logger.LogInformation("Looking for topic {TopicName}", topicName);
    foreach (Topic topic in topics)
    {
        if (string.Equals(topic.Name, topicName, StringComparison.OrdinalIgnoreCase))
        {
            app.Logger.LogInformation("Found topic {TopicName}", topicName);
            return Results.Ok(topic);
        }
    }

    app.Logger.LogWarning("{TopicName} not found", topicName);
    return Results.NotFound();

})
.WithName("GetTopicByName")
.WithDescription("Gets a topic by it's topic name (case-insensitive)")
.WithOpenApi();


app.Run();

internal record Topic(Guid Id, string Name, string Description);
