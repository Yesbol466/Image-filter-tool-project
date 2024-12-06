using System;

namespace Task1_ObjectCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Part 1
            while (true)
            {
                try
                {
                    Author author = TypeCrafter.TypeCraft<Author>();
                    Console.WriteLine(author);
                    break;
                }
                catch(ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Part 2
            while (true)
            {
                try
                {
                    Book book = TypeCrafter.TypeCraft<Book>();
                    Console.WriteLine(book);
                    break;
                }
                catch(ParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
           
        }
    }
}
