using Bubblefy.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddBubblefyServices();

var app = builder.Build();
app.Services.InitializeBubblefyDatabase();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.Run();
