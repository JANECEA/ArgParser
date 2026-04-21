using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace ArgParser.Analyzers;

[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
internal static class Rules
{
    public static readonly DiagnosticDescriptor NotOnParsableRule = new(
        id: "ARG001",
        title: "Attribute requires IParsable property",
        messageFormat: "'{0}' can only be applied to a property whose type implements IParsable<TSelf>. '{1}' does not.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor NotOnFlagRule = new(
        id: "ARG002",
        title: "Attribute requires a boolean property",
        messageFormat: "'{0}' can only be applied to a boolean property. '{1}' is not.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor OnFlagRule = new(
        id: "ARG003",
        title: "Attribute cannot be applied to a boolean property",
        messageFormat: "'{0}' cannot be applied to boolean property '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor WrongClassTypeRule = new(
        id: "ARG004",
        title: "Attribute requires a specific class type",
        messageFormat: "'{0}' can only be applied to a class that is or inherits from '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor WrongPropertyTypeRule = new(
        id: "ARG005",
        title: "Attribute requires a specific property type",
        messageFormat: "'{0}' can only be applied to a property of type that is or inherits from '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
