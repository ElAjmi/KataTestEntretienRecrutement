using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Kata
{
    public static class TraitmentService
    {
        public static IProvider provider;
        public static XDocument Doc;

        public static void VerifyAndExecuteFile(string filePath)
        {

            if (string.IsNullOrEmpty(filePath)) 
            {
                throw new Exception("FilePath Is Empty");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception("File Doesn't Exist");
            }
            else 
            {
                string fileName = Path.GetFileName(filePath);
                InitializeProvider(fileName);
                TraitFile(filePath,Doc);
            }

            
        }

        private static void InitializeProvider(string fileName)
        {
            if (fileName.StartsWith("Thing"))
            {
                provider = new ThingProvider();
                Doc = InitializeBase("Thing");
            }
            else if (fileName.StartsWith("AnotherThing"))
            {
                provider = new AnotherThingProvider();
                Doc = InitializeBase("AnotherThing");
            }
            else 
            {
                throw new Exception("Provider not Specified for this Entity");

            }
        }

        private static XDocument InitializeBase(string objectType)
        {
            XDocument doc;
            if (File.Exists("base.xml"))
            {
                doc = XDocument.Load("base.xml");
                if (doc.Root.Element(objectType) == null)
                {

                    XElement ItemThing = new XElement(objectType);
                    doc.Root.Add(ItemThing);
                    doc.Save("base.xml");
                }
            }
            else
            {
                doc = new XDocument();
                XElement root = new XElement("base");
                XElement ItemThing = new XElement(objectType);
                root.Add(ItemThing);
                doc.Add(root);
                doc.Save("base.xml");
            }
            return doc;
        }

        private static IEnumerable<string> ReadAllLines(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private static void TraitFile(string filePath, XDocument Doc)
        {

            string Message = string.Empty;
            try
            {
                provider.InitializeInsertedRow();
                List<string> headers = ReadAllLines(filePath).FirstOrDefault().Split(';').ToList();
                foreach (var line in ReadAllLines(filePath).Skip(1))
                {
                    try
                    {
                        TraitLine(line, headers, Doc);
                    }
                    catch (Exception lineException)
                    {
                        Console.WriteLine(lineException.Message);
                        Console.WriteLine("Continuer le traitement des autres lignes (O/N) : ");
                        ConsoleKeyInfo consolKey = Console.ReadKey();
                        if (consolKey.Key == ConsoleKey.N)
                        {
                            
                            break;
                        }
                        else if (consolKey.Key == ConsoleKey.O)
                        {
                            continue;
                        }
                        
                    }
                    
                }
                Console.Write("Import Terminé");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.ReadKey();
            }
        }

        private static void TraitLine(string line, List<string> headers, XDocument Doc)
        {
           
                object objectToImport = provider.CreateObject(line, headers);
                if (provider.AlReadyImported(objectToImport))
                {
                    throw new Exception("Incorrect inputs - Multiple occurences of " + line);
                }
                else
                {
                    object ObjectInBase = provider.Exist(objectToImport, Doc);
                    if (ObjectInBase != null)
                    {
                        provider.Update(objectToImport, ObjectInBase, Doc);
                    }
                    else
                    {
                        provider.Insert(objectToImport, Doc);
                    }
                }
            
        }
    }
}
