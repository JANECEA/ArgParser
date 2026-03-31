using ArgParser;
using ArgParser.Attributes;

namespace NumactlExample;

[
    InputValidation,
    AllowPlainArguments(true),
    ExampleUsage(
        """
            usage: numactl [--interleave= | -i <nodes>] [--preferred= | -p <node>]
                           [--physcpubind= | -C <cpus>] [--membind= | -m <nodes>]
                           command args ...
                   numactl [--show | -s]
                   numactl [--hardware | -H]

            <nodes> is a comma delimited list of node numbers or A-B ranges or all.
            <cpus> is a comma delimited list of cpu numbers or A-B ranges or all.
            """
    ),
]
internal sealed class ParseResults : BaseArgs
{
    public override bool HelpCalled
    {
        get => base.HelpCalled;
        set => base.HelpCalled = value;
    }

    [ShortOptions('H'), LongOptions("hardware"), Help("Print hardware configuration.")]
    public bool HardwareSwitch { get; set; }

    [ShortOptions('s'), LongOptions("show"), Help("Show current NUMA policy.")]
    public bool ShowSwitch { get; set; }

    [
        ShortOptions('p'),
        LongOptions("preferred"),
        ValuePlaceholder("<node>"),
        Help("Prefer memory allocations from given node."),
    ]
    public int? PreferredNode { get; set; }

    [
        ShortOptions('i'),
        LongOptions("interleave"),
        ValuePlaceholder("<nodes>"),
        Help("Interleave memory allocation across given nodes."),
    ]
    public InputNumberedEntity? Interleave { get; set; }

    [
        ShortOptions('m'),
        LongOptions("membind"),
        ValuePlaceholder("<nodes>"),
        Help("Allocate memory from given nodes only."),
    ]
    public InputNumberedEntity? Memory { get; set; }

    [
        ShortOptions('C'),
        LongOptions("physcpubind"),
        ValuePlaceholder("<cpus>"),
        Help("Run on given CPUs only."),
    ]
    public InputNumberedEntity? PhysicalCPU { get; set; }

    public override string[] PlainArguments { get; set; } = [];
}
