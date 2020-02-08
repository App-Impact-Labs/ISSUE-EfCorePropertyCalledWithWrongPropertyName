using System;
using System.Collections.Generic;
using System.Text;

namespace EfCoreWrongPropertyNameError
{
    public class Translation
    {
        public int Id { get; set; }

        public int LocaleId { get; set; }

        public int ResourceId { get; set; }
        public virtual Resource Resource { get; set; }

        public string Value { get; set; }
    }
}
