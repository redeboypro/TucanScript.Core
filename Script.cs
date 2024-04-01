using System.Text;

namespace TucanScript.Core;

public class Script : IExecutable, IReturnable
{
    private readonly Dictionary<string, Operable?> _variables;
    private readonly Dictionary<string, Function> _functions;
    private bool _inProgress;

    public Script()
    {
        _variables = new Dictionary<string, Operable?>();
        _functions = new Dictionary<string, Function>();
        Executables = new List<IExecutable>();
    }

    public void LoadFromSource(string source)
    {
        var rawTokens = TokenUtil.Tokenize(source);
        ProcessContainer(rawTokens, this, this);
    }
    
    public List<IExecutable> Executables { get; }

    public bool TryGetVariable(string name, out Operable? variable)
    {
        return _variables.TryGetValue(name, out variable);
    }

    public void SetVariable(string name, Operable variable)
    {
        if (TryGetVariable(name, out var storedVariable))
        {
            storedVariable?.Set(variable);
            return;
        }
        _variables.Add(name, variable);
    }

    public bool TryGetFunction(string name, out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }

    public void SetFunction(string name, Function function)
    {
        if (TryGetFunction(name, out var storedFunction))
        {
            storedFunction?.Set(function);
            return;
        }
        _functions.Add(name, function);
    }

    public void Execute()
    {
        _inProgress = true;
        foreach (var executable in Executables)
        {
            executable.Execute();
            
            if (!_inProgress)
                break;
        }
    }

    public void Reset()
    {
        foreach (var executable in Executables)
            executable.Reset();

        foreach (var function in _functions)
            function.Value.Reset();
    }

    private void ProcessContainer(
        IReadOnlyList<Entity> tokens,
        IExecutableContainer container, 
        IReturnable lastReturnable,
        ILoop? lastLoop = null)
    {
        var executables = container.Executables;
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];

