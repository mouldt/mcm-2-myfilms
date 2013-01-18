using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using System.Xml.XPath;

namespace MCM2MyFilms
{
    public class MCMMovie
    {
        static MCMFieldMappingsSection MCMFieldMappings = null;
        static MCMMovie()
        {
            MCMFieldMappings = ConfigurationManager.GetSection("MCMFieldMappings")
                as MCMFieldMappingsSection;
        }

        public class Person
        {
            public enum PersonType
            {
                Actor,
                Director,
                Producer,
                Other
            }
            public string Name { get; set; }
            public PersonType Type { get; set; }
            public string Role { get; set; }
            public string PersonTypeOther { get; set; }
        }

        public MCMMovie(string path)
        {
            MCMFieldMapping mapping = null;

            Initialize();
            XDocument metadata = XDocument.Load(Path.Combine(path, "movie.xml"));
            XElement contents = metadata.Descendants("Title").First();

            mapping = MCMFieldMappings.mappings.Find("LocalTitle");
            LocalTitle = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("OriginalTitle");
            OriginalTitle = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("IMDBrating");
            IMDBrating = float.Parse(contents.SelectElementByXPaths(mapping.XPathArray).Value);

            mapping = MCMFieldMappings.mappings.Find("ProductionYear");
            ProductionYear = int.Parse(contents.SelectElementByXPaths(mapping.XPathArray).Value);

            mapping = MCMFieldMappings.mappings.Find("IMDBId");
            IMDBId = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("Language");
            Language = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("Country");
            Country = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("Description");
            Description = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("Director");
            Director = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("FullCertifications");
            FullCertifications = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("MPAARating");
            MPAARating = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("BackdropURL");
            BackdropURL = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("WritersList");
            WritersList = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("Tagline");
            Tagline = contents.SelectElementByXPaths(mapping.XPathArray).Value;

            mapping = MCMFieldMappings.mappings.Find("PersonCollection");
            foreach (XElement person in contents.SelectElementsByXPaths(mapping.XPathArray))
            {
                Person newPerson = new Person();

                mapping = MCMFieldMappings.mappings.Find("PersonName");
                newPerson.Name = person.SelectElementByXPaths(mapping.XPathArray).Value;

                mapping = MCMFieldMappings.mappings.Find("PersonType");
                try { newPerson.Type = (Person.PersonType)Enum.Parse(typeof(Person.PersonType), person.SelectElementByXPaths(mapping.XPathArray).Value); }
                catch (ArgumentException)
                {
                    newPerson.Type = Person.PersonType.Other;
                    newPerson.PersonTypeOther = person.SelectElementByXPaths(mapping.XPathArray).Value;
                }
                mapping = MCMFieldMappings.mappings.Find("PersonRole");
                newPerson.Role = person.SelectElementByXPaths(mapping.XPathArray).Value;
                Persons.Add(newPerson);
            }

            mapping = MCMFieldMappings.mappings.Find("GenresCollection");
            foreach (string genre in contents.SelectElementsByXPaths(mapping.XPathArray))
                Genres.Add(genre);

            try
            {
                mapping = MCMFieldMappings.mappings.Find("StudiosCollection");
                foreach (string studio in contents.SelectElementsByXPaths(mapping.XPathArray))
                    Studios.Add(studio);
            }
            catch (Exception) // No Studios Found
            {
            }

        }

        private void Initialize()
        {
            LocalTitle = String.Empty;
            OriginalTitle = String.Empty;
            IMDBrating = 0;
            ProductionYear = 0;
            IMDBId = String.Empty;
            Language = String.Empty;
            Country = String.Empty;
            Description = String.Empty;
            Director = String.Empty;
            Tagline = String.Empty;
            FullCertifications = String.Empty;
            MPAARating = String.Empty;
            BackdropURL = String.Empty;
            WritersList = String.Empty;

            Persons = new List<Person>();
            Genres = new List<String>();
            Studios = new List<String>();
        }

        public String LocalTitle { get; set; }
        public String OriginalTitle { get; set; }
        public float IMDBrating { get; set; }
        public int ProductionYear { get; set; }
        public String IMDBId { get; set; }
        public String Language { get; set; }
        public String Country { get; set; }
        public String Description { get; set; }
        public String Director { get; set; }
        public String Tagline { get; set; }
        public String FullCertifications { get; set; }
        public String MPAARating { get; set; }
        public String BackdropURL { get; set; }
        public String WritersList { get; set; }
        public List<Person> Persons { get; set; }
        public List<String> Genres { get; set; }
        public List<String> Studios { get; set; }

        public string Certification 
        {
            get
            {
                string ukCert = FullCertifications.Split(new String[]{" | "}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault<string>(e=>e.Contains("UK:"));
                return String.IsNullOrEmpty(ukCert) ? MPAARating :  ukCert.Split(':').Last();
            }
        }
    }

