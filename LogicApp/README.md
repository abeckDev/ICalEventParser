# Azure Logic App - TripIt to Outlook Sync

This directory contains the Azure Logic App template that provides automated daily synchronization between TripIt calendar feeds and Microsoft Outlook calendars.

## Overview

The Logic App (`tripit-outlook-sync.json`) implements a complete automation workflow that:

1. **Runs daily at midnight** (W. Europe Standard Time)
2. **Fetches travel events** from TripIt via the Azure Function
3. **Cleans existing events** from the designated Outlook calendar
4. **Creates new events** in Outlook based on the latest TripIt data

## Workflow Steps

### 1. Daily Trigger
- **Frequency**: Every day at 00:00 (midnight)
- **Timezone**: W. Europe Standard Time
- **Type**: Recurrence trigger

### 2. Get TripIt Calendar Feed
- **Action**: HTTP GET request to the Azure Function
- **Purpose**: Retrieve parsed iCal events as JSON from TripIt
- **Configuration Required**:
  - `<Azure Function URL>`: Your deployed Azure Function endpoint
  - `<Azure Function Key>`: Function access key for authentication
  - `<TripIT - FeedUrl>`: Your private TripIt iCal feed URL

### 3. Parse JSON Response
- **Action**: Parse the JSON array returned by the Azure Function
- **Schema**: Validates the expected event structure with fields:
  - `eventTitel` (string): Event title
  - `location` (string): Event location
  - `eventStart` (string): Start date/time
  - `eventEnd` (string): End date/time
  - `description` (string): Event description

### 4. Get Existing Outlook Events
- **Action**: Retrieve all events from the target Outlook calendar
- **Purpose**: Prepare for cleanup to prevent duplicates
- **Calendar ID**: Pre-configured calendar identifier (encoded)

### 5. Delete Existing Events
- **Action**: Loop through and delete all existing events
- **Purpose**: Clean slate approach to prevent duplicates and ensure accuracy
- **Process**: For each event, perform DELETE operation via Outlook API

### 6. Create New Events
- **Action**: Loop through parsed TripIt events and create new Outlook events
- **Process**: For each TripIt event:
  - Map `eventTitel` → `Subject`
  - Map `eventStart` → `Start`
  - Map `eventEnd` → `End`
  - Map `description` → `Body` (formatted as HTML paragraph)
  - Map `location` → `Location`

## Configuration Requirements

### Before Deployment
1. **Azure Function**: Deploy the TripItICalEventParser Azure Function
2. **Outlook Connection**: Set up Outlook API connection in Azure
3. **Calendar Setup**: Create or identify the target Outlook calendar

### Template Placeholders to Replace
- `<Azure Function URL>`: Replace with your Function App URL (e.g., `https://your-function-app.azurewebsites.net/api/ParseICalFeed`)
- `<Azure Function Key>`: Replace with your function's access key
- `<TripIT - FeedUrl>`: Replace with your TripIt private iCal feed URL (e.g., `https://www.tripit.com/feed/ical/private/YOUR-KEY/tripit.ics`)

### Connection Parameters
The template includes pre-configured connection parameters for:
- **Subscription ID**: `b814d650-0963-4701-9dc9-feffc970ad6a`
- **Resource Group**: `tripiteventfeedparser`
- **Location**: `germanywestcentral`

**Note**: Update these values to match your Azure environment.

## Deployment Instructions

1. **Import the Logic App**:
   - In Azure Portal, go to Logic Apps
   - Click "Add" → "Consumption (Multi-tenant)"
   - Choose "Code view" and paste the contents of `tripit-outlook-sync.json`

2. **Configure Connections**:
   - Set up the Outlook connection when prompted
   - Authorize access to your Outlook/Office 365 account

3. **Update Parameters**:
   - Replace all placeholder values with your actual URLs and keys
   - Update the calendar ID to target your desired Outlook calendar

4. **Test the Logic App**:
   - Use "Run Trigger" to test the workflow manually
   - Check the run history for any errors
   - Verify events appear in your Outlook calendar

## Calendar ID Information

The template uses a specific calendar ID (encoded) that points to a dedicated calendar. To use your own calendar:

1. **Find Calendar ID**:
   - Use Outlook API Explorer or Graph Explorer
   - List calendars: `GET https://graph.microsoft.com/v1.0/me/calendars`
   - Find your target calendar and copy its ID

2. **Update Template**:
   - Replace the long encoded string in both "Get" and "Create" actions
   - Use the new calendar ID (URL-encoded if necessary)

## Monitoring and Troubleshooting

### Common Issues
- **Authentication Errors**: Verify Outlook connection is properly authorized
- **Function Errors**: Check Azure Function logs if HTTP requests fail
- **Calendar Errors**: Ensure calendar ID is correct and accessible

### Monitoring
- **Logic App Runs**: Monitor execution history in Azure Portal
- **Function Insights**: Use Application Insights for Function performance
- **Outlook Events**: Verify events are created successfully in target calendar

## Cost Considerations

This Logic App runs daily and performs the following billable actions:
- 1 HTTP request (to Azure Function)
- 1 JSON parse operation
- Multiple Outlook API calls (get, delete, create operations)

The exact cost depends on:
- Number of existing events to delete
- Number of new events to create
- Your Azure subscription pricing tier

## Security Notes

- **Function Keys**: Store Azure Function keys securely
- **Calendar Access**: Logic App requires full calendar access permissions
- **Data Processing**: TripIt calendar data passes through Azure Function and Logic App
- **Connection Security**: Outlook connection uses OAuth 2.0 authentication