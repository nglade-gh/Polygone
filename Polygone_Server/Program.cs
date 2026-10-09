namespace PolygoneHttp;


internal class Program {
    
    public static void Main(string[] args) {
        PolygoneServer server = new PolygoneServer("data/XML/polygone.xml");
        server.Start().GetAwaiter().GetResult();
    }
    
    
}