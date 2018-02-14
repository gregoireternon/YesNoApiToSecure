using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace YesNoApi.Controllers
{
    /// <summary>
    /// The No controller.
    /// </summary>
    [Produces("application/json")]
    [Route("api/No")]
    public class NoController : Controller
    {
        /// <summary>
        /// Get the resulting 'No' value form the YesNo API with corresponding GIF image.
        /// </summary>
        /// <returns>The 'No' value..</returns>
        [HttpGet]
        public async Task<object> Get()
        {
            HttpClient client = new HttpClient();
            string res = await client.GetStringAsync("https://yesno.wtf/api?force=no");
            return JsonConvert.DeserializeObject<object>(res);

        }
    }
}