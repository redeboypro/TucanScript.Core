namespace TucanScript.Core;

public class Expression : Operable, IExecutable
{
    private readonly List<Entity> _tokens;

    public Expression()
    {
        _tokens = new List<Entity>();
    }

    public void Append(Operable? token) => _tokens.Add(new OperableClone(token));
    
    public void Append(Entity token) => _tokens.Add(token);

    public void Reset()
    {
        foreach (var token in _tokens)
            if (token is IResetable operable)
                operable.Reset();
    }

    public void Execute()
    {
        Reset();

        var tokenStack = new Stack<Entity>();
        var result = new Queue<Entity>();

        foreach (var token in _tokens)
        {
            switch (token.Type)
            {
                case EntityType.String
                    or EntityType.Integer
                    or EntityType.FloatingPoint
                    or EntityType.Boolean
                    or EntityType.Array:
                    result.Enqueue(token);
                    break;
                case EntityType.Call:
                {
                    var callToken = (FunctionCall) token;
                    var functionResult = callToken.Execute();
                    result.Enqueue(functionResult);
                    break;
                }
                default:
                {
                    if (TokenUtil.IsOperator(token.Type))
                    {
                        while (tokenStack.Count > 0 &&
                               TokenUtil.IsOperator(tokenStack.Peek().Type) &&
                               Precedence(token.Type) <= Precedence(tokenStack.Peek().Type))
                            result.Enqueue(tokenStack.Pop());

                        tokenStack.Push(token);
                    }
                    else
                        switch (token.Type)
                        {
                            case EntityType.LParen:
                                tokenStack.Push(token);
                                break;
                            case EntityType.RParen:
                            {
                                while (tokenStack.Count > 0 && tokenStack.Peek().Type != EntityType.LParen)
                                    result.Enqueue(tokenStack.Pop());

                                if (tokenStack.Count == 0)
                                    throw new ArgumentException("Mismatched parentheses");

                                tokenStack.Pop();
                                break;
                            }
                        }

                    break;
                }
            }
        }

        while (tokenStack.Count > 0)
        {
            if (tokenStack.Peek().Type is EntityType.LParen or EntityType.RParen)
                throw new ArgumentException("Mismatched parentheses");
                
            result.Enqueue(tokenStack.Pop());
        }

        var valueStack = new Stack<Entity>();
        while (result.Count > 0)
        {
            var nextToken = result.Dequeue();

            if (nextToken.Type is EntityType.String
                or EntityType.Integer
                or EntityType.FloatingPoint
                or EntityType.Boolean
                or EntityType.Array)
                valueStack.Push(nextToken);

            else if (TokenUtil.IsOperator(nextToken.Type))
            {
                var b = (Operable) valueStack.Pop();
                var a = (Operable) valueStack.Pop();
                
                var op = nextToken.Type;
                
                switch (op)
                {
                    case EntityType.Plus:
                        a.Add(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Minus:
                        a.Sub(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Mul:
                        a.Mul(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Percent:
                        a.Rem(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Div:
                        a.Div(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.PlusEqual:
                        a.Add(b);
                        ((OperableClone)a).Apply();
                        valueStack.Push(a);
                        break;
                    case EntityType.MinusEqual:
                        a.Sub(b);
                        ((OperableClone)a).Apply();
                        valueStack.Push(a);
                        break;
                    case EntityType.MulEqual:
                        a.Mul(b);
                        ((OperableClone)a).Apply();
                        valueStack.Push(a);
                        break;
                    case EntityType.DivEqual:
                        a.Div(b);
                        ((OperableClone)a).Apply();
                        valueStack.Push(a);
                        break;
                    case EntityType.Equal:
                        a.Set(b);
                        ((OperableClone)a).Apply();
                        valueStack.Push(a);
                        break;
                    case EntityType.EqualEqual:
                        a.Equal(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Greater:
                        a.Greater(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Less:
                        a.Less(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.GEqual:
                        a.GEqual(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.LEqual:
                        a.LEqual(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.And:
                        a.And(b);
                        valueStack.Push(a);
                        break;
                    case EntityType.Or:
                        a.Or(b);
                        valueStack.Push(a);
                        break;
                }
            }
        }

        if (valueStack.Count != 1)
            throw new ArgumentException("Invalid expression");
 
        Set((Operable) valueStack.Pop());
    }
    
    private static int Precedence(EntityType token)
    {
        switch (token)
        {
            case EntityType.And: 
            case EntityType.Or:
                return 1;
            case EntityType.EqualEqual:
            case EntityType.GEqual:
            case EntityType.LEqual:
            case EntityType.Greater:
            case EntityType.Less:
                return 2;
            case EntityType.Plus:
            case EntityType.Minus:
                return 3;
            case EntityType.Mul:
            case EntityType.Div:
            case EntityType.Percent:
                return 4;
            default:
                return 0;
        }
    }
}