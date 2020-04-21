namespace Fraso.Hal.Conversions.Tests
{
    internal class ToWrap
    {
        public string Text { get; set; }
        public int Number { get; set; }

        public Bar Nested { get; set; }
    }

    internal class Bar
    {
        public string Text { get; set; }
        public int Number { get; set; }
    }
}
