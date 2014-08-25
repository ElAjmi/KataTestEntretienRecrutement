using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kata
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var reader = new StreamReader(File.OpenRead(args[0]));
            List<Thing> list = new List<Thing>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var thing = new Thing();
                thing.Name = values[0];
                thing.StartDate = DateTime.Parse(values[1]);
                thing.EndDate = DateTime.Parse(values[2]);
                thing.Owner = values[3];
                list.Add(thing);
            }
            
            //Check doublons
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (i != j && list[i].Name == list[j].Name)
                    {
                        Console.WriteLine("Error");
                        throw new Exception("Incorrect inputs - Multiple occurences of " + list[i].Name);
                    }
                }
            }

            //Check Mandatory Field
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Name == null || list[i].Name == "")
                {
                    Console.WriteLine("Error");
                    throw new Exception("Name is mandatory");
                }
            }

            //Persit
            int x = 0;
            int y = 0;
            for (int i = 0; i < list.Count - 1; i++)
            {
                var thingsExists = ThingProvider.GetThingByName(list[i].Name);
                if (thingsExists != null)
                {
                    thingsExists.StartDate = list[i].StartDate;
                    thingsExists.EndDate = list[i].EndDate;
                    ThingProvider.Update(thingsExists);
                    x++;
                }
                else
                {
                    ThingProvider.Insert(thingsExists);
                    y++;
                }
            }
            Console.WriteLine("count : " + x + "Inserts - " + y + "updates");
        }
    }
}