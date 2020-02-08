using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace EfCoreWrongPropertyNameError
{
    class Program
    {
        static void Main(string[] args)
        {
            int localeId = 1;

            using (AppDbContext dbContext = new AppDbContext())
            {
                dbContext.Database.Migrate();

                Dictionary<string, string> seeds = new Dictionary<string, string>
                {
                    { "Alpha", "Third seed entry" },
                    { "Beta", "Second seed entry" },
                    { "Gama", "First seed entry" }
                };

                foreach (KeyValuePair<string, string> seed in seeds)
                {
                    bool exists = dbContext.Entities
                        .Where(e => e.Name.Translations.Any(t => t.Value == seed.Key))
                        .Any();

                    if (!exists)
                    {
                        Entity entity = new Entity
                        {
                            Name = new Resource
                            {
                                Translations = new List<Translation> { new Translation { LocaleId = localeId, Value = seed.Key } }
                            },
                            Description = new Resource
                            {
                                Translations = new List<Translation> { new Translation { LocaleId = localeId, Value = seed.Value } }
                            }
                        };
                        dbContext.Entities.Add(entity);
                    }
                }
                dbContext.SaveChanges();
            }

            string col = "descriptionFormatted,nameFormatted";
            string ord = "ASC";

            using (AppDbContext dbContext = new AppDbContext())
            {
                List<EntityDto> entities = dbContext.Entities
                    .Select(e => new EntityDto
                    {
                        NameFormatted = e.Name.Translations
                         .Where(x => x.LocaleId == localeId)
                         .Select(x => x.Value)
                         .FirstOrDefault(),
                        DescriptionFormatted = e.Description.Translations
                         .Where(x => x.LocaleId == localeId)
                         .Select(x => x.Value)
                         .FirstOrDefault()
                    })
                    .OrderBy($"{col} {ord}")
                    .ToList();

                foreach (EntityDto entity in entities)
                {
                    Console.WriteLine($"{entity.NameFormatted} {entity.DescriptionFormatted}");
                }
            }

            Console.ReadLine();
        }
    }
}
