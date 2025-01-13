using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Cache
{
    public class PreferenceCache(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;

        public async Task SavePreferenceAsync(Preference preference)
        {
            var json = JsonConvert.SerializeObject(preference);
            await _cache.SetStringAsync(preference.Id.ToString(), json);
        }

        public async Task<Preference?> GetPreferenceAsync(Guid id)
        {
            var json = await _cache.GetStringAsync(id.ToString());
            return json != null ? JsonConvert.DeserializeObject<Preference>(json) : null;
        }
    }
}
