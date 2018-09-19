# LelShotter
Simple Windows screenshot maker, allowing fullscreen shots or area selected shots and saving result to desktop/imgur/clipboard.

## Requirements
* .NET Framework 4.7.2

## Global hotkeys
App registers two global system hotkeys: `Alt + Shift + W` for capturing fullscreen shot and `Alt + Shift + S` for selected area shot. Currently, hotkeys are not customizable.

## Imgur Upload
To enable Imgur uploading, you'll have to get an API key in user account settings and insert it into `ImageUploader.cs` file.

## Logging
App creates `LelShotter` directory in `ProgramData` and two log files: `LelShotter.err.log` and `LelShotter.out.log`.

## Libraries used
LelShotter uses `Imgur.API` v4.0.1 and `Newtonsoft.Json` v11.0.1.

## Building
Solution is based on Visual Studio 2017 and utilizes C# 7.3 (.NET 4.7.2). MSBuild is

## License
LelShotter is based on MIT license, see `LICENSE` file.
