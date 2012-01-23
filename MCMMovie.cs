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
            LocalTitle = contents.Element("LocalTitle").Value;
            OriginalTitle = contents.Element("OriginalTitle").Value;
            IMDBrating = float.Parse(contents.Element("IMDBrating").Value);
            ProductionYear = int.Parse(contents.Element("ProductionYear").Value);
            IMDBId = contents.Element("IMDbId").Value;
            Language = contents.Element("Language").Value;
            Country = contents.Element("Country").Value;
            Description = contents.Element("Description").Value;
            Director = contents.Element("Director").Value;
            Tagline = contents.Element("Tagline").Value;
            foreach (XElement person in contents.Element("Persons").Elements("Person"))
            {
                Person newPerson = new Person();
                newPerson.Name = person.Element("Name").Value;
                try{ newPerson.Type = (Person.PersonType)Enum.Parse(typeof(Person.PersonType), person.Element("Type").Value); }
                catch (ArgumentException)
                {
                    newPerson.Type = Person.PersonType.Other;
                    newPerson.PersonTypeOther = person.Element("Type").Value;
                }
                newPerson.Role = person.Element("Role").Value;
                Persons.Add(newPerson);
            }
            foreach (string genre in contents.Element("Genres").Elements("Genre"))
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
}
