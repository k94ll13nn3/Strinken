//#r "..\Strinken.dll"

using Strinken;

public class Person
{
    public string Name { get; set; }
}

public class NameTag : ITag<Person>
{
    public string Description => "Returns the name of a Person.";
    public string Name => "Name";
    public string Resolve(Person value) => value.Name.ToString();
}

var parser = new Parser<Person>().WithTag(new NameTag());

var result = parser.Resolve("My name is {Name}.", new Person { Name = "James" }); // will return "My name is James."

System.Console.WriteLine(result);