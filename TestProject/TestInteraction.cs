// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Logging;
using xTask.Systems.File.Concrete;

namespace TestProject;

/// <summary>
///  Simple test task interaction.
/// </summary>
public class TestInteraction : TaskInteraction
{
    /// <summary>
    ///  Combined logger.
    /// </summary>
    public TextLogger Logger { get; }

    public TestInteraction(
        IFileService fileService,
        IConsoleService consoleService,
        params string[] arguments)
        : this(new TextLogger(), arguments, fileService, consoleService)
    {
    }

    public TestInteraction(
        IConsoleService consoleService,
        params string[] arguments)
        : this(new TextLogger(), arguments, consoleService: consoleService)
    {
    }

    public TestInteraction(params string[] arguments)
        : this(new TextLogger(), arguments)
    {
    }

    private TestInteraction(
        TextLogger logger,
        string[] arguments,
        IFileService? fileService = null,
        IConsoleService? consoleService = null)
        : base(CreateArguments(
            fileService ?? new FileService(),
            consoleService ?? new ResponseConsoleService(),
            logger,
            arguments))
    {
        Logger = logger;
    }

    private static (IArgumentProvider, IServiceProvider) CreateArguments(
        IFileService fileService,
        IConsoleService consoleService,
        ILogger logger,
        params string[] arguments)
    {
        CommandLineParser parser = new(fileService);
        parser.Parse(arguments);

        SimpleServiceProvider serviceProvider = new();
        serviceProvider.AddService(arguments);
        serviceProvider.AddService(fileService);
        serviceProvider.AddService(consoleService);
        serviceProvider.AddService<ILoggers>(new SingleLogger(logger));
        return (parser, serviceProvider);
    }
}
