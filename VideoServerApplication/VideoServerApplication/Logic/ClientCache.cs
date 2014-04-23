using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoServerApplication.Logic
{
    public class ClientCache
    {
        private static ClientCache myInstance = new ClientCache();

        private Dictionary<String, String> cache = new Dictionary<string, string>();

        public static ClientCache Instance
        {
            get { return myInstance; }
        }

        public Dictionary<String, String> Cache
        {
            get { return cache; }

        }

        private ClientCache()
        {

        }

        public void AddClientValue(string clientName, string value )
        {
            if(cache.ContainsKey(clientName))
            {
                cache[clientName] = value;
            }
            else
            {
                lock(this)
                {
                    cache.Add(clientName, value);
                }
            }
        }


    }
}