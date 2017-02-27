// stylecop.header
using System;
using System.IO;

namespace Strinken.Engine
{
    /// <summary>
    /// Cursor used to read a string.
    /// </summary>
    internal sealed class Cursor : IDisposable
    {
        /// <summary>
        /// The reader used to read the string.
        /// </summary>
        private readonly StringReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="input">The string to read.</param>
        public Cursor(string input)
        {
            this.reader = new StringReader(input);
            this.Position = -1;
            this.Value = '\0';
        }

        /// <summary>
        /// Gets the current position of the cursor.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the current value of the cursor.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the current value of the cursor as a <see cref="char"/>.
        /// </summary>
        public char CharValue => (char)this.Value;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.reader?.Dispose();
        }

        /// <summary>
        /// Indicates if the cursor has reached the end.
        /// </summary>
        /// <returns>A value indicating whether the cursor as reached the end.</returns>
        public bool HasEnded() => this.Position != -1 && this.Value == -1;

        /// <summary>
        /// Moves the cursor.
        /// </summary>
        public void Next()
        {
            this.Value = this.reader.Read();
            this.Position++;
        }
    }
}