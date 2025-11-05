namespace Sandbox.Validation;

internal record ValidationErrorContext(Line Line, Dictionary<string, string> Data);
