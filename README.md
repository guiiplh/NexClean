# NexClean

**A lightweight Windows cleanup utility built with Godot and C#/.NET.**

NexClean is a Windows utility designed to provide quick access to system cleanup operations through the Windows context menu.

## Features

- **Temporary Files Cleanup:** Removes temporary files from selected Windows system directories, skipping files that are locked or inaccessible.
- **RAM Optimization:** Attempts to reduce the working set of the NexClean process.
- **Standby Memory Cleanup:** Requests Windows to purge the Standby List.

## Technologies

- Godot Engine 4.7.2
- C#
- .NET 8
- GDScript
- Windows API

## Requirements

- Windows 10 or later
- Godot Engine 4.7.2 .NET edition
- .NET 8 SDK

## Getting Started

1. Clone this repository:

   ```bash
   git clone https://github.com/guiiplh/NexClean.git
   ```

2. Open the project using Godot 4.7.2 .NET.
3. Build the C# project.
4. Export the project for Windows.

## Windows Context Menu

NexClean integrates with the Windows context menu, providing quick access to its cleanup operations.

The installer registers the available commands for files, folders, and the desktop background.

## Important Notes

- Windows may recreate temporary files and cached data after cleanup.
- Some files cannot be removed while they are in use or protected by Windows.
- RAM optimization currently targets the NexClean process itself. It does not clear the working sets of other applications.
- Standby List cleanup uses an undocumented Windows native API. Its behavior and availability may vary between Windows versions.
- Administrative permissions may be required for certain operations.

## Project Status

NexClean is under active development.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

**Developed by NexClean.**