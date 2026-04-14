using ArgParser.Attributes;
using ArgParser.Exceptions;
using System.Reflection;

namespace ArgParser;

/// <summary>
/// Base class your declared arguments class should inherit from.
/// </summary>
public abstract class BaseArgs
{
    /// <summary>
    /// The default implementation of the help flag.
    /// </summary>
    [ShortNames('h'), LongNames("help"), TerminatingFlag<HelpCalledException>]
    public virtual bool HelpCalled { get; set; }

    /// <summary>
    /// Where default program arguments will be stored.
    /// </summary>
    public abstract string[] PlainArguments { get; set; }
}

/// <summary>
/// Implements methods for creating <see cref="ArgParser{TArgs}"/>.
/// </summary>
public static class ArgParserFactory
{
    /// <summary>
    /// Creates a new <see cref="ArgParser{TArgs}"/>.
    /// </summary>
    /// <typeparam name="TArgs">Type of the created ArgParser</typeparam>
    /// <exception cref="ParserConfigurationException">Describes errors encountered during the validation of TArgs</exception>
    public static ArgParser<TArgs> FromType<TArgs>()
        where TArgs : BaseArgs, new()
    {
        return new ArgParser<TArgs>();
    }
}

internal class FunctionalAttributes
{
    internal ShortNamesAttribute? ShortNames { get; }
    internal LongNamesAttribute? LongNames { get;}
    internal bool isRequired { get; }
    internal RequiresAttribute? Requires { get;}
    internal ITerminatingFlag? terinatingFlag { get; }

}

internal class InformationalAttributes
{
    internal MetaVarNameAttribute? MetaVarName { get; }
    internal HelpAttribute? Help { get; }
}

internal class PropertyAttributeInfo
{
    internal PropertyInfo Info { get; }    

    internal FunctionalAttributes FunctionalAttributes { get;}

    internal InformationalAttributes InformationalAttributes { get; }

    internal List<IOptionValidator> ValidatorAttributes { get; }

}

/// <summary>
/// Implements parsing of standard program command line arguments into the specified type
/// </summary>
/// <typeparam name="TArgs">Declared argument type</typeparam>
public sealed class ArgParser<TArgs>
    where TArgs : BaseArgs, new()
{
    internal ArgParser() {
        Type ArgType = typeof(TArgs);
        PropertyInfo[] properties = ArgType.GetProperties();

        foreach (PropertyInfo info in properties)
        {
            Console.WriteLine("Property: " + info.Name);

            object[] attributes = info.GetCustomAttributes(false);
            foreach (object attribute in attributes)
            {
                Console.WriteLine("    Attribute: " + attribute.GetType().Name);
            }

            ShortNamesAttribute? shortNames  = info.GetCustomAttribute<ShortNamesAttribute>(false);
            
        }
    }

    private bool CheckCorrectUseOfAttribute()
    {
        return false;
    }

    /// <summary>
    /// Tries parsing the command line arguments according to the structure of
    /// TArgs and attributes defined in it.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <exception cref="CommandLineParsingException">Argument parsing has failed</exception>
    /// <exception cref="HelpCalledException">Program was called with the 'help' terminating flag</exception>
    public TArgs Parse(string[] args)
    {
        return new TArgs();
    }

    /// <summary>
    /// Generates a help message based on the information provided in attributes:
    /// <see cref="HelpAttribute"/>,
    /// <br/>
    /// <see cref="MetaVarNameAttribute"/>, and
    /// <br/>
    /// <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public string GenerateHelpMessage()
    {
        return "";
    }
}
