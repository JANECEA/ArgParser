using ArgParser.Attributes;

namespace NumactlExample;

internal sealed class InputValidationAttribute : ClassValidatorAttribute<ParseResults>
{
    public override bool Validate(ParseResults args, out string? errorMessage)
    {
        if (args.HardwareSwitch && args.ShowSwitch)
        {
            errorMessage = "Cannot invoke both -H and -s at the same time.";
            return false;
        }
        else if (!args.HardwareSwitch && !args.ShowSwitch)
        {
            errorMessage = ValidateOptionsIfNoSwitch(args);
            if (errorMessage is not null)
            {
                return false;
            }
        }
        else
        {
            errorMessage = ValidateOptionsIfSwitch(args);
            if (errorMessage is not null)
            {
                return false;
            }
        }

        return true;
    }

    private static string? ValidateOptionsIfNoSwitch(ParseResults args)
    {
        if (args.PlainArguments.Length == 0)
        {
            return "Command to execute must be specified.";
        }

        bool[] optionsPresent =
        [
            args.PreferredNode is not null,
            args.Interleave is not null,
            args.Memory is not null,
        ];

        return optionsPresent.Count(p => p is true) > 1
            ? "At most one of options -i, -m, -p can be specified."
            : null;
    }

    private static string? ValidateOptionsIfSwitch(ParseResults args)
    {
        if (args.PlainArguments.Length != 0)
        {
            return "Program does not take additional commands.";
        }

        bool[] optionsPresent =
        [
            args.PreferredNode is not null,
            args.Interleave is not null,
            args.Memory is not null,
            args.PhysicalCPU is not null,
        ];

        return optionsPresent.Any(p => p is true)
            ? "When using switches -H or -s, policies cannot be specified."
            : null;
    }
}
