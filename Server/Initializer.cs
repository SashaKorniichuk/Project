using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Initializer : DropCreateDatabaseIfModelChanges<Model1>
    {
        protected override void Seed(Model1 context)
        {
            base.Seed(context);

            var str = new List<Game>()
            {
                new Game()
                {
                    Name="Call Of Duty1",
                    Genre="Shooter"
                },
                new Game()
                {
                    Name="Call Of Duty2",
                    Genre="Shooter"
                },
                new Game()
                {
                    Name="Bus simulator",
                    Genre="Simulator"
                },
                new Game()
                {
                    Name="Car simulator",
                    Genre="Simulator"
                },
                new Game()
                {
                    Name="Chess",
                    Genre="Strategy"
                },
                new Game()
                {
                    Name="Checkers",
                    Genre="Strategy"
                },
                new Game()
                {
                    Name="Granny",
                    Genre="Horror"
                },
                new Game()
                {
                    Name="Sweet Home",
                    Genre="Horror"
                }
            };
            context.games.AddRange(str);
            context.SaveChanges();
        }
    }
}
