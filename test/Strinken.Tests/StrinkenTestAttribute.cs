using System;
using NUnit.Framework;

namespace Strinken.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class StrinkenTestAttribute : TestAttribute
    {
    }
}