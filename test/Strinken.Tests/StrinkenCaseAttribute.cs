using System;
using NUnit.Framework;

namespace Strinken.Tests
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class StrinkenCaseAttribute : TestCaseAttribute
    {
        public StrinkenCaseAttribute(object arg) 
            : base(arg)
        {
        }

        public StrinkenCaseAttribute(object arg1, object arg2) 
            : base(arg1, arg2)
        {
        }

        public StrinkenCaseAttribute(object arg1, object arg2, object arg3) 
            : base(arg1, arg2, arg3)
        {
        }
    }
}