using System;
using NUnit.Framework;

namespace Strinken.Public.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class StrinkenTestAttribute : TestAttribute
    {
    }
}