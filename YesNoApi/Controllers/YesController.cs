using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace YesNoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Yes")]
    public class YesController : Controller
    {
        [HttpGet]
        public async Task<object> Get()
        {
            HttpClient client = new HttpClient();
            string res = await client.GetStringAsync("https://yesno.wtf/api/");
            return JsonConvert.DeserializeObject<object>(res);
            
        }
    }
}