using System;
using System.ComponentModel;

namespace BitcoinRhExplorer.Server
{
    public class DataContextConfiguration : IComponent
    {
        public string ContextName { get; set; }
        public string SchemaName { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose();
            }
        }

        public ISite Site { get; set; }
        public event EventHandler Disposed;
    }
}
