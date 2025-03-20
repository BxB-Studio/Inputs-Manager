# Inputs-Manager
A comprehensive, high-performance input management system for Unity built on the New Input System.

**Version:** 1.1.14

## Features
- **High Performance**: Leverages C# Jobs System & Burst compilation for optimal speed
- **Multi-Device Support**: Full compatibility with keyboards, mice, gamepads, and joysticks
- **Advanced Input Detection**: Double press, hold, and release detection
- **Flexible Configuration**:
  - Editor-based key binding
  - Runtime input editing
  - Preset management (save/import)
  - JSON-based configuration
- **Developer-Friendly**:
  - Intuitive API
  - Dynamic editor window
  - Extensive customization options
- **Robust Architecture**:
  - Secure data handling
  - Configurable settings
  - Optimized for performance

## Documentation
Comprehensive documentation is coming soon!<br/>
In the meantime:
- Watch our [video tutorial](https://youtu.be/oZlrqwAjiqQ) for a quick start guide
- Refer to the editor **tooltips** for contextual help with specific features

## Important Notes
**Dependencies**: The Inputs Manager requires the [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities) library to function properly.<br/>
- **Recommended**: Use the Automatic installation method to avoid dependency issues
- **Required Packages**: See the Dependencies section below for required Unity packages
- **File Structure**: Do not relocate files from the `Resources` directory as this may cause internal errors

## Installation Methods

### 1. Automatic Installation (Recommended)
Requires Git on your machine ([Unity Documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html))

**Step 1: Install Unity CSharp Utilities**
1. Open Unity's Package Manager (`Window > Package Manager`)
2. Click the `+` button in the top-left corner
3. Select `Add package from git URL`
4. Enter: `https://github.com/BxB-Studio/Unity-CSharp-Utilities.git`
5. Click `Add`

**Step 2: Install Inputs Manager**
1. In Package Manager, click the `+` button again
2. Select `Add package from git URL`
3. Enter: `https://github.com/BxB-Studio/Inputs-Manager.git`
4. Click `Add`

### 2. Manual Installation
1. Download the [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities) repository
2. Extract the ZIP archive
3. Move the extracted folder to your project's `Assets` directory
4. Download the [Inputs Manager](https://www.github.com/BxB-Studio/Inputs-Manager) repository
5. Extract the ZIP archive
6. Move the extracted folder to your project's `Assets` directory

## Getting Started
- Access the Inputs Manager through `Tools > Utilities > Inputs Manager`
- Use the API via the `Utilities.Inputs` namespace
- Configure inputs through the editor or at runtime via code

## System Requirements
- Unity 2020.3.17f1 or newer

## Dependencies
- Utilities: 1.1.8 or newer
- Collections: 1.2.4 or newer
- Input System: 1.7.0 or newer

## Release Notes
- 1.1.14
	- Added code summary documentation
	- Fixed `InvalidOperationException`
	- Fixed `IndexOutOfRangeException`
- 1.1.13
	- Fixed `IndexOutOfRangeException` for `DefaultGamepadIndexFallback`
- 1.1.12
	- Upgraded `Utilities` package to `1.1.8`
- 1.1.11
	- Fixed `IndexOutOfRangeException` when getting values from an invalid gamepad index
- 1.1.10
	- Upgraded `Utilities` package to `1.1.7`
- 1.1.9
	- Fixed data save bug
	- Optimized data load
	- Changed gamepad index type from `byte` to `sbyte`
- 1.1.8
	- Added `CloseWindow` method
	- Replaced `OpenInputsManager` with `OpenWindow`
- 1.1.7
	- Changed data asset path
	- Replaced `DataAssetFullPath` with `EditorDataAssetFullPath`
	- Replaced `SaveData` with `EditorSaveData`
- 1.1.6
	- Upgraded to Utilities 1.1.6
	- Fixed un-disposed `inputsGamepadAccess` array
- 1.1.5
	- Fixed gamepads detection
- 1.1.4
	- Updated obsolete code
- 1.1.3
	- Improved exceptions handling
	- Updated `Readme.md`
- 1.1.2
	- Fixed backward compatibility for Unity 2020.3
- 1.1.1
	- Added default gamepad override
	- Optimized input calculation
	- Fixed input constructor
- 1.1.0
	- Added C# Jobs System integration
	- Updated Utilities library
- 1.0.3
	- Added a default Inputs Manager data asset file
	- Some minor API syntax improvements
	- A small code Refresh
- 1.0.2
	- Easier input edit through scripting
	- Added scripting custom data loading
	- Edit settings in Play Mode in Editor & through scripting
- 1.0.1
	- New updated Utilities library
	- Fixed Gamepad/Joystick detection at runtime
- 1.0.0
	- First production release
	- Added joystick/gamepad support
	- Optimized code
	- Optimized performance
	- Fixed a bunch of bugs
	- Ability to bind joystick/gamepad inputs
- 0.4-beta
	- Fixed some performance issues
	- Fixed some bugs
- 0.3-beta
	- Verified the Inputs Manager code
	- Added the ability to check for inputs double press
	- Optimized the code by adding some updating improvements and removing some unnecessary lines
	- Updated the Utilities library
	- Fixed some major bugs
- 0.2-beta
	- Added individual Keys binding methods (InputKey, InputKeyDown, InputKeyUp)
	- Fixed some bugs
- 0.1-beta.6
	- First official beta release
	- Fixed some bugs
- 0.1-beta.5
	- Fixed some bugs
- 0.1-beta.4
	- First Pre-Release
- 0.1-preview.3
	- Fixed the Editor compatibility for Unity 2019.3
- 0.1-preview.2
	- Added some meta files for the Editor icons
- 0.1-preview.1
	- Fixed some compatibility issues
- 0.1-preview (First Commit)
	- Add, remove, duplicate inputs
	- Bind keys in the editor
	- JSON preset importer

## Contributing
Please read the [contributing guidelines](https://github.com/BxB-Studio/Inputs-Manager/blob/master/CONTRIBUTING.md) for this repository.

## License
This project is under the MIT license. [Read more about it](https://github.com/BxB-Studio/Inputs-Manager/blob/master/LICENSE.md).
