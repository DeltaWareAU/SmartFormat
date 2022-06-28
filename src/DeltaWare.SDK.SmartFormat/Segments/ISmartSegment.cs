namespace DeltaWare.SDK.SmartFormat.Segments
{
    internal interface ISmartSegment
    {
        bool HasKey { get; }

        bool Nullable { get; }

        string Key { get; }

        string Parse(object value);
    }
}
