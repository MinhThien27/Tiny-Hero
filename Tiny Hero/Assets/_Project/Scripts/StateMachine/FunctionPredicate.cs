using System;

public class FunctionPredicate : IPredicate
{
    readonly Func<bool> func;
    public FunctionPredicate(Func<bool> func)
    {
        this.func = func;
    }

    public bool Evaluate() => func.Invoke();
}