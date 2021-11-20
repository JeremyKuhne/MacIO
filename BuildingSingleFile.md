# Building Single File #

To build the command line tool as a single file:

- Change to the "macio" subdirectory under the repo root
- Use the `dotnet publish` command to publish the desired OS

```
C:\Users\jerem\source\repos\MacIO\macio>dotnet publish -c:release --os:win
Microsoft (R) Build Engine version 17.0.0+c9eb9dd64 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  xTask -> C:\Users\jerem\source\repos\MacIO\artifacts\release\xTask\net6.0\xTask.dll
  MacIOLibrary -> C:\Users\jerem\source\repos\MacIO\artifacts\release\MacIOLibrary\net6.0\MacIOLibrary.dll
  macio -> C:\Users\jerem\source\repos\MacIO\artifacts\release\macio\net6.0\win-x64\macio.dll
  macio -> C:\Users\jerem\source\repos\MacIO\artifacts\release\macio\net6.0\win-x64\publish\
```

The last line in the output is the publish folder where you'll find the executable.

OS options:

- `--os:win`
- `--os:linux`
- `--os:osx`

You can also specify minimum versions for Windows and Macintosh:

- `--os:win10`
- `--os:osx.11.0`

You can further specify the architecture by adding the `--arch` option:

- `--arch:x86`
- `--arch:x64`
- `--arch:arm`
- `--arch:arm64`

And finally you can specify that you want the framework to be fully contained by adding `--self-contained:true`.
This will make a (much larger) executable that contains the .NET Framework. Unused code is trimmed by default.

Here is an example of using all of the options:

```
C:\Users\jerem\source\repos\MacIO\macio>dotnet publish -c:release --os:osx.11.0 --arch:arm64 --self-contained:true
Microsoft (R) Build Engine version 17.0.0+c9eb9dd64 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored C:\Users\jerem\source\repos\MacIO\xTask\xTask.csproj (in 3.74 sec).
  Restored C:\Users\jerem\source\repos\MacIO\MacIOLibrary\MacIOLibrary.csproj (in 3.74 sec).
  Restored C:\Users\jerem\source\repos\MacIO\macio\macio.csproj (in 3.74 sec).
  xTask -> C:\Users\jerem\source\repos\MacIO\artifacts\release\xTask\net6.0\xTask.dll
  MacIOLibrary -> C:\Users\jerem\source\repos\MacIO\artifacts\release\MacIOLibrary\net6.0\MacIOLibrary.dll
  macio -> C:\Users\jerem\source\repos\MacIO\artifacts\release\macio\net6.0\osx.11.0-arm64\macio.dll
  Optimizing assemblies for size, which may change the behavior of the app. Be sure to test after publishing. See: https://aka.ms/dotnet-illink
  macio -> C:\Users\jerem\source\repos\MacIO\artifacts\release\macio\net6.0\osx.11.0-arm64\publish\
```

Full runtime identifier information can be found [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).
