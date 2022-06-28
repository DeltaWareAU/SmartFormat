using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeltaWare.SDK.SmartFormat.Tests
{
    public class SmartFormatShould
    {
        private class Arguments
        {
            public string Address { get; set; }

            public string Resource { get; set; }

            public string Endpoint { get; set; }
        }

        [Fact]
        public void ParseNullableArgumentA()
        {
            string formatString = "{firstName?}";

            Dictionary<string, object> args = new Dictionary<string, object>();

            SmartFormat.Parse(formatString, args).ShouldBe(string.Empty);
        }

        [Fact]
        public void ParseArgumentsAsObject()
        {
            string formatString = "{Address}[/{ResourcePath?}][/{Resource}][/{Version?}][/{Endpoint?}][?{Query?}]";

            Arguments args = new Arguments
            {
                Address = "http://localhost",
                Resource = "Students",
                Endpoint = "GetById/52"
            };

            SmartFormat.Parse(formatString, args).ShouldBe("http://localhost/Students/GetById/52");
        }

        [Fact]
        public void ParseArgumentsAsAnonObject()
        {
            string formatString = "{Address}[/{ResourcePath?}][/{Resource}][/{Version?}][/{Endpoint?}][?{Query?}]";

            SmartFormat.Parse(formatString, new
            {
                Address = "http://localhost",
                Resource = "Students",
                Query = "firstName=John&lastName=Smith"
            }).ShouldBe("http://localhost/Students?firstName=John&lastName=Smith");
        }

        [Fact]
        public void ParseNullableArgumentB()
        {
            string formatString = "abcd{firstName?}efgh";

            Dictionary<string, object> args = new Dictionary<string, object>();

            SmartFormat.Parse(formatString, args).ShouldBe("abcdefgh");
        }

        [Fact]
        public void ParseConcatenatedNullableArgumentA()
        {
            string formatString = "[FirstName: {firstName?}]";

            Dictionary<string, object> args = new Dictionary<string, object>();

            SmartFormat.Parse(formatString, args).ShouldBe(string.Empty);
        }

        [Fact]
        public void ParseConcatenatedNullableArgumentB()
        {
            string formatString = "abcd[FirstName: {firstName?}]efgh";

            Dictionary<string, object> args = new Dictionary<string, object>();

            SmartFormat.Parse(formatString, args).ShouldBe("abcdefgh");
        }

        [Fact]
        public void ParseUriA()
        {
            string formatString = "{address}[/{resource_path?}][/{resource}][/{version?}][/{endpoint?}][?{query?}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"address", "http://localhost"},
                {"resource", "Students"}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("http://localhost/Students");
        }

        [Fact]
        public void ParseUriB()
        {
            string formatString = "{address}[/{resource_path?}][/{resource}][/{version?}][/{endpoint?}][?{query?}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"address", "http://localhost"},
                {"resource", "Students"},
                {"endpoint", "GetById/52"}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("http://localhost/Students/GetById/52");
        }

        [Fact]
        public void ParseUriC()
        {
            string formatString = "{address}[/{resource_path?}][/{resource}][/{version?}][/{endpoint?}][?{query?}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"address", "http://localhost"},
                {"resource", "Students"},
                {"endpoint", "Paged"},
                {"query", "firstName=John&lastName=Smith"}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("http://localhost/Students/Paged?firstName=John&lastName=Smith");
        }

        [Fact]
        public void ParseDateA()
        {
            string formatString = "Convert the date from {date} to {date:d}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"date", new DateTime(2012, 01, 01)}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("Convert the date from 1/01/2012 12:00:00 AM to 1/01/2012");
        }

        [Fact]
        public void ParseDoubleCurlyBracket()
        {
            string formatString = "Convert the date from {{date}} to {date:d}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"date", new DateTime(2012, 01, 01)}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("Convert the date from {date} to 1/01/2012");
        }

        [Fact]
        public void ParseDoubleSquareBracket()
        {
            string formatString = "Convert the date from [[date]] to {date:d}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"date", new DateTime(2012, 01, 01)}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("Convert the date from [date] to 1/01/2012");
        }

        [Fact]
        public void ParseBothDoubleBracket()
        {
            string formatString = "Convert the date from [[{{date}}]] to {date:d}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"date", new DateTime(2012, 01, 01)}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("Convert the date from [{date}] to 1/01/2012");
        }

        [Fact]
        public void ParsePlainText()
        {
            string formatString = "This is some plain text, kinda defeats the point of formatting though?";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"date", new DateTime(2012, 01, 01)}
            };

            SmartFormat.Parse(formatString, args).ShouldBe("This is some plain text, kinda defeats the point of formatting though?");
        }

        [Fact]
        public void ThrowKeyNotFoundException()
        {
            string formatString = "Convert the date from {date} to {date:d}";

            Dictionary<string, object> args = new Dictionary<string, object>();

            Should.Throw<KeyNotFoundException>(() => SmartFormat.Parse(formatString, args).ShouldBe("Convert the date from 1/01/2012 12:00:00 AM to 1/01/2012"));
        }

        [Fact]
        public void ThrowFormatExceptionForNonTerminatedVariableA()
        {
            string formatString = "{address[/{resource_path?}][/{resource}][/{version?}][/{endpoint?}][?{query?}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"address", "http://localhost"},
                {"resource", "Students"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionForNonTerminatedVariableB()
        {
            string formatString = "{address{resource_path?}][/{resource}][/{version?}][/{endpoint?}][?{query?}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"address", "http://localhost"},
                {"resource", "Students"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionInvalidConcat()
        {
            string formatString = "{varA}[/as]{varB}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionVariableAtEndOfString()
        {
            string formatString = "{varA}[/as]{varB}{";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionVariableNotTerminatedAtEndOfString()
        {
            string formatString = "{varA}[/as]{varB}{varC";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionConcatAtEndOfString()
        {
            string formatString = "{varA}[/as]{varB}[";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionConcatNotTerminatedAtEndOfString()
        {
            string formatString = "{varA}[/as]{varB}[concat";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionConcatTerminatedBeforeVariable()
        {
            string formatString = "[/{myVar]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionConcatDeclaredWithoutVariable()
        {
            string formatString = "[/myVar]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionVariableContainsMultipleColons()
        {
            string formatString = "{myBadVar:d:c}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionSecondVariableDeclaredInConcatenatedVariable()
        {
            string formatString = "[{myFirstVar}{mySecondVar}]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionVariableDeclaredBeforeVariableTerminated()
        {
            string formatString = "{myFirstVar{mySecondVar}";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }

        [Fact]
        public void ThrowFormatExceptionConcatenatedVariableDeclaredBeforeConcatenatedVariableTerminated()
        {
            string formatString = "[myFirstVar[mySecondVar]";

            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"varA", "AAA"},
                {"varB", "BBB"}
            };

            Should.Throw<FormatException>(() => SmartFormat.Parse(formatString, args));
        }
    }
}
