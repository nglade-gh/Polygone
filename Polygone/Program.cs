using System;
using XMLSchemaConstraints;

internal class Program {
    public static void Main(string[] args) {
       
        XMLManager<Polygone> Polygone_Serializer;
        Polygone_Serializer = new XMLManager<Polygone>();
        Polygone p = Polygone_Serializer.Load("./polygone.xml");
        Console.WriteLine(p+"\n\n");
        
        XMLManager<SVG> SVG_Serializer;
        SVG_Serializer = new XMLManager<SVG>();
        SVG svg = SVG_Serializer.Load("./polygone.svg");
        Console.WriteLine(svg);

        Polygone p2 = svg._Poly.toPolygone("star");
        Console.WriteLine(p2);

    }
}