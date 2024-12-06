namespace Task1_ObjectCreator
{
    public class Author
    {
        public Author(string name, string surname, DateTime birthDate)
        {
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
        }
        public Author()
        {

        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public int Age => DateTime.Now.Year - BirthDate.Year;

        // Part 4
         public DateTime BirthDate { get; set; }

        public override string? ToString()
        {
            return $"Author {Name} {Surname} (Age {Age})";
        }
    }
}
