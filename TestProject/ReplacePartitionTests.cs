// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class ReplacePartitionTests
{
    [Fact(Skip = "For manual testing.")]
    public void ReplaceImageStub()
    {
        ReplacePartitionTask task = new();

        TestInteraction interaction = new(
            new ResponseConsoleService
            {
                NextReadLine = "yes"
            },
            "rp", @"\\?\PhysicalDrive11", @"T:\ide\untitled(1.9 GB Apple_HFS)", "i:4");

        task.Execute(interaction);
    }

    [Fact]
    public void NoTargetReturnsError()
    {
        ReplacePartitionTask task = new();
        TestInteraction interaction = new("replacepartition");
        ExitCode exitCode = task.Execute(interaction);
        exitCode.Should().Be(ExitCode.InvalidArgument);
        interaction.Logger.ToString().Should().StartWith("Error: This command requires a target.");
    }

    [Fact]
    public void NoSourceReturnsError()
    {
        ReplacePartitionTask task = new();
        TestInteraction interaction = new("replacepartition", "foo");
        ExitCode exitCode = task.Execute(interaction);
        exitCode.Should().Be(ExitCode.InvalidArgument);
        interaction.Logger.ToString().Should().StartWith("Error: Need a target image and a source partition file.");
    }
}
