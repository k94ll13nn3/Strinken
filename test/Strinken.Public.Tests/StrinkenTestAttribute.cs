using System;
using Xunit;

namespace Strinken.Public.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class StrinkenTestAttribute : FactAttribute
    {
    }
}