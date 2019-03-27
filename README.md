# Powershell Rainmeter

A simple skin / plugin pair that adds a skin-able Powershell window onto the desktop, using rainmeter.

<p align="center"><img src="https://i.redd.it/7g6ueanvwqh21.gif"></p>

## Install

To use all the dependencies of this:
- Download [Rainmeter](https://www.rainmeter.net/)
- Download [Ubuntu Mono](https://design.ubuntu.com/font/)
- Download [Monoid](https://larsenwork.com/monoid/)
- Download [AutoHotKey](https://www.autohotkey.com/)

To install the skin itself:
- Move plugin/Powershell.dll into your rainmeter plugin directory (%APPDATA%/Roaming/Rainmeter/Plugins/ for instance)
- Move skin/main.ini and reclick.ahk into a new plugin folder (Documents/Rainmeter/Skins/Powershell/ for instance)
- restart rainmeter
- edit reclick.ahk to click at the input box. This will depend on your positioning, size, and resolution.

## To Customize

Most of the visual aspects of the skin are controlled by variables:
```
[Variables]
BackgroundColor=0C1620
InputBackgroundColor=0C1620
FontColor=F8F2FF
FontFace="Ubuntu Mono"
FontSize=10
PromptFace="Monoid"
Width=630
Height=1020
```

The elements to look at are:
- `[Background]` is the background panel.
- `[OutputText]` is the console output.
- `[MeterSearchLabel]` is the > prompt.
- `[MeasureSearchInput]` is the input box for console input.
