using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var blogs = new List<Blog>
{
    new Blog {Title = "My first post", Body = "This is my first post" },
    new Blog {Title = "My second post", Body = "This is my second post" }
};


app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Duration: {duration}");
});

app.Use(async (context, next) =>
{
    Console.WriteLine(context.Request.Path);
    await next.Invoke();
    Console.WriteLine(context.Response.StatusCode);
});

app.UseWhen(
    context => context.Request.Method != "GET",
    appBuilder => appBuilder.Use(async (context, next) =>
    {
        var extractedPassword = context.Request.Headers["X-Api-Key"];
        if (extractedPassword == "password")
        {
            await next.Invoke();
        }else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API key");
        }
    })
);

app.MapGet("/", () => "Hello World!");

app.MapGet("/blogs", () =>
{
    return Results.Ok(blogs);
});

app.MapGet("/blogs/{id}", (int id) =>
{
    if (id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }else
    {
        return Results.Ok(blogs[id]);
    }
});

app.MapPost("/blogs", (Blog blog) =>
{
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});

app.MapDelete("/blogs/{id}", (int id) =>
{
    if (id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }else
    {
        blogs.RemoveAt(id);
        return Results.NoContent();
    }
});

app.MapPut("/blogs/{id}", (int id, Blog blog) =>
{
    if (id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }else
    {
        blogs[id] = blog;
        return Results.Ok(blog);
    }
});

app.Run();

public class Blog
{
    public required string Title { get; set;}
    public required string Body {get; set;}
}