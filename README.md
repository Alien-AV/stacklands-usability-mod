# stacklands-usability-mod
## Motivation
This mod fixes a few usability and balance issues in the epic game [Stacklands](https://store.steampowered.com/app/1948280/Stacklands/).
## Changes
- Packs price increases temporarily (for 5 seconds) by 1 coin per each 2 packs bought.
- Holding Shift while dragging any card in a stack drags the whole stack.
- Markets spit out money below them (and into a chest if one is present there).
- Animal pens hold up to 5 animals.
- Zoom out limit higher.
## Installation (for users)
Extract the zip from releases into your game directory, next to Stacklands.exe.
Run the game.
## Compilation (for devs)
Some project references will be broken for you, you need to change those that begin with "Unity" to point to relevant files from Stacklands install directory, for example *[SteamAppsDir]\Stacklands\Stacklands_Data\Managed\UnityEngine.CoreModule.dll*

Build.

Use a DLL loader, such as [Unity Doorstop](https://github.com/NeighTools/UnityDoorstop) to inject the resulting DLL into the game. Make sure *0Harmony.dll* is present at runtime as well.