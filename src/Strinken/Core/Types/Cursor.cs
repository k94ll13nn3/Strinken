﻿// stylecop.header
using System;
using System.IO;

namespace Strinken.Core.Types
{
    /// <summary>
    /// Cursor used to read a string.
    /// </summary>
    internal class Cursor : IDisposable
    {
        /// <summary>
        /// The reader used to read the string.
        /// </summary>
        private readonly StringReader reader;

        /// <summary>
        /// A value indicating whether the <see cref="Dispose()"/> method has already been called.
        /// </summary>
        private bool disposed;

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

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Moves the cursor.
        /// </summary>
        public void Next()
        {
            this.Value = this.reader.Read();
            this.Position++;
        }

        /// <summary>
        /// Implementation of the dispose method.
        /// </summary>
        /// <param name="disposing">A value indicating whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.reader?.Dispose();
            }

            this.disposed = true;
        }
    }
}