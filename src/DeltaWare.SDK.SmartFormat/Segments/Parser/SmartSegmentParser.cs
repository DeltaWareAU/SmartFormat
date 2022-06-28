using System;
using System.Collections.Generic;

namespace DeltaWare.SDK.SmartFormat.Segments.Parser
{
    internal static class SmartSegmentParser
    {
        private const char StartConcat = '[';
        private const char EndConcat = ']';

        private const char StartVariable = '{';
        private const char EndVariable = '}';

        public static ISmartSegment[] ParseSegments(string value)
        {
            List<ISmartSegment> segments = new List<ISmartSegment>();

            int concatStartIndex = -1;
            bool inConcat = false;

            int variableStartIndex = -1;
            int variableEndIndex = -1;
            bool inVariable = false;
            string variable = null;

            string text = null;

            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];

                if (character == StartConcat)
                {
                    if (EndOfString(i))
                    {
                        throw new FormatException("Concatenated Variable Declared at end of string.");
                    }

                    if (value[i + 1] != StartConcat)
                    {
                        if (inVariable)
                        {
                            throw new FormatException($"Variable not terminated before Concatenated Variable declared at index[{i}].");
                        }

                        if (inConcat)
                        {
                            throw new FormatException($"A Concatenated Variable has been declared before the existing Concatenated Variable has terminated index[{i}]");
                        }

                        inConcat = true;

                        concatStartIndex = i + 1;
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (character == EndConcat)
                {
                    if (EndOfString(i) || value[i + 1] != EndConcat)
                    {
                        if (!inConcat)
                        {
                            throw new FormatException("Concatenated Variable Declared at end of string.");
                        }

                        inConcat = false;

                        var concatEndIndex = i - 1;

                        if (variable == null)
                        {
                            if (inVariable)
                            {
                                throw new FormatException($"Concatenated Variable terminated before inner Variable has been terminated at index[{i}].");
                            }

                            throw new FormatException($"Concatenated Variable declared without inner Variable present at index[{i}].");
                        }

                        var segment = new SmartFormatSegment(variable);

                        variable = null;

                        int length = variableStartIndex - concatStartIndex - 1;

                        if (length > 0)
                        {
                            string concatStart = value.Substring(concatStartIndex, length);

                            segment.SetConcatenationStart(concatStart);
                        }

                        length = concatEndIndex - variableEndIndex - 1;

                        if (length > 0)
                        {
                            string concatEnd = value.Substring(concatEndIndex, length);

                            segment.SetConcatenationEnd(concatEnd);
                        }

                        segments.Add(segment);

                        continue;
                    }

                    i++;
                }
                else if (character == StartVariable)
                {
                    if (EndOfString(i))
                    {
                        throw new FormatException("Variable Declared at end of string.");
                    }

                    if (value[i + 1] != StartVariable)
                    {
                        if (variable != null)
                        {
                            throw new FormatException($"A second Variable was declared inside Concatenated Variable at index[{i}]");
                        }

                        if (inVariable)
                        {
                            throw new FormatException($"A Variable has been declared before the existing Variable has terminated index[{i}]");
                        }

                        inVariable = true;

                        variableStartIndex = i + 1;
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (character == EndVariable)
                {
                    if (EndOfString(i) || value[i + 1] != EndVariable)
                    {
                        if (!inVariable)
                        {
                            throw new FormatException("Variable Declared at end of string.");
                        }

                        inVariable = false;

                        variableEndIndex = i;

                        variable = value.Substring(variableStartIndex, variableEndIndex - variableStartIndex);

                        if (inConcat)
                        {
                            continue;
                        }

                        segments.Add(new SmartFormatSegment(variable));

                        variable = null;

                        continue;
                    }

                    i++;
                }

                if (!inVariable && !inConcat)
                {
                    text += character;
                }
                else if (text != null)
                {
                    segments.Add(new PlainTextSegment(text));

                    text = null;
                }
            }

            if (text != null)
            {
                segments.Add(new PlainTextSegment(text));
            }

            if (inConcat)
            {
                throw new FormatException("Concatenated Variable not terminated before end of string.");
            }

            if (inVariable)
            {
                throw new FormatException("Variable not terminated before end of string.");
            }

            return segments.ToArray();

            bool EndOfString(int index) => index + 1 == value.Length;
        }
    }
}
