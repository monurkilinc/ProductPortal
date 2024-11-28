using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Security
{
    public class TokenBucket
    {
        private readonly int _capacity;
        private readonly TimeSpan _refillTime;
        private int _tokens;
        private DateTime _lastRefill;

        public TokenBucket(int capacity, TimeSpan refillTime)
        {
            _capacity = capacity;
            _refillTime = refillTime;
            _tokens = capacity;
            _lastRefill = DateTime.UtcNow;
        }

        public bool TryTake()
        {
            RefillTokens();
            if (_tokens <= 0) return false;
            _tokens--;
            return true;
        }

        private void RefillTokens()
        {
            var now = DateTime.UtcNow;
            var timePassed = now - _lastRefill;
            if (timePassed >= _refillTime)
            {
                _tokens = _capacity;
                _lastRefill = now;
            }
        }
    }
}
