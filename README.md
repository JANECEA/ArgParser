# ArgParser
ArgParser is a declarative CLI argument parsing library for .NET.

## Key features
- Parse custom types
- Support for plain arguments
- Define multiple short and long option names
- Define custom validators for an individual option or the whole argument class
- Mark options as required
- Define option dependencies
- Mark flags as terminating
- Automatically generate the help message using additional informational attributes
- Compile time validation of attribute usage using Roslyn 

## Not supported

- Positional arguments
- Typed plain arguments 

## Build instructions
```bash
# Clone repository
git clone https://gitlab.mff.cuni.cz/teaching/nprg043/2026-summer/task-1/t21-api-design.git

# Install in your project
cd YourProject/
dotnet add reference <path to t21-api-design>/src/ArgParser/ArgParser.csproj
dotnet build
```

## Roslyn analyzer

To include Roslyn validation add this to .csproj
```xml
<ItemGroup>
<ProjectReference
  Include="<path to t21-api-design>/src/analyzers/ArgParser.Analyzers/ArgParser.Analyzers.csproj"
  OutputItemType="Analyzer"
  ReferenceOutputAssembly="false"
/>
</ItemGroup>
```

## Simple usage

The following snippet of code shows declaration of simple class inheriting from BaseArgs. Two options that are expecting some value and one flag are defined. 

```cs
using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

[ExampleUsage("myProgram [options]")]
internal sealed class SimpleArgs : BaseArgs
{
    [
        ShortNames('i'),
        LongNames("int"),
        Help("Example of int option."),
        MetaVarName("INT_VALUE"),
    ]
    public int? IntOption { get; set; }

    [
        ShortNames('s'),
        LongNames("string"),
        Help("Example of string option."),
        MetaVarName("STR_VALUE"),
    ]
    public string? StringOption { get; set; }

    [
        ShortNames('f'),
        LongNames("flag"),
        Help("Example of flag."),
    ]
    public bool Flag { get; set; }

    public override string[] PlainArguments { get; set; } = [];
}
```
During the creation of `ArgParser<SimpleArgs>`, `SimpleArgs` class is validated, including the usage of attributes.

Now SimpleArgs class can be used in the program. 
If creating of ArgParser and the parsing of arguments was successful, the values can be accessed directly from the created SimpleArgs object by their defined property names.

```cs
internal class SimpleExampleProgram
{
    private static void Main(string[] args)
    {
        ArgParser<SimpleArgs> simpleArgsParser = 
            ArgParserFactory.FromType<SimpleArgs>();
        try
        {
            SimpleArgs simpleArguments = simpleArgsParser.Parse(args);
            Run(simpleArguments);
        }
        catch (CommandLineParsingException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (HelpCalledException helpEx)
        {
            Console.WriteLine(simpleArgsParser.GenerateHelpMessage());
        }
    }

    private static void Run(SimpleArgs args)
    {
        if (args.Flag)
        {
            // Do desired functionality
        }

        if (args.IntOption is int intVal)
        {
            // Do desired functionality with the given value
        }

        if (args.StringOption is string strVal)
        {
            // Do desired functionality with the given value
        }

        if (args.PlainArguments.Length > 0)
        {
            // Do desired functionality with given plain arguments
        }
    }
}

```
#### Equivalent ways to pass a value:
```cs
-o value -o=value --option value --option=value
```
#### Example of calling the program:
```sh
myapp.exe -i 10 --string="Hello World" -f plainArgument
```
Based on the class `SimpleArgs`, the following help message would be generated:
```sh
> myapp.exe --help
Usage: myProgram [options]

Options:
    -i INT_VALUE, --int=INT_VALUE
            Example of int option.

    -s STR_VALUE, --string=STR_VALUE
            Example of string option.

    -f, --flag
            Example of flag.
```


## Advanced usage

Now we will look into advanced usage of the library. 

First we will show how the user can define his own classes and attributes that will next be used in AdvancedArgs.

### Custom option types

Any type that implements the IParsable\<T\> interface is supported for the option values (for example Enum or custom type can be defined).

