using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ArgParser.Analyzers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArgParser.Analyzers;

[
    SuppressMessage(
        "MicrosoftCodeAnalysisReleaseTracking",
        "RS2008:Enable analyzer release tracking"
    ),
    DiagnosticAnalyzer(LanguageNames.CSharp),
]
public sealed class AttributeUsageAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor InvalidTargetRule = new(
        id: "ARG001",
        title: "Invalid attribute target",
        messageFormat: "Attribute '{0}' cannot be applied to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(InvalidTargetRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType, SymbolKind.Property);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        foreach (AttributeData attribute in context.Symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            foreach (INamedTypeSymbol iface in attribute.AttributeClass.AllInterfaces)
                ProcessInterface(context, attribute, context.Symbol, iface);
        }
    }

    private static void ProcessInterface(
        SymbolAnalysisContext context,
        AttributeData attribute,
        ISymbol symbol,
        INamedTypeSymbol iface
    )
    {
        switch (iface.Name)
        {
            case nameof(IOnParsable):
                if (!IsParsableTarget(symbol))
                    Report(context, attribute, symbol);
                break;

            case nameof(IOnFlag):
                if (!IsFlagProperty(symbol))
                    Report(context, attribute, symbol);
                break;

            case nameof(INotOnFlag):
                if (IsFlagProperty(symbol))
                    Report(context, attribute, symbol);
                break;

            default:
                switch (iface.OriginalDefinition.Name)
                {
                    case nameof(IOnClassType<>):
                        if (!IsOnClassType(symbol, iface.TypeArguments[0]))
                            Report(context, attribute, symbol);
                        break;

                    case nameof(IOnPropertyType<>):
                        if (!IsOnPropertyType(symbol, iface.TypeArguments[0]))
                            Report(context, attribute, symbol);
                        break;
                }
                break;
        }
    }

    private static void Report(
        SymbolAnalysisContext context,
        AttributeData attribute,
        ISymbol symbol
    )
    {
        Location? location = attribute
            .ApplicationSyntaxReference?.GetSyntax(context.CancellationToken)
            .GetLocation();
        if (location is null)
            return;

        Diagnostic diagnostic = Diagnostic.Create(
            InvalidTargetRule,
            location,
            attribute.AttributeClass?.Name ?? nameof(Attribute),
            symbol.ToDisplayString()
        );

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsParsableTarget(ISymbol symbol)
    {
        ITypeSymbol? propertyType = symbol is IPropertySymbol p ? p.Type : null;
        if (propertyType is null)
            return false;

        foreach (INamedTypeSymbol iface in propertyType.AllInterfaces)
            if (
                iface.OriginalDefinition.SpecialType == SpecialType.None
                && iface is { Name: "IParsable", TypeArguments.Length: 1 }
                && SymbolEqualityComparer.Default.Equals(iface.TypeArguments[0], propertyType)
            )
                return true;

        return false;
    }

    private static bool IsFlagProperty(ISymbol symbol) =>
        symbol is IPropertySymbol { Type.SpecialType: SpecialType.System_Boolean };

    private static bool IsOnPropertyType(ISymbol symbol, ITypeSymbol targetType)
    {
        ITypeSymbol? propertyType = symbol is IPropertySymbol p ? p.Type : null;
        if (propertyType is null)
            return false;

        return SymbolEqualityComparer.Default.Equals(propertyType, targetType)
            || propertyType.AllInterfaces.Contains(targetType, SymbolEqualityComparer.Default)
            || InheritsFrom(propertyType, targetType);
    }

    private static bool IsOnClassType(ISymbol symbol, ITypeSymbol targetType) =>
        symbol is INamedTypeSymbol subjectType
        && (
            SymbolEqualityComparer.Default.Equals(subjectType, targetType)
            || subjectType.AllInterfaces.Contains(targetType, SymbolEqualityComparer.Default)
            || InheritsFrom(subjectType, targetType)
        );

    private static bool InheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        ITypeSymbol? current = type.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;

            current = current.BaseType;
        }
        return false;
    }
}
