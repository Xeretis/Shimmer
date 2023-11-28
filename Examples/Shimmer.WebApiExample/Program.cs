using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Shimmer;
using Shimmer.AspNetCore;
using Shimmer.Services;
using Shimmer.WebApiExample.Jobs;
using Shimmer.WebApiExample.Jobs.Data;
using Shimmer.WebApiExample.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddShimmer().DiscoverJobs(Assembly.GetExecutingAssembly());
builder.Services.AddShimmerHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async ([FromServices] IShimmerJobFactory shimmerJobFactory) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();

        var job = await shimmerJobFactory.CreateAsync<LogWeatherJob, LogWeatherJobData>();

        await job.Data(new LogWeatherJobData
        {
            Forecasts = forecast.ToList()
        }).FireAsync();

        var checkWeatherJobTree = await shimmerJobFactory.CreateTreeAsync<CheckWeatherJob>();

        checkWeatherJobTree.AddDependentJob<SendWeatherNotificationJob>(j => j.StartAt(DateTime.Now.AddSeconds(30)));

        await checkWeatherJobTree.FireAsync();

        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();