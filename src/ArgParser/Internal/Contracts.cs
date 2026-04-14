namespace ArgParser.Internal;

internal interface ITerminatingFlag
{
    internal void ThrowException();
}

internal interface IOptionValidator
{
    internal Type ValidatorType { get; }

    internal bool ValidateInternal(object arg, out string? errorMessage);
}

internal interface IClassValidator
{
    internal Type ValidatorType { get; }

    internal bool ValidateInternal(BaseArgs args, out string? errorMessage);
}
