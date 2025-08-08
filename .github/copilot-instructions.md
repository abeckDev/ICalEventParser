# ICalEventParser
ICalEventParser is a .NET 8.0 Azure Functions application that provides an HTTP endpoint to parse iCal feeds and return events as JSON. The function uses the iCal.NET library to process calendar data and includes Azure Application Insights integration.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively
- Bootstrap and build the repository:
  - Ensure .NET 8.0 SDK is installed: `dotnet --version` (should show 8.0.x)
  - `dotnet restore` -- takes 45 seconds to complete. NEVER CANCEL. Set timeout to 90+ seconds.
  - `dotnet build` -- takes 52 seconds to complete. NEVER CANCEL. Set timeout to 120+ seconds.
  - Build produces warnings about nullable reference types - this is expected and does not prevent functionality
- Clean the project: `dotnet clean` -- takes less than 1 second
- Build release configuration: `dotnet build --configuration Release` -- takes 8 seconds. NEVER CANCEL. Set timeout to 60+ seconds.

## Running the Application Locally
- **ALWAYS** run the build steps first before attempting to start the function
- Install Azure Functions Core Tools v4:
  ```bash
  curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /usr/share/keyrings/microsoft-archive-keyring.gpg
  echo "deb [arch=amd64,arm64,armhf signed-by=/usr/share/keyrings/microsoft-archive-keyring.gpg] https://packages.microsoft.com/repos/microsoft-ubuntu-$(lsb_release -cs)-prod $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/microsoft-prod.list
  sudo apt update && sudo apt install -y azure-functions-core-tools-4
  ```
- Create local.settings.json (required for local development):
  ```json
  {
      "IsEncrypted": false,
      "Values": {
          "AzureWebJobsStorage": "UseDevelopmentStorage=true",
          "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
      }
  }
  ```
- Start the function: `func start --build` -- takes 60 seconds to start. NEVER CANCEL. Set timeout to 120+ seconds.
  - When prompted for worker runtime, select option "1" for "dotnet (isolated worker model)"
  - Function will be available at: `http://localhost:7071/api/ParseICalFeed`
- Test the function:
  - Missing parameter: `curl "http://localhost:7071/api/ParseICalFeed"` (should return "Missing 'icalFeedUrl' query parameter.")
  - With parameter: `curl "http://localhost:7071/api/ParseICalFeed?icalFeedUrl=YOUR_ICAL_URL"`

## Validation
- **ALWAYS** manually validate any new code by:
  1. Building the project successfully: `dotnet build`
  2. Starting the Azure Function: `func start --build`
  3. Testing with missing parameter to verify error handling
  4. Testing with a valid iCal URL to verify parsing functionality
- There are no unit tests in this repository - testing must be done manually with the running function
- Build warnings about nullable reference types are expected and do not affect functionality
- The function parses iCal feeds and returns events with these properties: eventTitel, location, eventStart, eventEnd, description

## Project Structure
- **ParseICalFeed.cs**: Main Azure Function containing HTTP trigger endpoint
- **Models/TripEvent.cs**: Data model for parsed calendar events  
- **Program.cs**: Function app startup and configuration
- **host.json**: Azure Functions runtime configuration with Application Insights settings
- **.vscode/**: VS Code configuration with build tasks and launch settings
- **local.settings.json**: Local development configuration (git-ignored)

## Common Tasks
The following are outputs from frequently run commands. Reference them instead of viewing, searching, or running bash commands to save time.

### Repository root structure
```
.git/
.gitignore
.vscode/
ICalEventParser.csproj
ICalEventParser.sln
LICENSE
Models/
ParseICalFeed.cs
Program.cs
Properties/
host.json
local.settings.json (created locally)
```

### Project dependencies (from ICalEventParser.csproj)
- **Target Framework**: net8.0
- **Azure Functions Version**: v4  
- **Key Dependencies**:
  - iCal.NET v5.1.0 (calendar parsing)
  - Microsoft.Azure.Functions.Worker v2.0.0 (isolated worker model)
  - Microsoft.ApplicationInsights.WorkerService v2.23.0 (telemetry)
  - Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore v2.0.2 (HTTP support)

### Build output (bin/Debug/net8.0/)
Contains compiled assemblies, dependencies, and Azure Functions metadata files including:
- ICalEventParser.dll (main assembly)
- functions.metadata (function definitions)
- host.json (runtime config)
- worker.config.json (worker configuration)

## Troubleshooting
- If `func start` hangs at worker runtime selection, ensure you select option "1" for dotnet isolated model
- If function fails to download iCal feeds, verify the URL is accessible and returns valid iCal format
- Build warnings about nullable reference types are expected - the code functions correctly despite these warnings
- The function requires internet access to download external iCal feeds
- Local testing can be done by serving a test iCal file with `python3 -m http.server` and using localhost URLs

## Expected Response Format
The function returns JSON array of events:
```json
[
  {
    "eventTitel": "Event Title",
    "location": "Event Location", 
    "eventStart": "2025-01-01T10:00:00Z",
    "eventEnd": "2025-01-01T11:00:00Z",
    "description": "Event description"
  }
]
```