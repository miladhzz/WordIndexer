using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace BaseIndexer
{
    public class Helper
    {
        public void Serialize(string filename, Export export)
        {
            var serializer = new XmlSerializer(typeof(Export));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, export);
            }
                           
        }
    }
}
