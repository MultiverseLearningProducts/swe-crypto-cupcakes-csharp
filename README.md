# SWE Crypto Cupcakes - C#

Welcome to SWE Crypto Cupcakes, C# Edition! The feature branches in this repo represent the evolution of a sample app. Each week we can demo a new branch, look at what has changed and why:

1. `cupcakes-api`
2. `security` (in progress)
3. `jwt` (in progress)
4. `oauth` (in progress)

The `main` branch is the same as the finished `oauth` project branch after the 4 weeks of delivery, so get started at the first branch to see the app from the very beginning. (in progress)

## Installation

Install the following:
- IDE such as Visual Studio Code (VS Code) or Visual Studio
- If using VS Code, install the following VS Code C# extension: [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- Install .NET: [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) - or preferred version
- Optionally, you may also install the following VS Code extensions:
    - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
    - [.NET MAUI](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)
    - [Unity](https://marketplace.visualstudio.com/items?itemName=visualstudiotoolsforunity.vstuc)

## Setup
- Clone this repository.
- Open up this project in your IDE of choice such as VS Code or Visual Studio.

## Run Application Locally

Trust HTTP certificate

```bash
  dotnet dev-certs https --trust
```

Start the server

```bash
  dotnet run --launch-profile https
```

## Testing

Test the API endpoints by:
- visting the https URL generated in the terminal with /swagger appended (e.g. https://localhost:7234/swagger)
- using a tool such as Postman or Thunder Client (https://localhost:7234)