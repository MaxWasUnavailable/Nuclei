# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Disable server process' audio on server start, to prevent annoyance in case of running the server on a local machine

## [1.0.1] - 2025-02-12

### Changed

- Filter out any leading slashes from ChatService's message sending, to prevent using e.g. the Say command to send
  commands as the server.

## [1.0.0] - 2025-02-12

Notice: All work up to this point has been consolidated into a single release for the sake of simplicity. This
initial changelog will not be written in exhaustive detail, but will instead provide a high-level overview of
the initial features.

### Added

- Various services for development convenience and streamlining
    - ChatService for sending chat messages from the server to clients
    - MessageService for sending of messages (i.e. host ended message)
    - MissionService for handling of mission selection, validation from config, etc...
    - SteamLobbyService wrapper for Steam Lobby-related calls
    - TimeService for triggering of time events
- NucleiConfig, a configuration class that handles all BepInEx ConfigEntry objects and validation, as well as various
  helper methods
- Server.cs wrapping the entire server & mission start/stop/restart flow, as well as some timed hooks such as the
  message of the day and max mission duration
- Chat Commands, with a CommandService to handle parsing and execution of commands, an ICommand interface for
  command implementations, a PermissionConfigurableCommand base class for commands that allow their permission level
  to be configured, and a number of default commands such as kick, say, stop, newmission, and setpermissionlevel. Also
  includes a PermissionLevel enum for command permission levels, and config entries to assign moderators, admins, and
  the server owner by their Steam ID
- Player, Server, and Time events. To be used by other mod devs and Nuclei itself for event-driven systems
- Various helpers
    - Globals, a convenience class with static variables linking to instances of important game objects
    - PermissionLevelUtils, a utility class for parsing of permission levels from strings and integers
    - PlayerUtils, a utility class for fetching of a Player object from INetworkPlayer or from the GamePlayers list
- A number of patches to allow for better logging and some functionality
    - Patched the chat manager to log chat messages and allow for chat commands without sending the command message
      itself to chat
    - CSteamAPIContext patch to start the server when the game is known to be ready, ensuring consistency
    - MessageManager patches to log join & leave messages to the server console, and to allow for sending of a welcome
      message to new players
    - NetworkServerPatches to fire server events
    - PlayerPatches to change the server client name to "Server", to make it clear when the server is sending a message
      to chat