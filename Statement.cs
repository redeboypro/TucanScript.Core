namespace TucanScript.Core;

public abstract class Statement : IExecutable, IExecutableContainer
{
    public List<IExecutable> Executables { get; }
    
    protected Statement()
    {
        Executables = new List<IExecutable>();
    }

    public void Reset()
    {
        foreach (var executable in Executables)
            executable.Reset();
    }

    public void Append(IExecutable executable) => Executables.Add(executable);

    public abstract void Execute();
}