using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PhotoVs.Engine
{
    public class WebDebugger
    {
        private HttpClient _client;
        private string _url = "http://localhost:3000";
        private JsonSerializerSettings _settings;

        public WebDebugger()
        {
            _client = new HttpClient();
            _settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public void Post(string target, object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None, _settings);
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"data", json}
            });

            Task.Factory.StartNew(() => _client.PostAsync($"{_url}/{target}", content));
        }
    }
}
