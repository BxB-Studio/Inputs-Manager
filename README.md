# Inputs-Manager
A new customizable and dynamic Input alternative for Unity based on the New Input System.

This packages includes some additional [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities).

**Version:** 1.1.1

## Start-up Tutorial
The Inputs Manager is available under the `Tools > Utilities > Inputs Manager` menu.<br/>
You can access the Inputs Manager API from the `Utilities.Inputs` namespace.

## Documentation
Will be available soon...<br/>
You can use this temporary [tutorial](https://youtu.be/oZlrqwAjiqQ) to help you. You can also you the editor **tooltips** whenever you need something to be explained.

## Features
- High performant code (Using C# Jobs System)
- Joysticks/Gamepad compatibility
- Runtime double press and hold binding
- Bind keys within the Unity Editor
- Edit inputs at runtime
- High performant code
- Easy inputs setup
- Save and import presets
- Editor ready JSON presets
- Fully customizable API
- Dynamic editor window
- Secured data saving
- Wide range of settings

## Compatibility
- Unity 2020.3.17f1 or newer<br/>

## Dependencies
- Collections: 1.2.4 or newer
- Input System: 1.7.0 or newer

## Release Notes
- 1.1.1
	- Added default gamepad override
	- Optimized inputs calculation
	- Fixed input constructor
- 1.1.0
	- Added C# Jobs System integration
	- Updated Utilities library
- 1.0.3
	- Added a default Inputs Manager data asset file
	- Some minor API syntax improvements
	- A small code refresh
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
	- Fixed bunch of bugs
	- Ability to bind joystick/gamepad inputs
- 0.4-beta
	- Fixed some performance issues
	- Fixed some bugs
- 0.3-beta
	- Verified the Inputs Manager code
	- Added the ability to check for inputs double press
	- Optimized the code by adding some updating improvements and removed some unnecessary lines
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
	- Bind keys in editor
	- JSON preset importer

## Important Notes!
**The Inputs Manager requires an API Compatibility Level of .NET 4.x or newer (If available).<br/>
This package requires the New Input System package to be installed from the Package Manager.<br/>
Please do not changes the destination of the existing files under the** `Resources` **directory. Doing so may cause some internal errors!**

## Contributing
Please read the [contributing guidelines](https://github.com/BxB-Studio/Inputs-Manager/blob/master/CONTRIBUTING.md) for this repository.

## License
This project is under the MIT license. [Read more about it](https://github.com/BxB-Studio/Inputs-Manager/blob/master/LICENSE.md).
