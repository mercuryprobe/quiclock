# Build Instructions

## Prerequisites

- Windows 10 or Windows 11
- .NET 8 SDK
- Optional: Visual Studio 2022 with the `.NET desktop development` workload

## Build From a Terminal

From the repository root:

```bash
dotnet restore Quiclock.sln
dotnet build Quiclock.sln
```

For a release build:

```bash
dotnet build Quiclock.sln -c Release
```

## Run Tests

```bash
dotnet test Quiclock.sln
```

## Run the App

```bash
dotnet run --project src/Quiclock/Quiclock.csproj
```

The app is a Windows-only WPF tray application. It should be built and run on Windows.

## Publish

Example release publish:

```bash
dotnet publish src/Quiclock/Quiclock.csproj -c Release -r win-x64 --self-contained false
```

Published output will be placed under:

```text
src/Quiclock/bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/
```

## Visual Studio

1. Open `Quiclock.sln`.
2. Select `Debug` or `Release`.
3. Build the solution.
4. Set `Quiclock` as the startup project and run it.
