namespace ArgParser.Analyzers.Abstractions;

/// <summary>
/// Flag interface used by the roslyn analyzer
/// </summary>
public interface IOnParsable;

/// <summary>
/// Flag interface used by the roslyn analyzer
/// </summary>
public interface IOnPropertyType<in T>;

/// <summary>
/// Flag interface used by the roslyn analyzer
/// </summary>
public interface IOnClassType<in T>;

/// <summary>
/// Flag interface used by the roslyn analyzer
/// </summary>
public interface IOnFlag;

/// <summary>
/// Flag interface used by the roslyn analyzer
/// </summary>
public interface INotOnFlag;
