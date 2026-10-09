using System.Text;
using System.Xml.Serialization;
using XMLSchemaConstraints;

namespace PolygoneHttp;

// Le client interroge le serveur pour obtenir les infos du polygone,
// désérialise le XML reçu en objet Polygone, puis peut renvoyer
// un polygone modifié (sérialisé en XML) au serveur.


internal class Program {


    public static async Task Main(string[] args) {
        //async Task Main() {
        PolygoneClient polygoneClient = new PolygoneClient();
            // --- infos simples ---
            Console.WriteLine("== Nom du polygone ==");
            Console.WriteLine(await polygoneClient.GetAsync("/nom"));

            Console.WriteLine("== Nombre de sommets ==");
            Console.WriteLine(await polygoneClient.GetAsync("/sommets/nombre"));

            Console.WriteLine("== Coordonnées ==");
            Console.WriteLine(await polygoneClient.GetAsync("/sommets"));

            // --- le fichier XML complet, désérialisé en objet Polygone ---
            Console.WriteLine("== Polygone complet (XML -> objet) ==");
            Polygone? p = await polygoneClient.GetPolygoneAsync("/polygone/xml");
            if (p != null) {
                Console.WriteLine("Nom : " + p._Nom);
                Console.WriteLine("Sommets : " + p._sommets.Count);
                Console.WriteLine(p);

                // --- modification locale puis renvoi sérialisé vers le serveur ---
                Polygone envoi = new Polygone("star-modifie");
                foreach (var sommet in p._sommets)
                    envoi._sommets.Add(sommet);

                Console.WriteLine("== Envoi du polygone modifié ==");
                Console.WriteLine(await polygoneClient.PostPolygoneAsync("/polygone", envoi));
       //     }
        }
    }
    
    
}
