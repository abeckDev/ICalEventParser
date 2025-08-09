# TripItICalEventParser

An Azure Function that parses iCal feeds and converts them to JSON format, specifically designed for TripIt integration with Microsoft Power Automate.

## Purpose

This project solves a specific integration challenge: **Microsoft Outlook cannot directly work with TripIt's default .ics format**. This Azure Function acts as a bridge, consuming TripIt calendar feeds and outputting structured JSON data that can be easily consumed by Power Automate flows to sync TripIt events into Outlook calendars.

## How It Works

### The Integration Process

1. **Daily Automation**: A Power Automate flow runs once per day
2. **Calendar Cleanup**: All existing events in the dedicated Outlook calendar are deleted
3. **Feed Processing**: The flow calls this Azure Function endpoint with the user's TripIt calendar feed URL
4. **Event Parsing**: The function downloads and parses the iCal feed, extracting event information
5. **JSON Response**: Returns a JSON array containing all events for the current year
6. **Calendar Sync**: Power Automate adds all returned events back to the Outlook calendar

This approach ensures TripIt events are synchronized daily into Outlook, providing a "hack-like but functional" integration solution.

## API Endpoint

### ParseICalFeed Function

**Endpoint**: `GET /api/ParseICalFeed`

**Parameters**:
- `icalFeedUrl` (required): The URL of the iCal feed to parse (typically from TripIt)

**Response**: JSON array of `TripEvent` objects

**Example Request**:
```
GET /api/ParseICalFeed?icalFeedUrl=https://www.tripit.com/feed/ical/private/[YOUR-PRIVATE-KEY]/tripit.ics
```

**Example Response**:
```json
[
  {
    "eventTitel": "Flight to Berlin",
    "location": "Berlin Airport (BER)",
    "description": "Flight details: LH123 from Frankfurt to Berlin",
    "eventStart": "2024-03-15T10:30:00Z",
    "eventEnd": "2024-03-15T12:00:00Z"
  },
  {
    "eventTitel": "Hotel Check-in",
    "location": "Hotel Adlon, Berlin",
    "description": "Reservation confirmation: ABC123",
    "eventStart": "2024-03-15T15:00:00Z",
    "eventEnd": "2024-03-17T11:00:00Z"
  }
]
```

## Event Data Structure

Each event in the response contains the following properties:

- **eventTitel**: The title/summary of the event
- **location**: Event location information
- **description**: Detailed event description
- **eventStart**: Event start date and time (UTC)
- **eventEnd**: Event end date and time (UTC)

## Installation & Deployment

### Prerequisites

- .NET 8.0 SDK
- Azure Functions Core Tools (for local development)
- Azure subscription (for deployment)

### Local Development

1. **Clone the repository**:
   ```bash
   git clone https://github.com/abeckDev/ICalEventParser.git
   cd ICalEventParser
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run locally**:
   ```bash
   func start
   ```

The function will be available at `http://localhost:7071/api/ParseICalFeed`

### Azure Deployment

1. **Create an Azure Function App** in your Azure subscription

2. **Deploy using Azure Functions Core Tools**:
   ```bash
   func azure functionapp publish <YOUR-FUNCTION-APP-NAME>
   ```

3. **Configure Function Authorization**: Ensure the function is set to `Function` authorization level for security

## Features

- **Year-based filtering**: Automatically retrieves events from the current calendar year
- **Timezone handling**: Converts all times to UTC for consistent processing

## Limitations

- **Current year only**: Events are filtered to the current calendar year (January 1 - December 31)
- **Basic event data**: Only extracts essential event information (title, location, description, start/end times)
- **No real-time sync**: Requires daily execution via Power Automate for synchronization

## Dependencies

- **iCal.NET**: Library for parsing iCal format files
- **Microsoft.Azure.Functions.Worker**: Azure Functions runtime
- **Microsoft.ApplicationInsights**: Application monitoring and logging

## Acknowledgments

Thanks to the Team from [iCal.NET](https://github.com/ical-org/ical.net) for providing an easy to use and well documented way on how to deal with iCal files! 

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

This is a specialized tool designed for TripIt-to-Outlook integration via Power Automate. For issues or questions, please open an issue on GitHub.
