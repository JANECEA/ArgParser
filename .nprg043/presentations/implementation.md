---
class: lead
backgroundColor: #fff
paginate: true
_paginate: false
marp: true
style: |
  section.title {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }

  section.title h1 {
    font-size: 3em;
    text-align: center;
  }

  code {
    color: #1e1e1e;
    background: #d4d4d4;
  }

  pre code {
    font-size: 0.9em;
  }

  /* keywords */
  .token.keyword { color: #569cd6; }

  /* strings */
  .token.string { color: #ce9178; }

  /* comments */
  .token.comment { color: #6a9955; }

  /* functions */
  .token.function { color: #dcdcaa; }
---

<!-- _class: title -->

# Implementace

Tereza Jandová
Adam Janeček

---

# Shrnutí API

- Deklarativní API
- Použití reflexe a atributů

```cs
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

    public override string[] PlainArguments { get; set; } = [];
}
```

---

## Použití

- Validace typu oddělená od parsování

```cs
private static void Main(string[] args)
{
    ArgParser<SimpleArgs> simpleArgsParser = ArgParserFactory.FromType<SimpleArgs>();

    try {
        SimpleArgs simpleArguments = simpleArgsParser.Parse(args);
        Run(simpleArguments);
    }
    catch (CommandLineParsingException ex) {
        Console.WriteLine(ex.Message);
    }
    catch (HelpCalledException helpEx) {
        Console.WriteLine(simpleArgsParser.GenerateHelpMessage());
    }
}
```

---

# Změny v API po review

### Doplnění dokumentace
- V README.md
- Referenční dokumentace

### Přejmenování
- `ShortOptionsAttribute` => `ShortNamesAttribute`
- `LongOptionsAttribute` => `LongNamesAttribute`
- `ValuePlaceholderAttribute` => `MetaVarNameAttribute`

### Možnosti předání hodnot
- `-o value` == `-o=value` == `--option value` == `--option=value`

---

# Navržená změna - pozicionální argumenty

**Pořadí v deklaraci** - není garantované reflekční API
```cs
internal sealed class SimpleArgs : BaseArgs {
    public int? IntOption { get; set; }
    public string? StrOption { get; set; }
}
```

**Pořadí indexací** - naprd
```cs
internal sealed class SimpleArgs : BaseArgs {
    [ArgIndex(0)]
    public int? IntOption { get; set; }
    [ArgIndex(1)]
    public string? StrOption { get; set; }
}
```

---

# Stav implementace

Zpracování typu `Args`
```cs
public static ArgParser<TArgs> FromType<TArgs>()
    where TArgs : BaseArgs, new()
{
    Type ArgType = typeof(TArgs);

    var classMetadata = ArgsClassMetadata.FromType(ArgType);
    MetadataValidator.Validate(classMetadata);

    var processed = ProcessedClassMetadata.FromMetadata(classMetadata);
    return new ArgParser<TArgs>(processed);
}
```

---

## Okrajové případy

Terminating flag po delimiteru
```sh
myprogram -o Output.txt -- --help
```

Delimiter jako hodnota
```sh
myprogram -d -- -- plainArg
```

Prázdná hodnota u argumentu
```sh
myprogram --argument=
```

Terminating flag má přednost
```sh
myprogram --help -o
```
---

# Integrace testů

**Status**: 262 / 355 :)

bye bye