```cs
[EnumCasePolicy(EnumCase.PreserveCase)]
internal enum MyEnum
{
    First,
    Second,
    Third,
}

internal class MyClass : IParsable<MyClass>
{
    public static MyClass Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out MyClass result)) 
            return result; 
        else
            throw new ArgumentException("arg is null");
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s, 
        IFormatProvider? provider, 
        [MaybeNullWhen(false)] out MyClass result
    )
    {
        if (s is not null)
        {
            result = new MyClass();
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }
}
```

### Custom class validators

Custom validators for the whole class can be defined. Here we created example of implementing mutual exclusivity.
```cs
internal sealed class MutuallyExclusiveEnumEmailAttribute : ClassValidatorAttribute<AdvancedArgs>
{
    public override bool Validate(AdvancedArgs args, out string? errorMessage)
    {
        if (args.Email is not null && args.Enum is not null)
        {
            errorMessage = "Mutually exclusive attributes Enum and Email were given.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}
```

### Custom option validators

Define custom validator for options. Here we define an example validator that checks if string contains a given substring.

```cs

public sealed class MustContainAttribute : OptionValidatorAttribute<string>
{
    private readonly string _required;

    public MustContainAttribute(string required)
    {
        _required = required;
    }

    public override bool Validate(string arg, out string? errorMessage)
    {
        if (!arg.Contains(_required))
        {
            errorMessage = $"The argument {arg} must contain {_required}";
            return false;
        }
        errorMessage = null;
        return true;
    }
}
```

### Terminating flags

Flags can be marked as terminating using the TerminatingFlag attribute. 
This attribute accepts a type parameter, which must derive from the base Exception class 
and must provide a parameterless constructor.
```cs
internal class FlagCalledException : Exception {}

...
    [
        ShortNames('f'),
        TerminatingFlag<FlagCalledException>
    ]
    public bool Flag {get; set;}

```
If this flag is present in the command-line arguments the specified exception is thrown in order to skip parsing and validation.

```cs
...
    try
    {
        Args Arguments = ArgsParser.Parse(args);
    }
    catch (FlagCalledException flagEx)
    {
        // Handle terminating flag
    }

```


### AdvancedArgs declaration

In this class advanced usage is shown using advanced attributes and the showcased examples.
Default values for options can be defined.
These default values will not be overridden during parsing in case the option is not present.
```cs
using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

[
    ExampleUsage("myProgram -c <COUNT> [options]"),
    MutuallyExclusiveEnumEmail,
    AllowPlainArguments(true),
]
internal sealed class AdvancedArgs : BaseArgs
{
    [
        ShortNames('c', 'x'),
        LongNames("count", "ct"),
        Range<int>(0, int.MaxValue),
        Required,
        MetaVarName("COUNT"),
        Help("Help for count option.")
    ]
    public int? Count { get; set; }

    [
        LongNames("email"),
        MustContain("@")
    ]
    public string? Email { get; set; }

    [
        ShortNames('e')
    ]
    public MyEnum Enum { get; set; } = MyEnum.Second;

    [
        ShortNames('f'),
        LongNames("flag"),
        TerminatingFlag<FlagCalledException>,
    ]
    public bool Flag { set; get; }

    [
        ShortNames('l')
        LongNames("class"),
        Requires(nameof(Email)),
        MetaVarName("CLASS"),
    ]
    public MyClass Class { get; set; } = new();

    public override string[] PlainArguments { get; set; } = [];
}
```

Lastly, the use of created AdvancedArgs.
```cs
internal class AdvancedExample
{
    internal static void Main(string[] args)
    {        
        ArgParser<AdvancedArgs> advancedArgsParser = 
            ArgParserFactory.FromType<AdvancedArgs>();
        try
        {
            AdvancedArgs AdvArguments = advancedArgsParser.Parse(args);
        }
        catch (CommandLineParsingException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (FlagCalledException flagEx)
        {
            // Handle terminating flag
        }
        catch (HelpCalledException helpEx)
        {
            Console.WriteLine(advancedArgsParser.GenerateHelpMessage());
        }
    }

    private static void Run(AdvancedArgs args)
    {
        if (args.Enum is MyEnum.Second)
        {
            // Do desired functionality
        }
        // access all properties by their defined names
    }
}
```

#### Example of calling the program:
```sh
myapp.exe -c=10 plainArgument1 --email=example@abc.de -l myclass -- PlainArgument2 -PlainArgument3
```
