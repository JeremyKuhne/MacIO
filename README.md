# macio utility #

"macio" is (.NET 6.0) cross-platform command-line tool and supporting library for working with older Macintosh volumes and drive images.
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
Ensure you have the .NET 6.0 SDK installed. Either open the solution and build, or run `dotnet build -c release`.

I've created some self-contained, trimmed, "publish" targets for Windows x64 and OSX x64. Select the "macio" project
and "Publish" from the "Build" menu in Visual Studio.

## Basics ##

Run `macio` or `macio help` to get basic information.

```
>macio
A utility for manipulating and creating Macintosh 68K drive images.

Task              Aliases   Summary
----------------- --------- --------------------------------------------
info                        Lists the summary info for the image.
blockzero         bz
listpartitions    lp        Lists the partitions in the image.
deletepartition   dp        Deletes the specified partition.
checksum          hash, cs  Generates an MD5 checksum for a file.
extractdriver     ed        Extracts the specified driver code.
extractpartition  ep        Extracts the specified partition
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

## Contributing ##
Contribution is welcome for all changes that are free to be put under the current Copyright and license. Other changes will be considered as well.
## License ##
Released under the MIT license, see the LICENSE file for more information.