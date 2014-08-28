using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kata
{
    public static class TraitmentService
    {
        public static IProvider provider;
        
        public static void VerifyFile (string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) 
            {
                throw new Exception("le chemin du fichier spécifié est vide");
            }
            else if (!File.Exists(filePath))
            {
                throw new Exception("le fichier spécifié n'existe pas");
            }
            else 
            {
                string fileName = Path.GetFileName(filePath);
                InitializeProvider(fileName);
            }

            
        }

        private static void InitializeProvider(string fileName)
        {
            if (fileName.StartsWith("Thing"))
            {
                provider = new ThingProvider();
            }
            else if (fileName.StartsWith(""))
            {
                provider = new AnotherThingProvider();
            }
            else 
            {
                throw new Exception("Aucun provider Specifié pour ce type d'entité");

            }
        }

        public static void TraitFile(string filePath)
        {
            try
            {

                StreamReader streamReader = new StreamReader(filePath);
                string line = streamReader.ReadLine();
                while (line != null)
                {
                    TraitLine(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private static void TraitLine(string line)
        {
            object objectToImport = provider.CreateObject(line);
            if (provider.AlReadyImported(objectToImport))
            {

            }
            else 
            {
                if (provider.Exist(objectToImport))
                {
                    provider.Update(objectToImport);
                }
                else 
                {
                    provider.Insert(objectToImport);
                }
            }
        }
    }
}
