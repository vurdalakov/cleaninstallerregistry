# CleanInstallerRegistry

[CleanInstallerRegistry](https://github.com/vurdalakov/cleaninstallerregistry) is a command line tool that fixes errors in [Windows Installer](https://en.wikipedia.org/wiki/Windows_Installer) Registry.

Microsoft support site has a [great tool](https://support.microsoft.com/en-us/topic/fix-problems-that-block-programs-from-being-installed-or-removed-cca7d1b6-65a9-3d98-426b-e9f927e1eb4d) that fixes problems that block programs from being installed or removed, but in many cases it also damages the Windows Installer Registry entries that leads to strange problems. For example, files of a program are not being removed even after "successful" uninstallation of this program.

So I created `CleanInstallerRegistry` tool and run it right after running Microsoft troubleshooter.

## Links

* [Fix problems that block programs from being installed or removed](https://support.microsoft.com/en-us/topic/fix-problems-that-block-programs-from-being-installed-or-removed-cca7d1b6-65a9-3d98-426b-e9f927e1eb4d)
* [CleanInstallerRegistry GitHub repository](https://github.com/vurdalakov/cleaninstallerregistry)

## License

`CleanInstallerRegistry` program is distributed under the terms of the [MIT license](https://opensource.org/licenses/MIT).
