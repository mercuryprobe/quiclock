# Quiclock Plan

## Summary

Quiclock is a native Windows background app built with `C#` and `WPF`.
It runs at login, listens for a configurable global shortcut, opens a small
input window, accepts a duration, and starts one or more concurrent timers.
After a timer starts, the input window hides immediately. When a timer
finishes, the app shows a Windows toast notification and plays a short sound.

## Core Product Decisions

- Primary platform: Windows desktop
- Tech stack: `C#` + `WPF`
- App model: background tray app
- Startup behavior: launch at Windows login
- Timer model: multiple concurrent timers
- Trigger: one configurable global hotkey
- Completion behavior: Windows notification plus sound

## Main Behaviors

### Global Shortcut

- Register a default shortcut such as `Ctrl+Alt+T`
- Allow the shortcut to be changed in a settings UI
- Keep the shortcut active while the app runs in the background

### Quick Timer Input

- When the hotkey is pressed, show a compact always-on-top input window
- Focus the input field immediately
- Hide the window as soon as a valid timer is created

### Accepted Duration Formats

- Whole minutes, for example `5`
- Decimal minutes, for example `1.5`
- Raw seconds with suffix, for example `90s`
- `mm:ss` format, for example `2:30`

### Timer Lifecycle

- Support multiple active timers at once
- Track each timer independently
- Allow individual timers to be canceled from the active timers view
- On completion, notify once and mark the timer complete

## UI Surfaces

### Tray Menu

- Open timer input
- View active timers
- Settings
- Quit

### Active Timers Window

- Show all running timers
- Display remaining time for each timer
- Allow canceling individual timers

### Settings Window

- Edit global shortcut
- Toggle launch at login
- Toggle completion sound

## Internal Structure

### Models

- `AppSettings`: hotkey definition, run-at-startup flag, sound enabled flag
- `TimerEntry`: id, parsed duration, start time, due time, status
- `HotkeyBinding`: modifiers plus key

### Services

- `HotkeyService`: register and unregister the global shortcut
- `TimerService`: create, track, complete, and cancel timers
- `NotificationService`: dispatch toast notifications and sound
- `StartupService`: manage Windows login startup registration
- `SettingsService`: load and save local config

## Persistence

- Persist settings to a local JSON config file under the user profile
- Do not persist active timers across app restarts in v1

## Test Cases

- Global shortcut opens the input window while another app is focused
- Changing the shortcut updates registration correctly
- Invalid or conflicting shortcuts are rejected clearly
- `5` starts a 5-minute timer
- `1.5` starts a 90-second timer
- `90s` starts a 90-second timer
- `2:30` starts a 2-minute 30-second timer
- Empty, zero, negative, and malformed inputs are rejected
- Starting a timer hides the input window immediately
- Multiple timers run concurrently without interfering
- Canceling one timer does not affect the others
- Timer completion shows one toast notification
- Completion sound only plays when enabled
- Exiting the app unregisters the hotkey cleanly

## Explicit v1 Non-Goals

- Named timers
- Timer pause and resume
- Timer history
- Persisting active timers across restart
- Preset timer buttons
