using System;

namespace DeltaWare.SDK.SmartFormat.Segments
{
    internal class SmartFormatSegment : ISmartSegment
    {
        private readonly string _formatString;

        private string _concatenationStart = string.Empty;
        private string _concatenationEnd = string.Empty;
        private bool _concatenated;

        public bool Nullable { get; }
        public string Key { get; }
        public bool HasKey => true;

        public SmartFormatSegment(string value)
        {
            string[] segments = value.Split(':');

            if (segments.Length > 2)
            {
                throw new FormatException($"Variable contains multiple value formatters, only one formatter may be present in a variable declaration. {value}");
            }

            string key = segments[0];

            if (TryGetNullableKey(key, out string nullableKey))
            {
                Nullable = true;

                Key = nullableKey;
            }
            else
            {
                Key = key;
            }

            if (segments.Length == 1)
            {
                return;
            }

            string format = segments[1];

            if (TryGetNullableKey(format, out string nullableFormat))
            {
                Nullable = true;

                _formatString = nullableFormat;
            }
            else
            {
                _formatString = format;
            }
        }

        public void SetConcatenationStart(string value)
        {
            _concatenationStart = value;
            _concatenated = true;
        }

        public void SetConcatenationEnd(string value)
        {
            _concatenationEnd = value;
            _concatenated = true;
        }

        public string Parse(object value)
        {
            string format;

            if (_formatString == null)
            {
                format = value.ToString();
            }
            else
            {
                format = string.Format($"{{0:{_formatString}}}", value);
            }

            if (_concatenated)
            {
                return _concatenationStart + format + _concatenationEnd;
            }

            return format;
        }

        private static bool TryGetNullableKey(string key, out string nullableKey)
        {
            nullableKey = null;

            if (!key.EndsWith('?'))
            {
                return false;
            }

            nullableKey = key[..^1];

            return true;

        }
    }
}
