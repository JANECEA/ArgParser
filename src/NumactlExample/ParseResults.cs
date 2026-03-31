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

    [ShortNames('H'), LongNames("hardware"), Help("Print hardware configuration.")]
    public bool HardwareSwitch { get; set; }

    [ShortNames('s'), LongNames("show"), Help("Show current NUMA policy.")]
    public bool ShowSwitch { get; set; }

    [
        ShortNames('p'),
        LongNames("preferred"),
        MetaVarName("<node>"),
        Help("Prefer memory allocations from given node."),
    ]
    public int? PreferredNode { get; set; }

    [
        ShortNames('i'),
        LongNames("interleave"),
        MetaVarName("<nodes>"),
        Help("Interleave memory allocation across given nodes."),
    ]
    public InputNumberedEntity? Interleave { get; set; }

    [
        ShortNames('m'),
        LongNames("membind"),
        MetaVarName("<nodes>"),
        Help("Allocate memory from given nodes only."),
    ]
    public InputNumberedEntity? Memory { get; set; }

    [
        ShortNames('C'),
        LongNames("physcpubind"),
        MetaVarName("<cpus>"),
        Help("Run on given CPUs only."),
    ]
    public InputNumberedEntity? PhysicalCPU { get; set; }

    public override string[] PlainArguments { get; set; } = [];
}
