namespace Syncrer.Inputs;

public class InputParams
{
    public InputParams(string[] args)
    {
        var parsedInput = InputParamsUtils.ParseParams(args);
        InputParamsUtils.VerifyParams(parsedInput);
        Params = parsedInput;
    }

    public InputParamsRecord Params { get; }
}