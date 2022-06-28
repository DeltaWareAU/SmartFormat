using DeltaWare.SDK.SmartFormat.Segments;
using DeltaWare.SDK.SmartFormat.Segments.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWare.SDK.SmartFormat
{
    public static class SmartFormat
    {
        public static string Parse(string format, Dictionary<string, object> args)
        {
            ISmartSegment[] segments = SmartSegmentParser.ParseSegments(format);

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

        public static string Parse(string format, object args)
        {
            return Parse(format, args.GetPublicPropertiesAsDictionary());
        }
    }
}
