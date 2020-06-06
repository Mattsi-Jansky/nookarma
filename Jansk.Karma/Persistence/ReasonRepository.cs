using System.Collections.Generic;
using System.Linq;
using Jansk.Karma.Models;

namespace Jansk.Karma.Persistence
{
    public class ReasonRepository
    {
        private readonly KarmaContext _context;

        public ReasonRepository(KarmaContext context)
        {
            _context = context;
        }

        public IEnumerable<Reason> Get(string name)
        {
            return _context.Reasons.Where(x => x.Name == name);
        }

        public IEnumerable<Reason> Get(string name, int maxResults)
        {
            return Get(name).Take(maxResults);
        }

        public void Add(Reason reason)
        {
            _context.Reasons.Add(reason);
            _context.SaveChanges();
        }
    }
}