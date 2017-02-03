using System;
using Xunit;

namespace Strinken.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class StrinkenTestAttribute : FactAttribute
    {
    }
}