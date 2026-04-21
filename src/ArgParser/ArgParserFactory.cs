using ArgParser.Exceptions;
using ArgParser.Internal.Metadata;

namespace ArgParser;

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
        Type ArgType = typeof(TArgs);

        ArgsClassMetadata classMetadata = ArgsClassMetadata.FromType(ArgType);
        MetadataValidator.Validate(classMetadata);

        ProcessedClassMetadata processed = ProcessedClassMetadata.FromMetadata(classMetadata);
        return new ArgParser<TArgs>(processed, new Lazy<string>(string.Empty));
    }
}
