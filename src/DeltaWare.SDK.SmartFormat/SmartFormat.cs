using DeltaWare.SDK.SmartFormat.Segments;
using DeltaWare.SDK.SmartFormat.Segments.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWare.SDK.SmartFormat
{
    /// <summary>
    /// Parses Smart Format.
    /// </summary>
    public static class SmartFormat
    {
        /// <summary>
        /// Parses the Smart Format template against the arguments.
        /// </summary>
        /// <param name="smartFormatTemplate">A composite Smart Format string.</param>
        /// <param name="args">The arguments to be formatted.</param>
        /// <returns>A copy of <paramref name="smartFormatTemplate"/> in which any format items are replaced by the string representation contained in <paramref name="args"/>.</returns>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="FormatException"/>
        public static string Parse(string smartFormatTemplate, Dictionary<string, object> args)
        {
            ISmartSegment[] segments = SmartSegmentParser.ParseSegments(smartFormatTemplate);

            StringBuilder formatBuilder = new StringBuilder();

            foreach (ISmartSegment segment in segments)
            {
                object value = null;

                if (segment.HasKey)
                {
                    args.TryGetValue(segment.Key, out value);

                    if (value == null)
                    {
                        if (segment.Nullable)
                        {
                            continue;
                        }

                        throw new KeyNotFoundException($"The non nullable segment {segment.Key} does not have an associated argument.");
                    }
                }

                string text = segment.Parse(value);

                formatBuilder.Append(text);
            }

            return formatBuilder.ToString();
        }

        /// <summary>
        /// Parses the Smart Format template against the definition.
        /// </summary>
        /// <param name="smartFormatTemplate">A composite Smart Format string.</param>
        /// <param name="definition">An object containing the arguments to be formatted.</param>
        /// <returns>A copy of <paramref name="smartFormatTemplate"/> in which any format items are replaced by the string representation contained in <paramref name="definition"/>.</returns>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="FormatException"/>
        public static string Parse(string smartFormatTemplate, object definition)
        {
            return Parse(smartFormatTemplate, definition.GetPublicPropertiesAsDictionary());
        }
    }
}
