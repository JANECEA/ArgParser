using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArgParser.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
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
        ISymbol symbol = context.Symbol;

        foreach (AttributeData attribute in symbol.GetAttributes())
            ProcessAttribute(context, attribute, symbol);
    }

    private static void ProcessAttribute(
        SymbolAnalysisContext context,
        AttributeData attribute,
        ISymbol symbol
    )
    {
        INamedTypeSymbol? attributeClass = attribute.AttributeClass;
        if (attributeClass == null)
            return;

        if (!attributeClass.AllInterfaces.Any())
            return;

        foreach (INamedTypeSymbol iface in attributeClass.AllInterfaces)
        {
            string ifaceName = iface.Name;

            switch (ifaceName)
            {
                case "IOnParsable":
                    if (!IsParsableTarget(symbol))
                        Report(context, attribute, symbol);
                    break;

                case "IOnFlag":
                    if (!IsFlagProperty(symbol))
                        Report(context, attribute, symbol);
                    break;

                case "INotOnFlag":
                    if (IsFlagProperty(symbol))
                        Report(context, attribute, symbol);
                    break;

                default:
                    switch (iface.OriginalDefinition.Name)
                    {
                        case "IOnClassType":
                        {
                            ITypeSymbol targetType = iface.TypeArguments[0];
                            if (!IsOnClassType(symbol, targetType))
                                Report(context, attribute, symbol);
                            break;
                        }
                        case "IOnPropertyType":
                        {
                            ITypeSymbol targetType = iface.TypeArguments[0];
                            if (!IsOnPropertyType(symbol, targetType))
                                Report(context, attribute, symbol);
                            break;
                        }
                    }
                    break;
            }
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
        if (location == null)
            return;

        Diagnostic diagnostic = Diagnostic.Create(
            InvalidTargetRule,
            location,
            attribute.AttributeClass?.Name ?? "Attribute",
            symbol.ToDisplayString()
        );

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsParsableTarget(ISymbol symbol)
    {
        ITypeSymbol? subject = symbol switch
        {
            INamedTypeSymbol t => t,
            IPropertySymbol p => p.Type,
            _ => null,
        };

        if (subject is null)
            return false;

        // We are looking for: IParsable<TSelf> where TSelf == subject
        foreach (INamedTypeSymbol iface in subject.AllInterfaces)
        {
            if (
                iface.OriginalDefinition.SpecialType == SpecialType.None
                && iface.Name == "IParsable"
                && iface.TypeArguments.Length == 1
                && SymbolEqualityComparer.Default.Equals(iface.TypeArguments[0], subject)
            )
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFlagProperty(ISymbol symbol)
    {
        if (symbol is not IPropertySymbol property)
            return false;

        return property.Type.SpecialType == SpecialType.System_Boolean
            || (
                property.Type is INamedTypeSymbol named
                && named.TypeArguments[0].SpecialType == SpecialType.System_Boolean
            );
    }

    private static bool IsOnPropertyType(ISymbol symbol, ITypeSymbol targetType)
    {
        ITypeSymbol? propertyType = symbol is IPropertySymbol property ? property.Type : null;
        if (propertyType is null)
            return false;

        return SymbolEqualityComparer.Default.Equals(propertyType, targetType)
            || propertyType.AllInterfaces.Contains(targetType, SymbolEqualityComparer.Default)
            || InheritsFrom(propertyType, targetType);
    }

    private static bool IsOnClassType(ISymbol symbol, ITypeSymbol targetType)
    {
        ITypeSymbol? subjectType = symbol as INamedTypeSymbol;
        if (subjectType is null)
            return false;

        return SymbolEqualityComparer.Default.Equals(subjectType, targetType)
            || subjectType.AllInterfaces.Contains(targetType, SymbolEqualityComparer.Default)
            || InheritsFrom(subjectType, targetType);
    }

    private static bool InheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        ITypeSymbol? current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;

            current = current.BaseType;
        }

        return false;
    }
}