    public static class LinqToXMLExtensions
    {
        public static XElement ElementByNames(this XElement element, params string[] namestofind)
        {
            foreach (string name in namestofind)
            {
                XElement testElem = element.Element(name);
                if (testElem != null)
                    return testElem;
            }
            throw new Exception(String.Format("Unable to find any element with names: {0}", String.Join(", ", namestofind)));
        }

        public static IEnumerable<XElement> ElementsByNames(this XElement element, params string[] namestofind)
        {
            bool bFound = false;
            foreach (string name in namestofind)
            {
                IEnumerable<XElement> testElems = element.Elements(name);
                if (testElems.Count() > 0)
                {
                    bFound = true;

                    foreach (var testElem in testElems)
                        yield return testElem;
                }
              
            }
            if (!bFound)
                throw new Exception(String.Format("Unable to find any elements with names: {0}", String.Join(", ", namestofind)));
        }

        public static XElement SelectElementByXPaths(this XElement element, params string[] expressions)
        {
            foreach (string expression in expressions)
            {
                XElement target = element.XPathSelectElement(expression);
                if (target != null)
                    return target;
            }
            throw new Exception(String.Format("Unable to find any MCM data at XPaths: {0}", String.Join(", ", expressions)));
        }

        public static IEnumerable<XElement> SelectElementsByXPaths(this XElement element, params string[] expressions)
        {
            bool bFound = false;
            foreach (string expression in expressions)
            {
                IEnumerable<XElement> testElems = element.XPathSelectElements(expression);
                if (testElems.Count() > 0)
                {
                    bFound = true;

                    foreach (var testElem in testElems)
                        yield return testElem;
                }

            }
            if (!bFound)
                throw new Exception(String.Format("Unable to find any MCM data at XPaths: {0}", String.Join(", ", expressions)));
        }

        public static XElement XPathSelectElementByNames(this XElement element, string expression, params string[] namestofind)
        {
            foreach (string name in namestofind)
            {
                XElement target = element.XPathSelectElement(expression+name);
                if (target != null)
                    return target;
            }
            throw new Exception(String.Format("Unable to find any MCM data with XPath: {0} and names: {1}", expression, String.Join(", ", namestofind)));
        }

        public static IEnumerable<XElement> XPathSelectElementsByNames(this XElement element, string expression, params string[] namestofind)
        {
            bool bFound = false;
            foreach (string name in namestofind)
            {
                IEnumerable<XElement> testElems = element.XPathSelectElements(expression + name);
                if (testElems.Count() > 0)
                {
                    bFound = true;

                    foreach (var testElem in testElems)
                        yield return testElem;
                }

            }
            if (!bFound)
                throw new Exception(String.Format("Unable to find any MCM data with XPath: {0} and names: {1}", expression, String.Join(", ", namestofind)));
        }
    }

    public class MCMFieldMappingsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection=true)]
        public MCMFieldMappings mappings 
        {
            get { return (MCMFieldMappings)this[""]; }
        }
    }
    public class MCMFieldMappings : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "Mapping";
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new MCMFieldMapping();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MCMFieldMapping)element).FieldName;
        }
        public MCMFieldMapping Find(string name)
        {
            ConfigurationElement elem = BaseGet(name);
            if (elem == null)
                throw new Exception(String.Format("Unable to elements in configuration with name: {0}", name));
            return elem as MCMFieldMapping;
        }
    }
    public class MCMFieldMapping : ConfigurationElement
    {
        [ConfigurationProperty("FieldName", IsKey = true, IsRequired = true)]
        public string FieldName
        {
            get
            {
                return (string)base["FieldName"];
            }
            set
            {
                base["FieldName"] = value;
            }
        }
        [ConfigurationProperty("MCMFieldNames", IsRequired = true)]
        public string MCMFieldNames
        {
            get
            {
                return (string)base["MCMFieldNames"];
            }
            set
            {
                base["MCMFieldNames"] = value;
            }
        }
        public string[] MCMFieldNamesArray
        {
            get
            {
                string value = (string)base["MCMFieldNames"];
                return value.Split(',');
            }
        }

        [ConfigurationProperty("XPaths", IsRequired = true)]
        public string XPath
        {
            get
            {
                return (string)base["XPaths"];
            }
            set
            {
                base["XPaths"] = value;
            }
        }
        public string[] XPathArray
        {
            get
            {
                List<string> ret = new List<string>();
                string value = (string)base["XPaths"];
                foreach (string item in value.Split(','))
                {
                    if (item.Contains("{MCMFieldName}"))
                    {
                        foreach (string token in this.MCMFieldNamesArray)
	                    {
                            ret.Add(item.Replace("{MCMFieldName}", token));
	                    }
                    }
                    else
                        ret.Add(item);
                }

                return ret.ToArray();
            }
        }

    }
}
