using System.Collections.Generic;
using ServiceDiscvery.SelfRegisteration.Model;
using Bogus;
using System.Linq;

namespace ServiceDiscvery.SelfRegisteration.Services
{
    public class Repository
    {
        public List<City> Cities { get; set; }

        public Repository()
        {
            GenerateFakeData();
        }

        private void GenerateFakeData()
        {
            Cities = new Faker<City>()
                 .RuleFor(s => s.Id, f => int.Parse(f.Random.Replace("#####")))
                 .RuleFor(s => s.Name, f => f.Locale)
                 .Generate(50).ToList();
        }
    }
}
