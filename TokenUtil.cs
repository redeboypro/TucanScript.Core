using System.Globalization;
using System.Text;

namespace TucanScript.Core;

public static class TokenUtil
{
    public const char SpaceChar = ' ';
    public const char QuoteChar = '"';
    public const char SharpChar = '#';

    private static readonly Dictionary<char, EntityType> SingleTokenMap;
    private static readonly Dictionary<string, EntityType> ReservedTokenMap;

    static TokenUtil()
    {
        SingleTokenMap = new Dictionary<char, EntityType>
        {
            { '=', EntityType.Equal },
            { '*', EntityType.Mul },
            { '/', EntityType.Div },
            { '%', EntityType.Percent },
            { '+', EntityType.Plus },
            { '-', EntityType.Minus },
            { '>', EntityType.Greater },
            { '<', EntityType.Less },
            { '(', EntityType.LParen },
            { ')', EntityType.RParen },
            { '{', EntityType.BeginBlock },
            { '}', EntityType.EndBlock },
            { ';', EntityType.Semicolon },
            { ',', EntityType.Comma },
            { '&', EntityType.Ref }
        };
        
        ReservedTokenMap = new Dictionary<string, EntityType>
        {
            { "AND", EntityType.And },
            { "OR", EntityType.Or },
            { "IF", EntityType.If },
            { "WHILE", EntityType.While },
            { "DEF", EntityType.Def },
            { "IMP", EntityType.Imp },
            { "FOR", EntityType.For },
            { "IN", EntityType.In },
            { "BREAK", EntityType.Break },
            { "CONTINUE", EntityType.Continue},
            { "RETURN", EntityType.Return}
        };
    }

    public static bool IsOperator(EntityType type)
    {
        var typeAsInt = (uint) type;
        return typeAsInt is >= 6 and <= 24;
    }
    
    public static bool IsSingleToken(this char source, out EntityType entityType)
    {
        if (SingleTokenMap.TryGetValue(source, out entityType))
            return true;

        entityType = EntityType.None;
        return false;
    }

    public static IReadOnlyList<Entity> Tokenize(String source)
    {
        var tokenStringBuilder = new StringBuilder();
        var rawTokenList = new List<Entity>();

        for (var index = 0; index < source.Length; index++)
        {
            var sym = source[index];
            var tokenStr = tokenStringBuilder.ToString();

            switch (sym)
            {
                case SharpChar:
                {
                    sym = source[++index];
                
                    while (sym != '\n' && sym != '\r')
                        sym = source[++index];
                
                    continue;
                }
                case SpaceChar:
                {
                    if (tokenStr.Length > 0)
                        rawTokenList.Add(ParseToken(tokenStr));

                    tokenStringBuilder.Clear();
                    continue;
                }
            }

            if (sym.IsSingleToken(out var type1))
            {
                if (tokenStr.Length > 0) 
                    rawTokenList.Add(ParseToken(tokenStr));

                tokenStringBuilder.Clear();

                if (index < source.Length - 1)
                {
                    var nextSym = source[index + 1];
                    if (nextSym.IsSingleToken(out var type2) && type2 == EntityType.Equal)
                    {
                        switch (type1)
                        {
                            case EntityType.Equal:
                                rawTokenList.Add(new Entity(EntityType.EqualEqual));
                                break;
                            case EntityType.Less:
                                rawTokenList.Add(new Entity(EntityType.LEqual));
                                break;
                            case EntityType.Greater:
                                rawTokenList.Add(new Entity(EntityType.GEqual));
                                break;
                            case EntityType.Plus:
                                rawTokenList.Add(new Entity(EntityType.PlusEqual));
                                break;
                            case EntityType.Minus:
                                rawTokenList.Add(new Entity(EntityType.MinusEqual));
                                break;
                            case EntityType.Mul:
                                rawTokenList.Add(new Entity(EntityType.MulEqual));
                                break;
                            case EntityType.Div:
                                rawTokenList.Add(new Entity(EntityType.DivEqual));
                                break;
                        }

                        index++;
                        continue;
                    }
                }

                rawTokenList.Add(new Entity(type1));
                continue;
            }

            if (sym == QuoteChar)
            {
                index++;
                while (source[index] != QuoteChar)
                {
                    tokenStringBuilder.Append(source[index]);
                    index++;
                }
                rawTokenList.Add(new Const(tokenStringBuilder.ToString()));
                tokenStringBuilder.Clear();
                continue;
            }

            if (!char.IsControl(sym))
                tokenStringBuilder.Append(sym);
        }

        if (tokenStringBuilder.Length > 0)
            rawTokenList.Add(ParseToken(tokenStringBuilder.ToString()));

        return rawTokenList;
    }

    private static Entity ParseToken(string tokenStr)
    {
        if (Boolean.TryParse(tokenStr, out var booleanValue))
            return new Const(booleanValue);
        
        if (Int64.TryParse(tokenStr, out var intValue))
            return new Const(intValue);

        if (Double.TryParse(tokenStr,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var floatingPointValue))
            return new Const(floatingPointValue);

        return ReservedTokenMap.TryGetValue(tokenStr, out var anotherLiteralValue) ?
            new Entity(anotherLiteralValue) :
            new Undefined(tokenStr);
    }
}