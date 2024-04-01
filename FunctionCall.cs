namespace TucanScript.Core;

public class FunctionCall : Entity
{
    private readonly List<Operable?> _arguments;
    private readonly Operable _result;

    public FunctionCall(Function? function)
    {
        _arguments = new List<Operable?>();
        _result = new Operable();
        Function = function;
        Type = EntityType.Call;
    }
    
    private Function? Function { get; }

    public void AppendArg(Operable? argument) => _arguments.Add(argument);

    public Operable Execute()
    {
        if (Function == null)
            return _result;
        
        for (var index = 0; index < _arguments.Count; index++)
        {
            var argument = _arguments[index];
            if (argument is IResetable resetable and not Expression)
                resetable.Reset();

            if (argument is Expression argExpression)
                argExpression.Execute();

            Function.SetArgValue(index, _arguments[index]);
        }
        
        Function.InvolvedArgumentCount = _arguments.Count;
        
        Function.Execute();
        _result.Set(Function);

        return _result;
    }
}