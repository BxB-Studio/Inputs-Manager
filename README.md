# Inputs-Manager
A new customizable and dynamic Input alternative for Unity based on the New Input System.

**Version:** 1.1.6

## Features
- High-performant code (Using C# Jobs System & Burst)
- Joysticks/Gamepad compatibility
- Runtime double press and hold binding
- Bind keys within the Unity Editor
- Edit inputs at runtime
- Easy input setup
- Save and import presets
- Editor-ready JSON presets
- Fully customizable API
- Dynamic editor window
- Secured data saving
- Wide range of settings

## Documentation
Will be available soon...<br/>
You can use this temporary [tutorial](https://youtu.be/oZlrqwAjiqQ) to help you. You can also use the editor **tooltips** whenever you need something to be explained.

## Important Notes!
**Importing the Inputs Manager manually to your Unity project requires the [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities) library to be added to your project for it to work without any** `MissingReferenceException`**.<br/>
- Use the Automatic install method to avoid importing errors.<br/>
- This package requires a few packages to be installed from the Package Manager. Check the Dependencies section below for more information<br/>
- Please do not change the destination of the existing files under the** `Resources` **directory. Doing so may cause some internal errors!**

## How to Install
There are multiple methods to install the Inputs Manager in your Unity project.<br/>
1. **Automatically: Requires Git to be installed on your machine ([Learn more](https://docs.unity3d.com/Manual/upm-ui-giturl.html))**:
	- Open Unity's Package Manager by going to `Window > Package Manager`.
	- Click on the `+` button on the top left of the Package Manager window.
	- Click on `Add package from git URL`.
	- Enter the following Git URL: `https://github.com/BxB-Studio/Unity-CSharp-Utilities.git` to add the [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities) library.
	- Click the `Add` button.
	- Repeat the same steps to add the Inputs Manager.
	- Click on the `+` button on the top left of the Package Manager window.
	- Click on `Add package from git URL`.
	- Enter the following Git URL: `https://github.com/BxB-Studio/Inputs-Manager.git`.
	- Click the `Add` button.

2. **Manually:**
	- Clone the GitHub repository of the [Unity CSharp Utilities](https://www.github.com/BxB-Studio/Unity-CSharp-Utilities) library.
	- Extract the ZIP archive to a folder.
	- Move the extracted folder to your project's `Assets` folder.
	- Clone the GitHub repository of the [Inputs Manager](https://www.github.com/BxB-Studio/Inputs-Manager) repository.
	- Extract the ZIP archive to a folder.
	- Move the extracted folder to your project's `Assets` folder.

## Start-up Tutorial
The Inputs Manager is available under the `Tools > Utilities > Inputs Manager` menu.<br/>
You can access the Inputs Manager API from the `Utilities.Inputs` namespace.

## Compatibility
- Unity 2020.3.17f1 or newer<br/>

## Dependencies
- Utilities: 1.1.6 or newer
- Collections: 1.2.4 or newer
- Input System: 1.7.0 or newer

## Release Notes
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
