// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Tasks;

public class TaskService : ITaskService
{
    private readonly string _generalHelpString;

    private readonly Lazy<SimpleTaskRegistry> _taskRegistry;

    protected TaskService(string? generalHelpString = null)
    {
        _taskRegistry = new Lazy<SimpleTaskRegistry>(() =>
        {
            return new SimpleTaskRegistry();
        });

        _generalHelpString = generalHelpString ?? Resources.HelpGeneral;
    }

    public virtual void Initialize()
    {
        SimpleTaskRegistry registry = GetTaskRegistry();

        // These commands are provided as part of the xTask framework
        registry.RegisterTask(() => new HelpTask(registry, _generalHelpString), "help", "?");
        registry.RegisterDefaultTask(() => new UnknownTask(registry, _generalHelpString));
    }

    protected virtual SimpleTaskRegistry GetTaskRegistry() => _taskRegistry.Value;

    public ITaskRegistry TaskRegistry => GetTaskRegistry();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
