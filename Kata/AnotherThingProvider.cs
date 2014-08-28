using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kata
{
    class AnotherThingProvider : IProvider
    {
        public static List<AnotherThing> ListOfInsertedAnotherThing = new List<AnotherThing>();

        public  IList<object> GetAll()
        {
            Thread.Sleep(3000);
            Random random = new Random();
            return (IList<object>)Enumerable.Range(1, 20000)
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

        public  object GetById(int id)
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

        public  object GetByName(string name)
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

        public  void Update(object thing)
        {
            //Persists
        }

        public  void Insert(object thing)
        {
            //Persists
        }

        public object CreateObject(string line)
        {
            throw new NotImplementedException();
        }




        public void TraitLine(string line)
        {
            throw new NotImplementedException();
        }


        public bool AlReadyImported(object objectToImport)
        {
            throw new NotImplementedException();
        }

        public bool Exist(object objectToImport)
        {
            throw new NotImplementedException();
        }
    }
}
