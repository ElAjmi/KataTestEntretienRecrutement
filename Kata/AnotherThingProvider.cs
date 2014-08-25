using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kata
{
    class AnotherThingProvider
    {
        public static IList<AnotherThing> GetAllAnotherThings()
        {
            Thread.Sleep(3000);
            Random random = new Random();
            return Enumerable.Range(1, 20000)
                .ToList()
                .Select(x => new AnotherThing
                {
                    Id = x,
                    EndDate = random.Next(0, 2) == 1 ? null : (DateTime?)DateTime.Now,
                    Name = string.Format("XXXX{0}", x),
                    StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                    Status = random.Next(1, 5)
                })
                .ToList();
        }

        public static AnotherThing GetAnotherThingById(int id)
        {
            Random random = new Random();
            return random.Next(0, 2) == 1 ?
            new AnotherThing
            {
                Id = id,
                EndDate = random.Next(0, 1) == 1 ? null : (DateTime?)DateTime.Now,
                Name = string.Format("Hello, i'm {0}", id),                
                StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = random.Next(1, 5)
            }
                : null;
        }

        public static AnotherThing GetAnotherThingByName(string name)
        {
            Thread.Sleep(100);
            Random random = new Random();
            return random.Next(0, 2) == 1 ?
            new AnotherThing
            {
                Id = random.Next(1, 20000),
                EndDate = random.Next(0, 1) == 1 ? null : (DateTime?)DateTime.Now,
                Name = string.Format("XXXX{0}", name),
                StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = random.Next(1, 5)
            }
            : null;
        }

        public static void Update(Thing thing)
        {
            //Persists
        }

        public static void Insert(Thing thing)
        {
            //Persists
        }
    }
}
