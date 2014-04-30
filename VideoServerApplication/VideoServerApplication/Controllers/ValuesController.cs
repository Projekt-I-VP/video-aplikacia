using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VideoServerApplication.Logic;

namespace VideoServerApplication.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "it works :D"};
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        public List<KeyValuePair<string, string>> Get(string clientName, string time)
        {
            ClientCache.Instance.AddClientValue(clientName, time);

            return ClientCache.Instance.Cache.ToList<KeyValuePair<string, string>>(); 
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            Console.WriteLine("daco prislo :D " + value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
