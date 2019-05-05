using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Common;

namespace DataService
{
    public class DummyService : IDataService
    {
        public List<Thing> GetThings()
        {
            // The file SampleThings.xml is an embedded resource
            List<Thing> results = null;
            using (Stream str = this.GetType().Assembly.GetManifestResourceStream("DataService.SampleThings.xml"))
            {
                using (XmlReader reader = new XmlTextReader(str))
                {
                    XDocument doc = XDocument.Load(reader);
                    var q = from item in doc.Descendants("Thing")
                            select new Thing()
                            {
                                Year = Convert.ToInt32(item.Element("Year").Value),
                                Title = item.Element("Title").Value,
                                Author = item.Element("Author").Value,
                            };
                    results = q.ToList();
                }
            }
            return results;
        }
    }
}
