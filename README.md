# KzBot - Tibia 12 Private

Legacy C#/.NET 6.0 Windows Forms project for character automation on OT (Open Tibia) servers using protocol 12.x.

This repository is published as a historical and educational learning artifact. It is not maintained, not supported, and should not be used against active services.

## Status

Historical/study project, not a complete product. The implementation documents a period of hands-on learning in desktop automation, process memory reading, external API integration, and game behavior scripting.

The repository is kept as a record of the learning journey and the solution I was trying to build at the time (2022-2023).

## What the project attempted to do

- Automate characters on OT protocol 12.x servers (versions 12.90, 13.05, 13.10, 13.12).
- Read Tibia client state through version-specific memory addresses.
- Run cavebot with customizable waypoints (navigation, loot, refill, equip, imbue, travel).
- Manage targeting with per-creature, vocation, and priority rules.
- Healer system with rules configured by HP/MP, potion, and spell.
- Alarm system to detect situations like stuck, anti-bug, player on screen, and PZ.
- Telegram integration for notifications.
- CapSolver integration for captcha solving.
- Centralized hub to manage multiple characters/accounts simultaneously.
- Auto leveling system for automated character login.
- Account creator with random name/password generation.

## Stack

- C#
- .NET 6.0 Windows Forms
- Telegram.Bot
- CapSolver
- Tesseract (OCR)
- Newtonsoft.Json
- WinAPI (memory reading, input simulation)
- XML serialization for scripts/config

## Project Structure

### KzBot (main bot)

- `Adresses/`: version-specific client memory addresses (12.90, 13.05, 13.10, 13.12).
- `Objects/`: game state reading (Player, Creature, Battlelist, Client).
- `Threads/`: execution loops for Cavebot, Healer, Targeting, Alarms, ClientData.
- `UI/`: Windows Forms forms for each system.
- `Util/`: helpers for WinAPI, keyboard, screen capture, enums.
- `Script.cs`: script model with waypoints, targeting/healer/refill/alarms rules.
- `Globals.cs`: shared bot runtime state.

### HUB (multi-account controller)

- `Classes/`: server, client, script, character, connection models.
- `UI/`: forms for managing servers, clients, characters, scripts, connections.
- `Util/`: random name/password generation.
- `Program.cs`: loads config from `Config.xml` and starts the HUB.

## Project relationship

KzBot is the individual bot that operates one character. HUB is the manager that coordinates multiple KzBot instances, passing parameters such as server, client, account, character, and script via command line.

## Weak points and improvements

These points are intentionally included in the README to make the project's real level clear and what I would do differently today:

| Weak point in the old version | What should be improved |
| --- | --- |
| Hardcoded memory addresses per version, no integrity validation. | Extract addresses to external version profiles, validate signatures before use, and fail safely if the version is unknown. |
| Server, client, and account configuration in external `Config.xml` with no documented schema or example. | Document the XML schema, provide `Config.example.xml` without real values, and validate on load. |
| Static global state in `Globals` with shared timers and threads. | Separate services with dependency injection, clear lifecycle, and isolated per-character state. |
| UI, automation logic, memory reading, and domain rules mixed in the same code. | Separate into layers: memory reader, domain model, action executor, and interface. |
| Broad error handling with generic `catch` and no context (especially in `Main.cs`). | Structured logging by context, differentiate expected errors from critical ones, and graceful recovery. |
| Timers based on `System.Threading.Timer` without `CancellationToken` or coordinated shutdown. | Use `async Task`, `CancellationToken`, backoff on failures, and controlled shutdown with health checks. |
| No automated tests, CI, or verifiable build. | Add tests for script parsing, targeting/healer rules, and connection logic. |
| `CapSolver` (1.2.0) dependency may be less accessible or discontinued. | Abstract captcha solving service with an interface and support multiple providers. |
| `.png` assets in the root with non-descriptive names (`701.png`, `okbutton.png`, `teste.png`). | Organize resources in `Assets/` folder with descriptive names and remove unused files. |
| Code comments mixed in English and Portuguese, some incomplete or misleading. | Keep documentation in one language and use code documentation tools. |
| Project vocabulary reflects the original online game automation context. | Keep the honest history, but present the repository as a learning archive and discuss the knowledge gained in terms of desktop automation, WinAPI, and tooling. |
| The entire project depends on a specific game client and old versions. | For a modern educational project, create standalone examples that work with any Windows application, without dependency on a specific game. |

## Publication note

This repository can be read as legacy code and learning material. It should not be used as a current example of security, architecture, or deployment. The decision here is to preserve the original repository and history, making it clear that old operational values are discarded and that the code does not represent how I would implement a similar solution today.

## How to evaluate this repository

The value of this project lies more in the historical context than in the final code quality: it shows a practical attempt at building desktop automation in C#/.NET with memory reading, API integration, behavior scripting, and multi-account management. I would present this version as an old/unfinished learning project, not as current code or an architecture reference.
