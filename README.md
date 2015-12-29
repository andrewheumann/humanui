![Human UI](https://bytebucket.org/andheum/humanui/raw/14a8fde782f47a804ee68263b62447c114670fb9/SupportingMaterials/Logo2.png "Human UI")
# README #

On behalf of NBBJ Design Computation, I’m excited to announce the release of a new plug-in for Grasshopper: Human UI. 

Human UI lets you construct clean, WPF-based user interfaces for your Grasshopper definitions, without writing a single line of code.

This plug-in has been in development at NBBJ for the past year, and we are very excited to share it with you. It has been a huge asset in our own internal tool development, and we have decided to release it as an open source project so that all can benefit from it (and improve on it)!

Going well beyond the capabilities of the “remote control panel,” Human UI makes it easier to create a user-facing display for your Grasshopper definition – one that looks and feels like a Windows app – so you can use GH to build tools for other designers or even clients, without exposing the Grasshopper interface at all.

### What is this repository for? ###

* Human UI - WPF GUIs in Grasshopper
* Beta 0.6

### How do I get Human UI? ###
* Summary of set up

### Releases ###
* #### 0.6.0.0 - Initial Public / Open Source release

### Dependencies ###
* Rhino + RhinoCommon.dll
* Grasshopper
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
* [HelixToolkit.WPF](https://github.com/helix-toolkit)
* [Xceed.WPF](http://wpftoolkit.codeplex.com/)

### Known Issues ###
* Deleting the component that launched a currently open window can cause crashes
* Switching back from a subsidiary GH doc back to the parent doc with a HUI window can spawn unwanted extra windows

### Credits ###
Major contributions to this project so far:

* Initial Development: **Andrew Heumann** / andheum / [@andrewheumann](https://twitter.com/andrewheumann)

* Product Management: **Marc Syp** / marcsyp / [@mpsyp](https://twitter.com/mpsyp)

* Graphing Components: **Nate Holland** / nateholland / [@_NateHolland](https://twitter.com/_NateHolland)