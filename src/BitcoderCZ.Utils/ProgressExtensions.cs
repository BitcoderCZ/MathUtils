using System;

namespace BitcoderCZ.Utils;

public static class ProgressExtensions
{
    extension(IProgress<ProgressReport> progress)
    {
        /// <summary>
        /// Reports that the task or process is 100% complete.
        /// </summary>
        /// <param name="statusMessage">An optional message describing the final status.</param>
        public void Complete(string? statusMessage = "Done")
            => progress.Report(new ProgressReport(1d, statusMessage));

        /// <summary>
        /// Creates a sub-progress reporter that maps a normalized progress range (0.0 to 1.0) onto a specific sub-range [<paramref name="from"/>, <paramref name="to"/>] relative to the parent progress reporter.
        /// </summary>
        /// <param name="from">The starting fraction of the progress range (inclusive, between 0.0 and 1.0).</param>
        /// <param name="to">The ending fraction of the progress range (inclusive, between 0.0 and 1.0).</param>
        /// <returns>
        /// An <see cref="IProgress{ProgressReport}"/> instance that intercepts inner progress updates and scales them into the specified outer range.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="from"/> or <paramref name="to"/> is negative.</description></item>
        /// <item><description><paramref name="from"/> is greater than <paramref name="to"/>.</description></item>
        /// <item><description><paramref name="to"/> is greater than 1.0.</description></item>
        /// </list>
        /// </exception>
        public IProgress<ProgressReport> WrapRange(double from, double to)
        {
            ThrowHelper.ThrowIfNegative(from);
            ThrowHelper.ThrowIfNegative(to);
            ThrowHelper.ThrowIfGreaterThan(from, to);
            ThrowHelper.ThrowIfGreaterThan(to, 1.0);

            var range = to - from;

            return new Progress<ProgressReport>(report =>
            {
                progress.Report(new ProgressReport((report.PercentComplete * range) + from, report.StatusMessage));
            });
        }
    }
}
