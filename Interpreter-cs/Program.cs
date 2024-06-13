// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Book hp = new Book("Harry Potter", 300);

Frame frame = new Frame(5, 5, "nice");

Book hp2 = hp with { name = "Harry Potter 2" };


Console.WriteLine(hp == hp2);

Console.WriteLine(frame);

public record Book(string name, int pages)
{
    int amountOfPages = pages;
    string bookTitle = name;
}

public record struct Frame
{
    int sizeX;
    int sizeY;
    string photoURL;

    public Frame(int sizeX, int sizeY, string photoURL)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.photoURL = photoURL;
    }

}


