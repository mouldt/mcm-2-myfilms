using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace MCM2MyFilms
{
    public class MCMMovie
    {
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
            Initialize();
            XDocument metadata = XDocument.Load(Path.Combine(path, "mymovies.xml"));
            XElement contents = metadata.Descendants("Title").First();
            LocalTitle = contents.ElementByNames("LocalTitle").Value;
            OriginalTitle = contents.ElementByNames("OriginalTitle").Value;
            IMDBrating = float.Parse(contents.ElementByNames("IMDBrating").Value);
            ProductionYear = int.Parse(contents.ElementByNames("ProductionYear").Value);
            IMDBId = contents.ElementByNames("IMDbId").Value;
            Language = contents.ElementByNames("Language").Value;
            Country = contents.ElementByNames("Country").Value;
            Description = contents.ElementByNames("Description").Value;
            Director = contents.ElementByNames("Director").Value;
            Tagline = contents.ElementByNames("Tagline", "TagLine").Value;
            foreach (XElement person in contents.ElementByNames("Persons").ElementsByNames("Person"))
            {
                Person newPerson = new Person();
                newPerson.Name = person.ElementByNames("Name").Value;
                try { newPerson.Type = (Person.PersonType)Enum.Parse(typeof(Person.PersonType), person.ElementByNames("Type").Value); }
                catch (ArgumentException)
                {
                    newPerson.Type = Person.PersonType.Other;
                    newPerson.PersonTypeOther = person.ElementByNames("Type").Value;
                }
                newPerson.Role = person.ElementByNames("Role").Value;
                Persons.Add(newPerson);
            }
            foreach (string genre in contents.ElementByNames("Genres").ElementsByNames("Genre"))
                Genres.Add(genre);
            
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
            Persons = new List<Person>();
            Genres = new List<String>();            
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
        public List<Person> Persons { get; set; }
        public List<String> Genres { get; set; }

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
    }
}
