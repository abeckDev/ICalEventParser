using System;

namespace AbeckDev.ICalEventParser.Models;

public class TripEvent
{
    public string EventTitel { get; set; }

    public string Location { get; set; }

    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; }

    public string Description { get; set; }

}
