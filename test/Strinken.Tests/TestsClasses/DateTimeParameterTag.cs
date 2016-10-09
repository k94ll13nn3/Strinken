using System;
using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class DateTimeParameterTag : IParameterTag
    {
        public string Description => "DateTime";
        public string Name => "DateTime";

        public string Resolve() => DateTime.Now.ToString("u");
    }
}