            void SkipToken() => token = tokens[++index];
            string GetUndefinedContent() => (token as Undefined)?.Content!;

            
            switch (token.Type)
            {
                case EntityType.Undefined:
                    executables.Add(ProcessExpression(lastReturnable, tokens, index, out index));
                    break;
                case EntityType.Def:
                {
                    SkipToken();
                    var function = new Function();
                    var functionName = GetUndefinedContent();
                    
                    SkipToken();
                    while (token.Type != EntityType.Semicolon)
                    {
                        if (token.Type == EntityType.Ref)
                        {
                            SkipToken();
                            function.AppendArg(GetUndefinedContent(), true);
                        }
                        else
                            function.AppendArg(GetUndefinedContent(), false);
                        
                        SkipToken();
                    }
                    
                    _functions.Add(functionName, function);
                    break;
                }
                case EntityType.Imp:
                    SkipToken();
                    var function_stored = _functions[GetUndefinedContent()];
                    var functionInternalTokens = CollectInternalTokens(tokens, index + 1, out index);
                    ProcessContainer(functionInternalTokens, function_stored, function_stored);
                    break;
                case EntityType.If:
                case EntityType.While:
                    var conditionExpression = ProcessExpression(lastReturnable, tokens, index + 1, out index);
                    var statementInternalTokens = CollectInternalTokens(tokens, index, out index);
                    if (token.Type == EntityType.If)
                    {
                        var conditionInstance = new If(conditionExpression);
                        ProcessContainer(statementInternalTokens, conditionInstance, lastReturnable, lastLoop);
                        executables.Add(conditionInstance);
                        break;
                    }
                    var loopInstance = new While(conditionExpression);
                    ProcessContainer(statementInternalTokens, loopInstance, lastReturnable, loopInstance);
                    executables.Add(loopInstance);
                    break;
                case EntityType.For:
                    var variables = new List<Operable?>();
                    
                    var variableToken = tokens[++index];
                    while (variableToken.Type != EntityType.In)
                    {
                        if (variableToken is Undefined undefinedToken)
                            variables.Add(ProcessVariable(undefinedToken.Content, container));
                        
                        variableToken = tokens[++index];
                    }

                    var arrayExpression = ProcessExpression(lastReturnable, tokens, index + 1, out index);
                    var forStatementInternalTokens = CollectInternalTokens(tokens, index, out index);
                    var forLoopInstance = new For(variables, arrayExpression);
                    ProcessContainer(forStatementInternalTokens, forLoopInstance, lastReturnable, forLoopInstance);
                    executables.Add(forLoopInstance);
                    break;
                case EntityType.Return:
                    executables.Add(new Return(lastReturnable));
                    break;
                case EntityType.Break:
                    executables.Add(new Break(lastLoop));
                    break;
                case EntityType.Continue:
                    executables.Add(new Continue(lastLoop));
                    break;
            }
        }
    }

    private IReadOnlyList<Entity> CollectInternalTokens(IReadOnlyList<Entity> tokens, int index, out int newIndex)
    {
        var rawTokenBuffer = new List<Entity>();
        
        var token = tokens[index];
        
        if (token.Type != EntityType.BeginBlock)
            throw new Exception("Unexpected token");
        
        void SkipToken() => token = tokens[++index];
        
        SkipToken();

        var bracketCount = 1;

        if (token.Type == EntityType.EndBlock)
            goto end;
        
        while (bracketCount > 0)
        {
            switch (token.Type)
            {
                case EntityType.BeginBlock:
                    bracketCount++;
                    break;
                case EntityType.EndBlock:
                {
                    bracketCount--;
                    if (bracketCount == 0)
                        continue;
                    break;
                }
            }

            rawTokenBuffer.Add(token); 
            SkipToken();
        }
        
        end: 
        newIndex = index;
        return rawTokenBuffer;
    }

    private Expression ProcessExpression(IExecutableContainer? parent, IReadOnlyList<Entity> tokens, int index, out int newIndex)
    {
        var expression = new Expression();
        while (index < tokens.Count)
        {
            var currentToken = tokens[index];

            if (currentToken.Type is EntityType.Semicolon or EntityType.BeginBlock or EntityType.Comma) break;
            
            if (currentToken is Undefined undefinedToken)
            {
                var rawContent = undefinedToken.Content;
                
                var nextIndex = index + 1;
                if (nextIndex < tokens.Count && tokens[nextIndex].Type == EntityType.LParen)
                {
                    index += 2;
                    if (TryGetFunction(rawContent, out var function))
                        expression.Append(ProcessFunctionCall(parent, function, tokens, ref index));
                    else
                        throw new Exception($"Function {rawContent} not defined");

                    continue;
                }

                expression.Append(ProcessVariable(rawContent, parent));
            }
            else
                expression.Append(currentToken);

            index++;
        }

        newIndex = index;
        
        return expression;
    }

    private Operable? ProcessVariable(string rawContent, IExecutableContainer? parent)
    {
        if (TryGetVariable(rawContent, out var variable)) 
            return variable;
        
        if (TryGetFunction(rawContent, out var functionOperable))
            variable = functionOperable;
        else if (parent is Function parentFunction)
            variable = parentFunction.GetArgOperable(rawContent);
        else
        {
            variable = new Operable();
            _variables.Add(rawContent, variable);
        }

        return variable;
    }

    private FunctionCall ProcessFunctionCall(IExecutableContainer? parent, Function? function, IReadOnlyList<Entity> tokens, ref int index)
    {
        var functionCall = new FunctionCall(function);
        var argTokenBuffer = new List<Entity>();

        void ProcessInternalExpression(ref int localIndex)
        {
            switch (argTokenBuffer.Count)
            {
                case > 1:
                    functionCall.AppendArg(ProcessExpression(function, argTokenBuffer, 0, out _));
                    break;
                case 1:
                {
                    var singleToken = argTokenBuffer[0];
                    switch (singleToken)
                    {
                        case Undefined undefinedToken:
                        {
                            var rawContent = undefinedToken.Content;
                            if (!TryGetVariable(rawContent, out var variable))
                            {
                                if (parent is Function parentFunction)
                                    variable = parentFunction.GetArgOperable(rawContent);
                                else
                                    throw new Exception($"Unexpected argument \"{rawContent}\"");
                            }

                            functionCall.AppendArg(variable);
                            break;
                        }
                        case Operable operableToken:
                            functionCall.AppendArg(operableToken);
                            break;
                    }

                    break;
                }
            }

            argTokenBuffer.Clear();
            localIndex++;
        }

        var bracketCount = 1;
        while (bracketCount > 0)
        {
            var currentToken = tokens[index];

            switch (currentToken.Type)
            {
                case EntityType.Comma:
                    if (bracketCount == 1)
                    {
                        ProcessInternalExpression(ref index);
                        continue;
                    }
                    break;
                case EntityType.LParen:
                    bracketCount++;
                    break;
                case EntityType.RParen:
                    bracketCount--;
                    if (bracketCount == 0)
                    {
                        ProcessInternalExpression(ref index);
                        continue;
                    }
                    break;
            }
            
            argTokenBuffer.Add(currentToken);
            index++;
        }

        return functionCall;
    }

    public void Return()
    {
        _inProgress = false;
    }
}