using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YesNoApi.Controllers
{
    /// <summary>
    /// The sample values controller.
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        /// <summary>
        /// The example of GET call.
        /// </summary>
        /// <returns>The get values.</returns>
        /// <remarks>GET api/values</remarks>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// The exemple of GET call with specified id.
        /// </summary>
        /// <returns>The get value by id.</returns>
        /// <param name="id">Identifier.</param>
        /// <remarks>GET api/values/5</remarks> 
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value" + id;
        }

        /// <summary>
        /// The example of POST call for specified value.
        /// </summary>
        /// <param name="value">The posted value.</param>
        /// <remarks>POST api/values</remarks> 
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// The exemple of PUT call for the specified id and value.
        /// </summary>
        /// <param name="id">The identifier of value to be update.</param>
        /// <param name="value">The updated Value.</param>
        /// <remarks> PUT api/values/5</remarks>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// The esample of DELETE call for the specified id.
        /// </summary>
        /// <param name="id">The identifier for deleteing a value.</param>
        /// <remarks>DELETE api/values/5</remarks> 
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
