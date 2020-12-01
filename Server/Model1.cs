namespace Server
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Model1 : DbContext
    {

        public Model1()
            : base("name=Model1")
        {
            Database.SetInitializer<Model1>(new Initializer());
        }

        public virtual DbSet<Game> games { get; set; }
    }

}