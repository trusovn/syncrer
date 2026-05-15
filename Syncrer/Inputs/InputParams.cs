namespace Syncrer.Inputs;

public class InputParams
{
    public InputParams(string[] args)
    {
        InputParamsRecord parsedInput = InputParamsConfiguration.ParseParams(args);
        InputParamsVerifier.VerifyParams(parsedInput);
        Params = parsedInput;
    }

    public InputParamsRecord Params { get; }
}