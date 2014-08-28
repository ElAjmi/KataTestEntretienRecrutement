using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Kata
{
    public interface IProvider
    {
         Object CreateObject(string line,List<string> headers);
         IList<Object> GetAll();
         Object GetById(int id);
         Object GetByName(string name);

         void Update(object objectToImport, object objectInBase,XDocument doc);
         void Insert(object objectToImport, XDocument doc);


         bool AlReadyImported(object objectToImport);

         XElement Exist(object objectToImport, XDocument doc);



         void InitializeInsertedRow();
    }
}
