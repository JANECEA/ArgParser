using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParserTests
{
    public class DependencyTests
    {
        [ExampleUsage("program [options]")]
        class DependentOptions : BaseArgs
        {
            /* Dependencies:
             * A->B->C
             * D->B
             */

            [ShortNames('c'),LongNames("C")]
            public string? OptionC {  get; set; }

            [ShortNames('b'), LongNames("B"), Requires(nameof(OptionC))]
            public int? OptionB { get; set; }

            [ShortNames('a'), LongNames("A"), Requires(nameof(OptionB))]
            public int? OptionA { get; set; }

            [ShortNames('d'), LongNames("D"), Requires(nameof(OptionB))]
            public bool OptionD { get; set; }

            [ShortNames('e'),LongNames("E")]
            public bool OptionE { get; set; }


            public override string[] PlainArguments { get; set; } = [];
        }

        public static IEnumerable<object[]> ValidDependency =>
            [
                [new[] {"-c","abc"}],
                [new[] {"-c","abc","-b","8"}],
                [new[] { "-c", "abc", "-b", "8", "-a", "7", "-d" }],
                [new[] { "-c", "abc", "-b", "8","-e", "-a", "7", "-d" }]

            ];

        [Theory]
        [MemberData(nameof(ValidDependency))]

        public void DependencyFullfiled(string[] args)
        {
            var parser = ArgParserFactory.FromType<DependentOptions>();

            var result = parser.Parse(args);
        }


        [Fact]
        public void ShuffledDependencies()
        {
            string[] args = { "-a", "7", "-b", "8", "-d", "-c", "shuffled" };
            var parser = ArgParserFactory.FromType<DependentOptions>();

            var result = parser.Parse(args);

        }
        
        public static IEnumerable<object[]> InvalidDependency =>
            [
                [new[] { "-e","-b","7"}],
                [new[] { "-b", "7", "-a", "7"}],
                [new[] { "-c","value","-a","7"}],
                [new[] {"-c","value","-a","7","-d"}]
            ];

        [Theory]
        [MemberData(nameof(InvalidDependency))]
        public void DependencyNotFullfiled(string[] args)
        {
            var parser = ArgParserFactory.FromType<DependentOptions>();

            Assert.Throws<CommandLineParsingException>(()=>parser.Parse(args));

        }

    }
}
