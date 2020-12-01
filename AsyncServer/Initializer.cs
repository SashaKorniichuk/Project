using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncServer
{
    class Initializer : DropCreateDatabaseIfModelChanges<Model1>
    {
        protected override void Seed(Model1 context)
        {
            base.Seed(context);

            var str = new List<Street>()
            {
                new Street()
                {
                    Name="Soborna 287",
                    Index=1
                },
                 new Street()
                {
                    Name="Soborna 69",
                    Index=1
                },
                  new Street()
                {
                    Name="Verbova 3",
                    Index=2
                },
                   new Street()
                {
                    Name="Guryeva 1",
                    Index=3
                },
                    new Street()
                {
                    Name="Soborna 46",
                    Index=1
                },
                     new Street()
                {
                    Name="Guryeva 76",
                    Index=3
                },
                      new Street()
                {
                    Name="Soborna 290",
                    Index=1
                }
            };
            context.streets.AddRange(str);
            context.SaveChanges();
        }
    }
}
