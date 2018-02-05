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
    /// <summary>
    /// Yes controller.
    /// </summary>
    [Produces("application/json")]
    [Route("api/Yes")]
    public class YesController : Controller
    {
        /// <summary>
        /// Get the resulting value form the YesNo API with corresponding GIF image.
        /// </summary>
        /// <returns>The 'Yes' or 'No' value.</returns>
        [HttpGet]
        public async Task<object> Get()
        {
            HttpClient client = new HttpClient();
            string res = await client.GetStringAsync("https://yesno.wtf/api?force=yes");
            return JsonConvert.DeserializeObject<object>(res);
            
        }
    }
}