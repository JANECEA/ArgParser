namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal ArgsClassMetadata ClassMetadata { get; }

    private ProcessedClassMetadata(ArgsClassMetadata classMetadata)
    {
        ClassMetadata = classMetadata;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata)
    {
        return new ProcessedClassMetadata(metadata);
    }
}
