![](https://github.com/inidibininging/apocalypse_netcore/blob/master/apocalypse_logo_glitch.png)
# apocalypse_netcore
Game developed for my own self-teaching purpouses

![](https://github.com/inidibininging/apocalypse_netcore/blob/master/apocalypse_screen_01.png)
# What is this?
I developed this "game" with a little gaming framework on top of the MonoGame Framework in order to understand some principles and problems while making games.

The idea evolved into a non-working multiplayer game, that is byfar unfinished and without a final goal. Consider the current state as 'when it's done'.


The final view that I had for this game is a space hack-n-slay game that can function as a plattform for other games (thats the game client in code here).

The other games should be played in the same universe (game server) but from other devices and apps, for example a smart-phone, raspberry pi, telegramm etc.

Every device should have a certain perspective/view to the game and play a different role.

So, if you are interested: 

`Help is always welcomed`

---
Here are some features buried in the game code:

- Game object graph
- Split screen
- Multiplayer capability
- Sector-based game server
- Scripting language
- Language CLI for testing the language
- Web-interface
- CLI for send remote commands to the game server
- Collision for non-rotating rectangles ( client and server-side )
- Wrapper for the MonoGame sound api
- RPG stats
- Inventory
- State machine - based game server
- Planet generator
- Possiblity to make sectors within the game at runtime


## Some words regarding other frameworks
I know that there are some cool frameworks on top of MonoGame like MonoGame.Extended that can handle many things and tackle many problems including all that I confronted here, but my intention was to understand the problems and see how It works and not to have a working framework that does everything for you.


---
![](https://github.com/inidibininging/apocalypse_netcore/blob/master/apocalypse_screen_02.png)
# Installation

## Requirements
Things you need for the code to compile:
- .NET Core 2.1 SDK (client/server) https://dotnet.microsoft.com/download/dotnet-core/2.1
- Monogame https://www.monogame.net/downloads/
- Monogame Pipeline (included in some installations)
- Git Bash

## Installation Steps
- build the content pipeline with the MonoGame Pipeline 
- build the client
- create a folder in the clients output folder named "Content"
- copy all files within Apocalypse.Any.Client\Content\bin\DesktopGL into the "Content" folder
- build the server
- change the path to the game server (StartupScript) in gameserver_config.yaml
- start the server
- start the client

## Font Issues
If the client crashes and complaints because the font is missing, use another font with the monogame pipeline and retry.

![](https://github.com/inidibininging/apocalypse_netcore/blob/master/apocalypse_screen_03.png)
## Other Media
The license for sounds and images is stored in this repository under "Apocalypse.Any.Client\Content\LICENSE"

## Scripting Language

I provided a half-working scripting language. Here are 

some examples of what you can do:
[Link to wiki](https://github.com/inidibininging/apocalypse_netcore/wiki/Scripting-Language-examples)
