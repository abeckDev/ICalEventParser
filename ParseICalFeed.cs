using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AbeckDev.ICalEventParser.Models;
using System.Globalization;

namespace AbeckDev.ICalEventParser;

public class ParseICalFeed
{
    private readonly ILogger<ParseICalFeed> _logger;

    public ParseICalFeed(ILogger<ParseICalFeed> logger)
    {
        _logger = logger;
    }

    [Function("ParseICalFeed")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string icalFeedUrl = req.Query["icalFeedUrl"];
        if (string.IsNullOrEmpty(icalFeedUrl))
        {
            return new BadRequestObjectResult("Missing 'icalFeedUrl' query parameter.");
        }

        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(icalFeedUrl);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download iCal feed.");
            return new BadRequestObjectResult("Failed to download iCal feed.");
        }

        var calendar = Ical.Net.Calendar.Load(await response.Content.ReadAsStringAsync());

        //Define the Start date of when to start retrieving the events.  
        var Start = new CalDateTime(new DateTime(DateTime.UtcNow.Year, 1, 1), "Europe/Berlin");

        var events = calendar.GetOccurrences(Start)
        .Select(o => o.Source)
        .Cast<CalendarEvent>()
        .Distinct()
        .ToList();

        //string responseString = "Begin Event Parsing: \n";
        var responseEvents = new List<TripEvent>();
        foreach (var calendarEvent in events)
        {
            responseEvents.Add(new TripEvent
            {
                EventTitel = calendarEvent.Summary,
                Location = calendarEvent.Location,
                Description = calendarEvent.Description,
                EventStart = calendarEvent.Start.AsUtc,
                EventEnd = calendarEvent.End.AsUtc,

            });

            //foreach (var prop in calendarEvent.GetType().GetProperties())
            //{
            //    var value = prop.GetValue(calendarEvent, null);
            //    responseString += $"{prop.Name}: {value}\n";
            //}
            //responseString += "-----------NEXT ITEM COMING UP---------------\n";
        }

        return new OkObjectResult(responseEvents);

    }
}