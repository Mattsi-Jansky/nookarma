using System;
using System.Collections.Generic;
using System.Linq;
using Jansk.Karma.Models;
using Jansk.Karma.Persistence;
using Microsoft.EntityFrameworkCore;
using Noobot.Core.Plugins;

namespace Jansk.Karma.Plugins
{
    public class KarmaRepositoryPlugin : IPlugin
    {
        private KarmaRepository _karmaRepository;
        private ReasonRepository _reasonRepository;
        private KarmaContext _context;
        
        public void Start()
        {
            Console.WriteLine("KarmaPlugin started");
            _context = new KarmaContext();
            _context.Database.Migrate();
            _karmaRepository = new KarmaRepository(_context);
            _reasonRepository = new ReasonRepository(_context);
        }

        public void Stop()
        {
            _context.Dispose();
        }

        public void Update(ChangeRequest request)
        {
            var newKarma = _karmaRepository.KarmaFor(request.Name) + request.Amount;
            _karmaRepository.UpdateOrAdd(request.Name, newKarma);
            if (!String.IsNullOrEmpty(request.Reason)) AddReason(request);
        }

        private void AddReason(ChangeRequest request)
        {
            _reasonRepository.Add(Reason.FromChangeRequest(request));
        }

        public int GetKarma(string name)
        {
            return _karmaRepository.KarmaFor(name);
        }

        public IList<Reason> GetReasons(string name, int? maxResults = null)
        {
            return maxResults.HasValue ? _reasonRepository.Get(name, maxResults.Value).ToList() 
                : _reasonRepository.Get(name).ToList();
        }

        public IEnumerable<Entry> GetTop(int? n)
        {
            return _karmaRepository.GetTop(n);
        }
    }
}