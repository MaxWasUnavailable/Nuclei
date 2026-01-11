# Nuclei

Nuclei is a dedicated server solution for Nuclear Option. It is designed to offer a flexible, easy-to-use, and
easy-to-modify server solution. It is built on top of the [BepInEx](https://docs.bepinex.dev/index.html) modding
framework, and hence requires a BepInEx installation to run.

## Features

- Native support for chat commands with permission levels, with a number of default commands such as `ban`, `kick`,
  `say`, `stop`, `newmission`, and `setpermissionlevel`. Modders can easily add their own commands, and server owners
  can configure which roles can access what commands, and assign moderator and admin roles to specific users.
- Extensive configurability
    - Message of the day
    - Welcome message
    - Maximum mission duration
    - Mission rotation
    - Mission selection mode (Ranom, Random with no direct repeats, and sequential)
    - Moderator, admin, & owner
    - Steam vs UDP
    - Command permission levels
    - Max player count
    - Steam lobby type (public, friends-only, etc...)
    - And *much* more
- Modular and robust design, allowing for easy extension, modification, and maintenance
- A variety of dynamic placeholders that can be used in the server name, message of the day, and welcome message. See
  [Dynamic Placeholders](#dynamic-placeholders) for more information.
- Allows you to disable the server game audio on server start, to make it less annoying to run a server in the
  background

## Installation

1. [Install BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html)
2. Download the latest release of Nuclei from the [releases page](https://github.com/MaxWasUnavailable/Nuclei/releases)
3. Extract the contents of the archive into the `BepInEx/plugins` folder in your Nuclear Option installation directory
4. Copy the `run_server.bat` file from the `Nuclei` folder to the root of your Nuclear Option installation directory
5. Run the copied script to start the server once, and then close it. This will generate a`MaxWasUnavailable.Nuclei.cfg`
   file in the `BepInEx/config` folder, which you can edit to configure the server
6. After configuring the server, run the script whenever you want to start the server

## Dynamic Placeholders

The following dynamic placeholders are supported in the server name, message of the day, and welcome message:

- `{server_name}`: The name of the server
- `{player_name}`: The name of the player (only available in the welcome message)
- `{player_name_censored}`: The name of the player, censored using the game's censoring rules (only available in the
  welcome message)
- `{steamid}`: The Steam ID of the player (only available in the welcome message)
- `{mission_name}`: The name of the current mission
- `{faction1_name}`: The name of the first faction in the current mission
- `{faction2_name}`: The name of the second faction in the current mission
- `{faction1_tag}`: The tag of the first faction in the current mission
- `{faction2_tag}`: The tag of the second faction in the current mission
- `{faction1_score}`: The score of the first faction in the current mission
- `{faction2_score}`: The score of the second faction in the current mission
- `{all_missions}`: A list of all missions in the mission rotation
- `{random_3_missions_etc}`: A list of 3 random missions from the mission rotation, with (...) at the end if there are
  more than 3 missions

## Developer Documentation

Extensive effort has been put into making Nuclei easy to extend and modify. The codebase is documented through XML
docstrings, and the code is structured in a way that makes it easy to understand and modify. An attempt was made to
keep functionality modular and decoupled.

To create new chat commands, it is recommended to inherit from the `PermissionConfigurableCommand` class, which allows
users to configure the permission level required to execute the command. Once implemented, all you need to do is ensure
your mod is loaded after Nuclei, and you run `CommandService.RegisterCommand` with an instance of your command and your
BepInEx plugin's Config instance.

For an example of how to create a new chat command, see
the [SayCommand](Nuclei/Features/Commands/DefaultCommands/SayCommand.cs).

## Contributing

Contributing follows the typical [GitHub flow](https://guides.github.com/introduction/flow/). To contribute, fork the
repository, make your changes on a well-named branch, and then submit a pull request. Please ensure your changes are
well-documented and tested.

New commands are always welcome, though they might be better suited as a separate mod if they are not "core" to the
server experience. If you are unsure, feel free to open an issue to discuss your idea.

Large changes and new features should be discussed in an issue before being implemented, to ensure they align with the
goals of the project.

Code styling follows the default Rider/ReSharper settings.

If you're interested in discussing the project, or need help, feel free to contact me on the Nuclear Option Discord. My
username is `maxwasunavailable`.

You may also contact me via my [personal modding server](https://discord.com/invite/8PJeFeMCsu).

## Credits

- [Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/), since without it, we couldn't make mods
  for it.
- [JetF0x](https://github.com/JetF0x), for creating
  the [first dedicated server solution for Nuclear Option](https://github.com/JetF0x/NO-ServerHost), and kindly
  licensing it under the MIT license so others can learn from it.
