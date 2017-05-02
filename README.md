Men & Monsters 
============
A networked game built for CIS 4398. Men & Monsters is an Android and iOS compatible multiplayer, networked game, inspired by the classic board game Dungeons & Dragons. Players choose a character with distinct attributes and abilities. They then join a team of other players whom work together to progress to the end of the map. Along the way, the players have turn-based combat against monsters, encounter non-player characters, and make decisions to help gain an advantage over the final boss. Chat functionality and dynamically drawn mazes create a unique game experience every time.

## Implemented features
* Main Menu
  * Play / Quit button
* Lobby
  * Create new game
    * Optionally password protect game
  * Join an existing game
    * Enter password if one is set
  * Character selection
    * Disable character already chosen by another player
* Combat
  * Navigation controls for map
    * Enter combat room once the character icon has moved to opening via collision detection
  * In combat room:
    * Allow players to select from various attack to use on enemy for their turn
      * Enemy selects random player(s) to attack & deals random amount of damage within a range
  * Sync player stats accros all devices in the game
  * Allow players to chat with each other
  * End game once all players have died or all enemies have been defeated



## Installation and use
**Via binary: _(Android only)_**

1. Download the latest release for Android
1. Unzip the file
1. Connect your Android smartphone via USB to a computer
1. Drag the unzipped APK file to your Android device listed on your computer's connected devices
1. On your Android device tap the APK file to complete the installation

**From source:**

1. Download [Unity](https://unity3d.com/get-unity/download)
1. Download and unzip the latest released Men & Monsters source
1. Open Unity
1. Select "Open"
1. Navigate to the location of the unzipped source code
1. Select the main directory and click "Open"
1. From the Unity project tree select `MainMenu`
1. Double click the asset labeled `MainMenu` with a Unity logo icon
1. Click "Run" at the top of the Unity editor

## Known bugs
* When the host leaves the game, a new host is not assigned. Instead the lobby is destroyed and the players a pushed to the Lobby screen
* The position in the party of the player who leaves the game is not able to be filled by another player joining after the game has been starting. After the lobby enters the game no other players are able to join
* The character portrait is not greyed out for other players in the lobby, however the same character cannot be chosen by another player once it is chosen
