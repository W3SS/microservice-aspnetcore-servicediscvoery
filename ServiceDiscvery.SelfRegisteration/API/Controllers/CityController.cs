using Microsoft.AspNetCore.Mvc;
using ServiceDiscvery.SelfRegisteration.Model;
using ServiceDiscvery.SelfRegisteration.Services;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ServiceDiscvery.SelfRegisteration.Controllers
{
    [Route("api/[controller]")]
    public class CityController : Controller
    {
        private Repository repository;
        public CityController(Repository repo)
        {
            repository = repo;
        }
        // GET api/values
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var result =repository.Cities;
            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8, "application/json")
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public HttpResponseMessage Get(int id)
        {
            var result= repository.Cities.FirstOrDefault(city => city.Id == id);

            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8, "application/json") 
            };
        }

        // POST api/values
        [HttpPost]
        public HttpResponseMessage Post([FromBody]City city)
        {
            repository.Cities.Add(city);
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public HttpResponseMessage Put(int id, [FromBody]City city)
        {
            var city1= repository.Cities.SingleOrDefault(c => c.Id == id);

            if (city1 == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

            repository.Cities.Remove(city1);
            repository.Cities.Add(city);

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public HttpResponseMessage Delete(int id)
        {
            var city1 = repository.Cities.SingleOrDefault(c => c.Id == id);

            if (city1 == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

            repository.Cities.Remove(city1);
            
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
