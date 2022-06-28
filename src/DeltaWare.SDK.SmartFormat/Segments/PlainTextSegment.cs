namespace DeltaWare.SDK.SmartFormat.Segments
{
    internal class PlainTextSegment : ISmartSegment
    {
        private readonly string _value;

        public bool HasKey => false;
        public bool Nullable => true;
        public string Key => null;

        public PlainTextSegment(string value)
        {
            _value = value;
        }

        public string Parse(object value) => _value;
    }
}
