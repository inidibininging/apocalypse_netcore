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


### : and !

You can create functions with the ":" symbol and call it via "!".

Example:
```
:myfunction
!CallSomeFunctionAsynchronously

:CallSomeFunctionAsynchronously
Wait +666 Seconds
```

### @> and <@
the "@>" calls a factory and creates an instance. the entity made with the the factory made by the factory gets an alias as assignment. In the example below is ".weakEnemy"


```
:MakeAWeakEnemy
@>AliasOfEntityFactoryOnlyInAlphaNumeric .weakEnemy
Mod Stats .weakEnemy +123 Health
Mod Stats .weakEnemy +10 Speed
Mod Scale .weakEnemy +1 X
Mod Scale .weakEnemy +1 Y
```

the "<@" deletes the assignment

You CAN delete the assignment if you want or you can leave it and use the assignment later on in another function.


```
:MakeAWeakEnemy
@>AliasOfEntityFactoryOnlyInAlphaNumeric .weakEnemy
Mod Stats .weakEnemy +123 Health
Mod Stats .weakEnemy +10 Speed
Mod Scale .weakEnemy +1 X
Mod Scale .weakEnemy +1 Y
<@ .weakEnemy

:UseWeakEnemyLater
@>AliasOfEntityFactoryOnlyInAlphaCharacter .weakEnemy
Mod Stats .weakEnemy +123 Health
Mod Stats .weakEnemy +10 Speed
Mod Scale .weakEnemy +1 X
Mod Scale .weakEnemy +1 Y

:IAmUsingWeakEnemyAndRemovingItLater
Mod Stats .weakEnemy +1 Health
<@ .weakEnemy
```

### Wait
You can wait for a specific amount of time. It can be Seconds, Miliseconds or Minutes. 

```
:DoALittleDance
Wait +2 Minutes

:DoAnotherDance
Wait +10 Seconds

:Salsa
Wait +1 Miliseconds
```

Waiting is the only way for now to pause an async function. So this would be a way to make a loop without breaking the game:

The code below puts the play in position 
x: 512, y: 512 
every 5 seconds.

```
:Start
Wait +5 Seconds
!CenterPlayers

:CenterPlayers
Mod Position .Players +512 X
Mod Position .Players +512 Y
!Start

```

So if you dont wait, you will likely get a StackOverflowException or OutOfMemoryException

### Mod

Mod modifies a value of a game entity (specifically a CharacterEntity)

For more examples you can look into "apocalypse.echse"

#### Scripting Language Limitations

- there is no possibility now for writing comments or calling functions synchronously
- names are only limited to letters WithoutSpaceInBetween
- some stuff is broken
- numbers must be written, either with a "+" or "-" prefix.

