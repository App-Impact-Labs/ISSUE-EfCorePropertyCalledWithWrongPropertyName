namespace EfCoreWrongPropertyNameError
{
    public class Entity
    {
        public int Id { get; set; }

        public int NameId { get; set; }
        public virtual Resource Name { get; set; }

        public int DescriptionId { get; set; }
        public virtual Resource Description { get; set; }
    }
}
