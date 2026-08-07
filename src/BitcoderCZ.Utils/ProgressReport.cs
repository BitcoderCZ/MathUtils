namespace BitcoderCZ.Utils;

/// <summary>
/// Represents a progress report.
/// </summary>
/// <param name="PercentComplete">How much of the task has been completed, between 0 and 1.</param>
/// <param name="StatusMessage">Optionally, what is currently being done.</param>
public readonly record struct ProgressReport(double PercentComplete, string? StatusMessage);
