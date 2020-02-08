using System.Collections.Generic;

namespace EfCoreWrongPropertyNameError
{
    public class Resource
    {
        public int Id { get; set; }

        public virtual IList<Translation> Translations { get; set; }
    }
}
