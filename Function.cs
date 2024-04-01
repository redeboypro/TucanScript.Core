namespace TucanScript.Core;

public class Function : Operable, IExecutable, IReturnable
{
    private readonly Dictionary<string, Argument> _arguments;
    private bool _inProgress;

    public Function()
    {
        Executables = new List<IExecutable>();
        _arguments = new Dictionary<string, Argument>();
    }
    
    public int InvolvedArgumentCount { get; set; }
    
    public List<IExecutable> Executables { get; }

    public Argument GetArg(int index) => _arguments.Values.ElementAt(index);

    public Operable? GetArgOperable(string argName) => _arguments[argName].Variable;

    public void Append(IExecutable executable) => Executables.Add(executable);

    public void AppendArg(string argName, bool isReference)
    {
        _arguments.Add(argName, new Argument
        {
            IsReference = isReference,
            Variable = new OperableClone(null)
        });
    }

    public void SetArgValue(int index, Operable? operable)
    {
        if (index >= _arguments.Count)
            AppendArg($"arg{index}", false);
        
        var argument = GetArg(index);
        SetArgValue(argument, operable);
    }

    public void Execute()
    {
        _inProgress = true;
        
        foreach (var executable in Executables)
        {
            executable.Execute();
            
            if (!_inProgress)
                return;
        }

        foreach (var argument in _arguments.Values)
        {
            var argumentOperable = argument.Variable;

            if (argumentOperable != null && argument.IsReference)
                argumentOperable.Apply();
        }
    }

    public void Return() => _inProgress = false;

    public void Reset()
    {
        foreach (var executable in Executables)
            executable.Reset();
    }

    private static void SetArgValue(Argument argument, Operable? operable)
    {
        var variable = argument.Variable;
        variable?.SetSource(operable);
        variable?.Reset();
    }
}