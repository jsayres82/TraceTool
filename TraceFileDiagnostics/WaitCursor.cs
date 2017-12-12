// © Parata Systems, LLC 2008 
// All rights reserved.
 
using System;
using System.Windows.Forms;

namespace TraceFileReader
{
    /// <summary>
    /// Provides a simple way to set a wait cursor (or other cursor)
    /// around a block of code.
    /// </summary>
    /// <example>
    /// using( WaitCursor wait = new WaitCursor()
    /// {
    ///		// Do something
    /// }
    /// </example>
    public sealed class WaitCursor : IDisposable
    {
        private Cursor oldCursor;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="WaitCursor"/> class.</para>
        /// </summary>
        public WaitCursor()
            : this( Cursors.WaitCursor )
        {
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="WaitCursor"/> class.</para>
        /// </summary>
        /// <param name="cursor">
        /// <para>The <see cref="Cursor"/> to use as the wait <see cref="Cursor"/>.</para>
        /// </param>
        public WaitCursor( Cursor cursor )
        {
            oldCursor = Cursor.Current;
            Cursor.Current = cursor;
        }

        /// <summary>
        /// <para>Releases the unmanaged resources used by the <see cref="WaitCursor"/> and optionally releases the managed resources.</para>
        /// </summary>
        public void Dispose()
        {
            Cursor.Current = oldCursor;
        }
    }
}
