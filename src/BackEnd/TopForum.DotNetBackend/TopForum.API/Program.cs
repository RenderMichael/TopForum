using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;

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



// Get all subjects

app.MapGet("/topics", () =>
{
    app.Logger.LogInformation("Returning all topics");

    IReadOnlyList<Topic> topics = TopicStorage.Instance.GetAllTopics();
    return Results.Ok(topics);
})
.WithName("GetTopics")
.WithOpenApi();

// Get a single subject

app.MapGet("/topic/{slug}", (string slug) =>
{
    app.Logger.LogInformation("Looking for topic {Slug}", slug);

    Topic? ourTopic = TopicStorage.Instance.FindTopicbySlug(slug);

    if(ourTopic == null)
    {
        app.Logger.LogWarning("{Slug} not found", slug);
        return Results.NotFound();
    }

    app.Logger.LogInformation("Found topic {Slug}", slug);
    return Results.Ok(ourTopic);
})
.WithName("GetTopicBySlug")
.WithDescription("Gets a topic by it's topic name (case-insensitive) or ID")
.WithOpenApi();


app.MapPost("/topic/{topicSlug}/thread", (string topicSlug, [FromBody] CreateThreadModel newThread) =>
{
    bool isValid = MiniValidator.TryValidate(newThread,out var errors);
    if(!isValid)
    {
        app.Logger.LogWarning("Bad Data in new Thread");
        return Results.BadRequest(errors);
    }

    Topic? ourTopic = TopicStorage.Instance.FindTopicbySlug(topicSlug);
    if(ourTopic == null)
    {
        app.Logger.LogWarning("{TopicSlug} not found", topicSlug);
        return Results.NotFound("Topic not found");
    }

    ForumThread thread = new ForumThread(Guid.NewGuid(), newThread.ThreadTitle, newThread.ThreadBody, newThread.AuthorUserName);

    ourTopic.Threads.Add(thread);
    app.Logger.LogInformation("Thread added to Topic");
    return Results.Created((string?) null, thread);
})
.WithName("CreateThreadInTopicBySlug")
.WithDescription("Creates a thread with the Title, Body and Author within a given topic found with slug")
.WithOpenApi();

app.Run();


internal record Topic(Guid Id, string Name, string Description)
{
    public List<ForumThread> Threads { get; } = new List<ForumThread>();
}

internal record ForumThread(Guid Id, string ThreadTitle, string ThreadBody, string AuthorUserName);

internal record CreateThreadModel(
    [Required, MinLength(1)] string ThreadTitle, 
    [Required, MinLength(1)] string ThreadBody, 
    [Required, MinLength(1)] string AuthorUserName);

internal class TopicStorage
{
    private List<Topic> topics =
    [
        new Topic(Guid.Parse("63b4696e-88f5-4411-bc9a-51343c73fa97"), "Cars", "This is for talking all about vehicles."),
        new Topic(Guid.Parse("bc027c5f-5add-491e-878c-e322eea99baf"), "Coffee", "This is for conversations about The Liquid Stuff."),
        new Topic(Guid.Parse("8dc8eda6-7dc8-4b9b-a9e6-fffbf5f025b4"), "Bikes", "Conversations about Biking, urban policy, and other stuff."),
        new Topic(Guid.Parse("ec322b1b-1e2a-4a94-909c-bf94ed738e75"), "Programming", "Software dev convo."),
        new Topic(Guid.Parse("575ced2a-cd43-41f5-b17d-001bbed5fb99"), "Exercise", "Convo about exercise progress."),
    ];

    private TopicStorage()
    {
    }

    public static TopicStorage Instance{ get; } = new TopicStorage();

    public IReadOnlyList<Topic> GetAllTopics()
    {
        return topics;
    }

    public Topic? FindTopicbySlug(string slug)
    {
        foreach (Topic topic in topics)
        {
            if (string.Equals(topic.Name, slug, StringComparison.OrdinalIgnoreCase)
                || string.Equals(topic.Id.ToString(), slug, StringComparison.OrdinalIgnoreCase))
            {
                return topic;
            }
        }

        return null;
    }
}