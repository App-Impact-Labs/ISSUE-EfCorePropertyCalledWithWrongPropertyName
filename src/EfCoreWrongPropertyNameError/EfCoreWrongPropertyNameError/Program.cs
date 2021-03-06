﻿using Microsoft.EntityFrameworkCore;
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
            #region Migration & data seeding - not relevant to the issue

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

            #endregion

            string col = "descriptionFormatted,nameFormatted"; // If one column name is omitted, query will begin to work.
            string ord = "ASC";

            using (AppDbContext dbContext = new AppDbContext())
            {
                List<EntityDto> entities = dbContext.Entities
                    .Join(dbContext.Resources, e => e.NameId, r => r.Id, (e, r) => new { Entity = e, Name = r })
                    .Join(dbContext.Resources, er => er.Entity.DescriptionId, r => r.Id, (er, r) => new { er.Entity, er.Name, Description = r })
                    .Select(e => new EntityDto
                    {
                        NameFormatted = e.Name.Translations
                         .Where(t => t.LocaleId == localeId)
                         .Select(t => t.Value)
                         .FirstOrDefault(),
                        DescriptionFormatted = e.Description.Translations
                         .Where(t => t.LocaleId == localeId)
                         .Select(t => t.Value)
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
