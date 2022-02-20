# Inputs-Manager
A new customizable and dynamic Input alternative for Unity based on the New Input System.

This packages includes some additional [Unity CSharp Utilities](https://www.github.com/mediamax07/Unity-CSharp-Utilities).

**Version:** 1.0.1

## Start-up Tutorial
The Inputs Manager is available under the `Tools > Utilities > Inputs Manager` menu.<br/>
You can access the Inputs Manager from the `Utilities.Inputs` namespace.

## Documentation
Will be available soon...<br/>
You can use this temporary [tutorial](https://youtu.be/oZlrqwAjiqQ) to help you. You can also you the editor **tooltips** whenever you need something to be explained.

## Features
- Joysticks/Gamepad compatibility
- Runtime double press and hold binding
- Bind keys within the Unity Editor
- Edit inputs at runtime
- High performant code
- Easy inputs setup
- Save and import presets
- Editor ready Json presets
- Fully customizable API
- Dynamic editor window
- Secured data saving
- Wide range of settings

## Compatibility
- Unity 2018.4 or newer<br/>

## Dependencies
- Unity 2018.4
	- Input System: 0.2.1-preview only
- Unity 2019.1 or newer
	- Input System: 1.0.0-preview or newer

## Release Notes
- 1.0.1
	- New updated Utilities library
	- Fixed Gamepad/Joystick detection at runtime
	- Added new icons
	- Deleted unnecessary files
	- Single/Multiple Touch Binding support for Unity 2018.4
- 1.0.0
	- First production release
	- Added joystick/gamepad support
	- Optimized code
	- Optimized performance
	- Fixed bunch of bugs
	- Ability to bind joystick/gamepad inputs
	- Single/Multiple Touch Binding *(Unity 2019.1 or newer; Input System 0.9-preview or newer)*
	- Replaced SVG icons with PNG ones *(Vector Graphics package is no longer required!)*
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
	- Json preset importer

## Important Notes!
**The Inputs Manager requires a Scripting Runtime Version of .NET 4.x Equivalent or newer *(If available; This is only for older Unity versions)*.<br/>
This package requires the New Input System package to be installed from the Package Manager.<br/>
Once the Input System has been installed, a Scripting Define Symbol `ENABLE_INPUT_SYSTEM` would be added; If not, please go to `Edit > Project Settings > Player > Scripting Define Symbols` and add `ENABLE_INPUT_SYSTEM` there. *On Unity 2019.4 or older; If `Scripting Define Symbols` is not empty, you can separate symbols with a `;`.*<br />
Please do not changes the destination of the existing files under the `Resources` directory. Doing so may cause some internal errors!**

## Contributing
Please read the [contributing guidlines](https://github.com/mediamax07/Inputs-Manager/blob/master/CONTRIBUTING.md) for this repository.

## License
This project is under the MIT license. [Read more about it](https://github.com/mediamax07/Inputs-Manager/blob/master/LICENSE.md).
