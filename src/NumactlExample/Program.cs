using System.Text;
using ArgParser;
using ArgParser.Exceptions;

namespace NumactlExample;

internal static class Program
{
    public static void Main(string[] args)
    {
        var argsParser = ArgParserFactory.FromType<ParseResults>();
        if (args.Length == 0)
        {
            Console.WriteLine(
                """
                Generated this message:
                usage: numactl [--interleave= | -i <nodes>] [--preferred= | -p <node>]
                               [--physcpubind= | -C <cpus>] [--membind= | -m <nodes>]
                               command args ...
                       numactl [--show | -s]
                       numactl [--hardware | -H]

                <nodes> is a comma delimited list of node numbers or A-B ranges or all.
                <cpus> is a comma delimited list of cpu numbers or A-B ranges or all.

                --interleave, -i   Interleave memory allocation across given nodes.
                --preferred, -p    Prefer memory allocations from given node.
                --membind, -m      Allocate memory from given nodes only.
                --physcpubind, -C  Run on given CPUs only.
                --show, -S         Show current NUMA policy.
                --hardware, -H     Print hardware configuration.
                """
            );
            Console.WriteLine(argsParser.GenerateHelpMessage());
            return;
        }

        try
        {
            Run(argsParser.Parse(args));
        }
        catch (CommandLineParsingException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void Run(ParseResults results)
    {
        if (results.HardwareSwitch)
        {
            Console.WriteLine(
                """
                For -H prints this message:
                available: 2 nodes (0-1)
                node 0 cpus: 0 2 4 6 8 10 12 14 16 18 20 22
                node 0 size: 24189 MB
                node 0 free: 18796 MB
                node 1 cpus: 1 3 5 7 9 11 13 15 17 19 21 23
                node 1 size: 24088 MB
                node 1 free: 16810 MB
                node distances:
                node   0   1
                  0:  10  20
                  1:  20  10
                """
            );
        }

        if (results.ShowSwitch)
        {
            Console.WriteLine(
                """
                For -s prints this message:
                policy: default
                preferred node: current
                physcpubind: 0 1 2 3 4 5 6 7 8
                cpubind: 0 1
                nodebind: 0 1
                membind: 0 1
                """
            );
        }
        else
        {
            var program = ProgramToString(results.PlainArguments);
            if (results.Interleave is not null)
            {
                Console.Write($"Will run {program} with option -i for nodes {results.Interleave}");
            }
            else if (results.Memory is not null)
            {
                Console.Write($"Will run {program} with option -m for nodes {results.Memory}");
            }
            else if (results.PreferredNode is not null)
            {
                Console.Write(
                    $"Will run {program} with option -p for node {results.PreferredNode.Value}"
                );
            }

            if (results.PhysicalCPU is not null)
            {
                Console.Write($" on CPUs {results.PhysicalCPU}");
            }
            Console.WriteLine(".");
        }
    }

    private static string ProgramToString(string[] plainArguments)
    {
        if (plainArguments.Length == 1)
        {
            return plainArguments[0];
        }

        var builder = new StringBuilder();
        builder.Append($"{plainArguments[0]} with arguments ");
        for (int i = 1; i < plainArguments.Length; ++i)
        {
            builder.Append($"{plainArguments[i]}, ");
        }

        return builder.ToString();
    }
}
