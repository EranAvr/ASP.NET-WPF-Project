using MongoDB.Driver;
using MongoDB.Bson;

var connectionString = "mongodb://localhost:27017";
var client = new MongoClient(connectionString);
var collection = client.GetDatabase("C_SHARP_FINAL");
if (collection == null)
    Environment.Exit(-1);
Console.WriteLine(collection);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
