namespace Syncrer.Inputs;

public sealed class InputParamsException : Exception
{
    private InputParamsException(string message, int exitCode) : base(message)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }

    public static InputParamsException HelpRequested()
    {
        return new InputParamsException(string.Empty, 0);
    }

    public static InputParamsException Invalid(IEnumerable<string> errors)
    {
        return new InputParamsException(string.Join(Environment.NewLine, errors), 1);
    }
}