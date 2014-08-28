using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace Kata
{
    class AnotherThingProvider : IProvider
    {
        public static List<string> ListOfInsertedAnotherThing = new List<string>();

        public IList<object> GetAll()
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

        public object GetById(int id)
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

        public object GetByName(string name)
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

        public void Update(object objectToImport, object objectInBase, XDocument doc)
        {
            var obj = (from bases in doc.Root.Element("AnotherThing").Elements()
                       where bases.Element("Name").Value == ((XElement)objectInBase).Element("Name").Value
                       select bases).FirstOrDefault();

            obj.Element("StartDate").Value = ((AnotherThing)objectToImport).StartDate.ToString("yyyy-MM-dd");
            obj.Element("EndDate").Value = ((AnotherThing)objectToImport).EndDate != null ? ((DateTime)((AnotherThing)objectToImport).EndDate).ToString("yyyy-MM-dd") : string.Empty;
            obj.Element("ValueDate").Value = ((AnotherThing)objectToImport).ValueDate != null ? ((DateTime)((AnotherThing)objectToImport).ValueDate).ToString("yyyy-MM-dd") : string.Empty;
            obj.Element("Status").Value = ((Byte)Enumeration.Update).ToString();
            obj.Element("Comment").Value = ((AnotherThing)objectToImport).Comment;

            doc.Save("base.xml");
            ListOfInsertedAnotherThing.Add(((AnotherThing)objectToImport).Name);
        }

        public void Insert(object objectToImport, XDocument doc)
        {

            XElement Item = new XElement("Item", new XElement("Name", ((AnotherThing)objectToImport).Name),
                                                new XElement("StartDate", ((AnotherThing)objectToImport).StartDate.ToString("yyyy-MM-dd")),
                                                new XElement("EndDate", ((AnotherThing)objectToImport).EndDate != null ? ((DateTime)((AnotherThing)objectToImport).EndDate).ToString("yyyy-MM-dd") : string.Empty),
                                                new XElement("ValueDate", ((AnotherThing)objectToImport).ValueDate != null ? ((DateTime)((AnotherThing)objectToImport).ValueDate).ToString("yyyy-MM-dd") : string.Empty),
                                                new XElement("Status", ((byte)Enumeration.Insert).ToString()),
                                                new XElement("Comment", ((AnotherThing)objectToImport).Comment)
                                                );
            if (doc.Root.Element("AnotherThing") != null)
            {
                doc.Root.Element("AnotherThing").Add(Item);
            }
            else
            {
                XElement ItemThing = new XElement("AnotherThing");
                ItemThing.Add(Item);
                doc.Root.Add(ItemThing);
            }
            doc.Save("base.xml");
            ListOfInsertedAnotherThing.Add(((AnotherThing)objectToImport).Name);
        }

        public object CreateObject(string line, List<string> headers)
        {
            string[] values = line.Split(';');
            AnotherThing thing = new AnotherThing();
            foreach (var property in thing.GetType().GetProperties())
            {
                string FileColumnsName = GetFileColumnsNameFromMappage(property, "AnotherThing");
                int IndexColumnsInFile = FileColumnsName != null ? GetIndexColumnsInFile(FileColumnsName, headers) : -1;
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
                            bool isDate = DateTime.TryParse(values[IndexColumnsInFile], out TestOfString);
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
            AnotherThing thing = (AnotherThing)objectToImport;
            return ListOfInsertedAnotherThing.Contains(thing.Name);
        }

        public XElement Exist(object objectToImport, XDocument doc)
        {


            var objectInBase = (from thing in doc.Root.Element("AnotherThing").Elements()
                                where (string)thing.Element("Name") == ((AnotherThing)objectToImport).Name
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
            ListOfInsertedAnotherThing = new List<string>();
        }
    }
}
