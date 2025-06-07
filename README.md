# AlbionRadar

AlbionRadar is an experimental tool designed to provide a real-time information overlay for Albion Online, aiming to enhance a player's understanding of their surroundings. This project focuses on displaying contextual information such as the location and type of in-game entities like creatures and resources in a separate overlay.

**Note:** This tool is developed for educational and experimental purposes to explore network packet analysis in the context of game data.

## Features

* **Real-time Entity Tracking**: Visualize the positions and types of various in-game entities, including creatures and resources.
* **Detailed Information Display**: Get insights into the tier and type of detected entities.
* **Intuitive Overlay**: Information is presented in a separate, non-intrusive window.

## Requirements

To run AlbionRadar, you will need the following:

* **Npcap**: This project relies on [Npcap](https://nmap.org/npcap/) for capturing network packets. Please ensure Npcap is installed on your system before attempting to run AlbionRadar. Npcap provides the necessary drivers and libraries for packet sniffing on Windows.

## Installation and Setup

You have two options for getting AlbionRadar:

### Option 1: Download the Latest Release (Recommended for most users)

For users who prefer not to build the project from source, you can download the latest pre-compiled release:

1.  **Install Npcap**:
    * Download the latest version of Npcap from the official website: [https://nmap.org/npcap/](https://nmap.org/npcap/)
    * Run the installer and follow the on-screen instructions. It is generally recommended to install with the "Install Npcap in WinPcap API-compatible Mode" option checked.

2.  **Download AlbionRadar**:
    * Go to the [AlbionRadar v0.0.4 Release Page](https://github.com/raidenblackout/AlbionRadar/releases/tag/v0.0.4)
    * Download the executable or compiled package for your system.

3.  **Run the Application**:
    * Extract the downloaded files (if it's a zip/archive).
    * Run the main executable file (e.g., `AlbionRadar.exe`).
    * **Network Adapter Selection**: The first time you run AlbionRadar, it will prompt you to select your network adapter for packet capture.
        * **For Wi-Fi Users**: Select your active **Wi-Fi adapter**.
        * **For Ethernet Users**: Select your active **Ethernet adapter**.
        ![Network Adapter Selection Prompt](https://github.com/raidenblackout/AlbionRadar/blob/main/Assets/PacketDeviceSelectorDialogue.png?raw=true)
        * Upon successful selection, AlbionRadar should launch its separate information overlay window.
        ![AlbionRadar In-Game Overlay](https://github.com/raidenblackout/AlbionRadar/blob/main/Assets/AlbionRader.png?raw=true)
### Option 2: Build from Source (For Developers)

If you are a developer and wish to contribute or modify the project, you can build it from the source code:

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
        cd AlbionRadar
        ```
    * This project is built with C# and .NET. Please follow the standard build and run procedures for that environment.
        * Open the `.sln` file in Visual Studio, build the solution, and then run the project.


## Contributions
Any kind of contributions are accepted, but we would love to see:

1. TypeId and EntityType mappings for more in-game entities. Or corrections to existing mappings. You can find the mappings [here](https://github.com/raidenblackout/AlbionRadar/blob/main/AlbionDataHandlers/Assets/mob_info.json) (*We know there are many wrong mappings*)
2. Refactoring and cleaning out the codebase.
3. Improving the UI/UX of the overlay.
4. Adding new features or improving existing ones.
5. Bug fixes and performance improvements.
6. Documentation improvements.
## Disclaimer

This tool is provided for educational and experimental purposes only. The developers are not responsible for any actions taken by users of this software. Use at your own risk. Please be aware of and adhere to the Terms of Service of Albion Online when using any third-party tools.

## Credits
This tool is a .NET port of the following [ZQRadar](https://github.com/Zeldruck/Albion-Online-ZQRadar) with additional features in mind.
