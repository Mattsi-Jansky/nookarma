using System;
using System.IO;
using Jansk.Karma.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Jansk.Karma.Tests.Persistence
{
    public class BaseKarmaContextTests : IDisposable
    {
        private string _databaseFilename;
        protected KarmaContext Context;
        
        protected void InitContext()
        {
            if (string.IsNullOrEmpty(_databaseFilename))
            {
                _databaseFilename = Guid.NewGuid().ToString();
            }
            Context?.Dispose();
            Context = new KarmaContext(_databaseFilename);
            Context.Database.Migrate();
        }

        public virtual void Dispose()
        {
            Context?.Dispose();
            if (File.Exists(_databaseFilename))
            {
                File.Delete(_databaseFilename);
            }
        }
    }
}