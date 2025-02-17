# macio utility #

"macio" is (.NET 8.0 based) cross-platform command-line tool and supporting library for working with older Macintosh volumes and drive images.
It is intended to ease working with both physical machines and emulators and going between them, with a particular
focus on SD card drive emulators.

- Listing drive image and volume information
- Modifying partition info
- Creating new blank drive images
- Extracting and replacing partitions in drive images

I started this project to work with my 68K Macs. I'll be expanding this to improve my own workflows and satisfy my
curiosity. Feedback and participation is welcome.

There is no GUI for this yet and I haven't quite decided what UI stack I intend to use when/if I start creating a GUI.

## Building ##
Building can be done on any platform .NET 8.0 supports (Mac, Linux, Windows). 

To build from the command line ensure you have the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed. From the root of the project, simply run `dotnet build -c release`.
The command line tool will output to `artifacts\AnyCPU\Release`.

If you want to build self-contained single-file executables see ["Building Single File"](BuildingSingleFile.md).

You can also build from [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) by opening the `MacIO.sln`. I've created some self-contained, trimmed, "publish" targets for
Windows x64 and OSX x64. Select the "macio" project and "Publish" from the "Build" menu in Visual Studio.

## Basics ##

Run `macio` or `macio help` to get basic information.

```
A utility for manipulating and creating Macintosh 68K drive images.
 (https://github.com/JeremyKuhne/MacIO)

Basic usage:

 >macio {task} [parameters]
 >macio help {task}

Task              Aliases   Summary
----------------- --------- --------------------------------------------
info                        Lists the summary info for the image.
blockzero         bz        Lists the zero block information.
listpartitions    lp        Lists the partitions in the image.
deletepartition   dp        Deletes the specified partition.
checksum          hash, cs  Generates an MD5 checksum for a file.
extractdriver     ed        Extracts the specified driver code.
extractpartition  ep        Extracts the specified partition.
replacepartition  rp        Replaces a partition with a specified file.
hfsinfo           hfs, hi   Lists information for HFS volumes.
createimage       ci        Creates an empty image of the given size.

>macio help lp
Lists drive image partition info

 > macio listpartitions {driveImage} [-index:{index}]

Option Details:
---------------

-index (-i) Get detailed information for the specified partition
```

See the [Wiki](https://github.com/JeremyKuhne/MacIO/wiki) for more information.

## Contributing ##
Contribution is welcome! Please respect the [Code of Conduct](CODE_OF_CONDUCT.md).
## License ##
Released under the MIT license, see the LICENSE file for more information.
