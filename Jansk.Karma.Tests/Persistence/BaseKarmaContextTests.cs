using System;
using System.IO;
using Jansk.Karma.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Jansk.Karma.Tests.Persistence
{
    public class BaseKarmaContextTests : IDisposable
    {
        protected string _databaseFilename;
        protected KarmaContext _context;
        
        protected void InitContext()
        {
            if (string.IsNullOrEmpty(_databaseFilename))
            {
                _databaseFilename = Guid.NewGuid().ToString();
            }
            _context?.Dispose();
            _context = new KarmaContext(_databaseFilename);
            _context.Database.Migrate();
        }

        public virtual void Dispose()
        {
            _context?.Dispose();
            if (File.Exists(_databaseFilename))
            {
                File.Delete(_databaseFilename);
            }
        }
    }
}