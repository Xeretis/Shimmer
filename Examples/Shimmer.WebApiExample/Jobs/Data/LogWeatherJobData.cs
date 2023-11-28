using Shimmer.WebApiExample.Models;

namespace Shimmer.WebApiExample.Jobs.Data;

public class LogWeatherJobData
{
    public List<WeatherForecast> Forecasts { get; set; }
}