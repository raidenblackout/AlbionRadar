# Contributing guidelines

If you are looking to contribute to this project. You can follow the instructions below:
1.  **Install Npcap**:
    * Download the latest version of Npcap from the official website: [https://nmap.org/npcap/](https://nmap.org/npcap/)
    * Run the installer and follow the on-screen instructions. It is generally recommended to install with the "Install Npcap in WinPcap API-compatible Mode" option checked.

2.  **Clone the Repository**:
    * Open your Git-enabled terminal or command prompt.
    * Navigate to the directory where you want to store the project.
    * Execute the following command to clone the AlbionRadar repository:
        ```bash
        git clone https://github.com/raidenblackout/AlbionRadar.git
        ```

3.  **Build and Run the Project**:
    * Navigate into the cloned directory:
        ```bash
        cd AlbionRada
        ```
    * Open the `.sln` file in Visual Studio, and build the solution.

## Contribution Types:
Any kind of contributions are accepted, but we would love to see:

1. TypeId and EntityType mappings for more in-game entities. Or corrections to existing mappings. You can find the mappings [here](https://github.com/raidenblackout/AlbionRadar/blob/main/AlbionDataHandlers/Assets/mob_info.json) (*We know there are many wrong mappings*)
2. Refactoring and cleaning out the codebase.
3. Improving the UI/UX of the overlay.
4. Adding new features or improving existing ones.
5. Bug fixes and performance improvements.
6. Documentation improvements.

## TypeId and EntityType mapping:
One way to contribute to this project is by updating *mob_info.json* for correct mappings. Currently, there are many incorrect mappings and many that are not included. So, how do you find out the correct mappings?

1.  Start the overlay.
2.  Go into the game.
3.  Move around and see if the mob or entity you see in-game matches the icons on the overlay radar.
4.  If the icons do not match. Simply note down the number present below the icon on the radar.
5.  Go to the file located at *AlbionDataHandlers/Assets/mob_info.json*.
6.  Search for the TYPE_ID.
7.  If the TYPE_ID is not present, add a new record with the following format:
    ```
    TYPE_ID string: [
      TIER int,
      MOB_TYPE int,
      MOB_SUB_TYPE string
    ],
    ```
    eg.
    ```json
    "562": [
      5,
      1,
      "hide"
    ],
    ```

## DataType info:
**TYPE_ID**
```
Any Integer Shown on the Overlay
```
**TIER:**
```
1 to 8
```

**MOB_TYPE:**
```c#
LivingHarvestable = 0,
LivingSkinnable = 1,
Enemy = 2,
MediumEnemy = 3,
EnchantedEnemy = 4,
MiniBoss = 5,
Boss = 6,
Drone = 7,
MistBoss = 8,
Events = 9,
```

**MOB_SUB_TYPE:**
```
hide
Logs
ore
fiber
rock
CRYSTALSPIDER
VEILWEAVER
FAIRYDRAGON
AVALONMINIONCHEST
GRIFFIN
```
