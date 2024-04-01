namespace TucanScript.Core;

public enum EntityType
{
    Undefined = -2,
    None = -1,
    
    String = 0,
    Integer = 1,
    FloatingPoint = 2,
    Boolean = 3,
    
    LParen = 4,
    RParen = 5,
    
    Equal = 6,
    EqualEqual = 7,
    GEqual = 8,
    LEqual = 9,
    Greater = 10,
    Less = 11,
    PlusEqual = 12,
    MinusEqual = 13,
    MulEqual = 14,
    DivEqual = 15,
    Plus = 18,
    Minus = 19,
    Percent = 20,
    Mul = 21,
    Div = 22,
    
    And = 23,
    Or = 24,
    
    If = 25,
    While = 26,
    
    Def = 27,
    Imp = 28,
    
    Semicolon = 29,
    Comma = 30,
    
    BeginBlock = 31,
    EndBlock = 32,
    
    Break = 33,
    Return = 34,
    Call = 35,
    Ref = 36,
    
    Array = 37,
    
    For = 38,
    In = 39,
    
    Continue = 40
}