namespace Strinken.Engine
{
    /// <summary>
    /// An indicator that follows an operator and defines a parsing method.
    /// </summary>
    internal class Indicator
    {
        public char Symbol { get; set; }
        public ParsingMethod ParsingMethod { get; set; }
    }
}