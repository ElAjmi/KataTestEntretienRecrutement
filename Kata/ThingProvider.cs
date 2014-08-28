using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace Kata
{
    public class ThingProvider : IProvider
    {

        public static List<string> ListOfInsertedThing = new List<string>();

        #region Pre-Implemente

        public IList<object> GetAll()
        {
            Thread.Sleep(3000);
            Random random = new Random();
            return (IList<object>)Enumerable.Range(1, 20000)
                .ToList()
                .Select(x => new
                {
                    Id = x,
                    EndDate = random.Next(0, 2) == 1 ? null : (DateTime?)DateTime.Now,
                    Name = string.Format("XXXX{0}", x),
                    Owner = "Me",
                    StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                    Status = random.Next(1, 5)
                })
                .ToList();
        }

        public object GetById(int id)
        {
            Random random = new Random();
            return random.Next(0, 2) == 1 ?
            new Thing
            {
                Id = id,
                EndDate = random.Next(0, 1) == 1 ? null : (DateTime?)DateTime.Now,
                Name = string.Format("Hello, i'm {0}", id),
                Owner = "Me",
                StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = random.Next(1, 5)
            }
                : null;
        }

        public object GetByName(string name)
        {
            Thread.Sleep(100);
            Random random = new Random();
            return random.Next(0, 2) == 1 ?
            new Thing
            {
                Id = random.Next(1, 20000),
                EndDate = random.Next(0, 1) == 1 ? null : (DateTime?)DateTime.Now,
                Name = string.Format("XXXX{0}", name),
                Owner = "Me",
                StartDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                Status = random.Next(1, 5)
            }
            : null;
        }

        #endregion

        public void Update(object objectToImport, object objectInBase, XDocument doc)
        {
           
            var obj = (from bases in doc.Root.Element("Thing").Elements()
                       where bases.Element("Name").Value == ((XElement)objectInBase).Element("Name").Value
                       select bases).FirstOrDefault();

            obj.Element("StartDate").Value = ((Thing)objectToImport).StartDate.ToString("yyyy-MM-dd");
            obj.Element("EndDate").Value = ((Thing)objectToImport).EndDate != null ? ((DateTime)((Thing)objectToImport).EndDate).ToString("yyyy-MM-dd") : string.Empty;
            obj.Element("Status").Value = ((Byte)Enumeration.Update).ToString();

            doc.Save("base.xml");
            ListOfInsertedThing.Add(((Thing)objectToImport).Name);
        }

        public void Insert(object objectToImport, XDocument doc)
        {
           
            XElement Item = new XElement("Item", new XElement("Name", ((Thing)objectToImport).Name),
                                                new XElement("StartDate", ((Thing)objectToImport).StartDate.ToString("yyyy-MM-dd")),
                                                new XElement("EndDate", ((Thing)objectToImport).EndDate != null ? ((DateTime)((Thing)objectToImport).EndDate).ToString("yyyy-MM-dd") : string.Empty),
                                                new XElement("Owner", ((Thing)objectToImport).Owner),
                                                new XElement("Status", ((byte)Enumeration.Insert).ToString())
                                                );
            if (doc.Root.Element("Thing") != null)
            {
                doc.Root.Element("Thing").Add(Item);
            }
            else
            {
                XElement ItemThing = new XElement("Thing");
                ItemThing.Add(Item);
                doc.Root.Add(ItemThing);
            }
            doc.Save("base.xml");
            ListOfInsertedThing.Add(((Thing)objectToImport).Name);
        }

        public object CreateObject(string line, List<string> headers)
        {
            string[] values = line.Split(';');
            Thing thing = new Thing();

            foreach (var property in thing.GetType().GetProperties())
            {
                string FileColumnsName = GetFileColumnsNameFromMappage(property, "Thing");//Recuperation du nom de la colonne Dans le fichier Importé depuis le mappage
                int IndexColumnsInFile = FileColumnsName != null ? GetIndexColumnsInFile(FileColumnsName, headers) : -1;//Recuperation de l'index colonne Dans le fichier Importé
                object toInsert = null;
                if (IndexColumnsInFile != -1)
                {
                    switch (property.PropertyType.Name)
                    {
                        case "DateTime":
                            toInsert = DateTime.Parse(values[IndexColumnsInFile]);
                            break;
                        
                        default:
                            DateTime TestOfString;
                            toInsert = string.IsNullOrEmpty(values[IndexColumnsInFile]) ? null : values[IndexColumnsInFile];
                            bool isDate = DateTime.TryParse(values[IndexColumnsInFile],out TestOfString);
                            if (isDate) 
                            {
                                toInsert = TestOfString;
                            }
                            
                            break;
                    }
                }
                property.SetValue(thing, toInsert, null);
            }

            if (string.IsNullOrEmpty(thing.Name))
            {
                throw new Exception("Name is mandatory");
            }
            return thing;
        }

        private int GetIndexColumnsInFile(string FileColumnsName, List<string> headers)
        {
            return headers.IndexOf(FileColumnsName);
        }

        private string GetFileColumnsNameFromMappage(PropertyInfo property, string ObjectType)
        {

            XDocument Mappage = XDocument.Load("Mappage.xml");
            if (Mappage.Root.Element(ObjectType) == null) 
            {
                throw new Exception("mappage of " + ObjectType + " doesn't Exist");
            }
            return (from map in Mappage.Root.Element(ObjectType).Elements()
                    where map.Name == property.Name
                    select map.Attribute("fileName").Value).FirstOrDefault();
        }

        public bool AlReadyImported(object objectToImport)
        {
            Thing thing = (Thing)objectToImport;
            return ListOfInsertedThing.Contains(thing.Name);
        }

        public XElement Exist(object objectToImport, XDocument doc)
        {
            

            var objectInBase = (from thing in doc.Root.Element("Thing").Elements()
                                where (string)thing.Element("Name") == ((Thing)objectToImport).Name
                                select thing).FirstOrDefault();
            if (objectInBase != null)
            {
                return objectInBase;
            }
            else
            {
                return null;
            }
        }

        public void InitializeInsertedRow()
        {
            ListOfInsertedThing = new List<string>();
        }
    }
}