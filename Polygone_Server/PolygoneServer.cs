using System.Net;
using System.Text;
using System.Xml.Serialization;
using XMLSchemaConstraints;

namespace PolygoneHttp;

// Le serveur charge le polygone depuis un fichier et un chemin path+filename (XmlPath),
// par défaut polygone.xml dans data/XML/ (désérialisation),
// puis expose ses informations en HTTP. Il peut aussi recevoir un polygone
// sérialisé en XML (POST /polygone) et le sauvegarder.

public class PolygoneServer {

    private const string Prefix = "http://localhost:8080/";
    private const string XmlPath = "data/XML/polygone.xml";

    private Polygone _polygone; // le modèle servi, mis à jour par le client

    /* Constructeur par défaut */
    public PolygoneServer() : this(XmlPath) { }
    
    
    /*
     * Construit à partir d'un chemin+filename.
     * Un Polygone est instancié par désérialisation du fichier xml.
     */
    public PolygoneServer(string XmlPath) {
        // Désérialisation du fichier XML au démarrage, via XMLManager
        Console.WriteLine("Readling XML file ...");
        XMLManager<Polygone> serializer = new XMLManager<Polygone>();
        _polygone = serializer.Load(XmlPath);
        Console.WriteLine("Polygone chargé : " + _polygone._Nom +
                          " (" + _polygone._sommets.Count + " sommets)");
    }

    /*
     * Cette méthode insancie un listener HTTP. 
     */
    public async Task Start() {
        // instancie un listener
        HttpListener listener = new HttpListener();
        // affecte une adresse
        listener.Prefixes.Add(Prefix);
        // démarre le serveur
        listener.Start();
        Console.WriteLine("Serveur démarré sur " + Prefix);
        // démarre la boucle d'écoute
        while (true) {
            // Le contexte récupère les requetes envoyées par le client et recues par le listener
            HttpListenerContext context = await listener.GetContextAsync();
            // prend en charge la requete via HandleRequest(context)
            // Task.Run(() => ...) : crée une tâche qui exécutera cette méthode sur un thread du pool de threads (en tâche de fond), et retourne immédiatement un objet Task.
            // _ = ... : on ignore volontairement la Task retournée. Le _ est une variable « discard » : elle signale au compilateur (et au lecteur) qu'on n'a pas l'intention d'attendre ou d'observer le résultat.
            _ = Task.Run(() => HandleRequest(context)); // traitement non bloquant
        }
    }

    /*
     * Cette méthode gère les requetes et leur répond
     */
    private async Task HandleRequest(HttpListenerContext context) {
        HttpListenerRequest req = context.Request;
        HttpListenerResponse res = context.Response;
        string path = req.Url!.AbsolutePath;

        Console.WriteLine(req.HttpMethod + " " + path);

        try {
            string body;
            string contentType = "text/plain; charset=utf-8";

            switch (path) {

                // --- infos simples ---
                // renvoie le nom du polygone
                case "/nom":
                    body = _polygone._Nom;
                    break;

                // renvoie le nombre de sommets du polygone
                case "/sommets/nombre":
                    body = _polygone._sommets.Count.ToString();
                    break;

                // renvoie la liste des sommets (valeurs X et Y)
                case "/sommets":
                    body = _polygone.ToString(); // "(X,Y)" ligne par ligne
                    break;

                // --- le fichier XML complet, re-sérialisé depuis l'objet ---
                case "/polygone/xml": {
                    contentType = "application/xml";
                    var xml = new XmlSerializer(typeof(Polygone));
                    using var writer = new StringWriter();
                    xml.Serialize(writer, _polygone);
                    body = writer.ToString();
                    break;
                }

                // --- réception d'un polygone sérialisé ---
                case "/polygone" when req.HttpMethod == "POST": {
                    using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                    string raw = await reader.ReadToEndAsync();

                    Polygone recu;
                    using var stringReader = new StringReader(raw);
                    var xml = new XmlSerializer(typeof(Polygone));
                    recu = (Polygone)xml.Deserialize(stringReader)!;

                    // sauvegarde dans le fichier (sérialisation) puis mise à jour du modèle
                    XMLManager<Polygone> manager = new XMLManager<Polygone>();
                    manager.Save(XmlPath, recu);
                    _polygone = recu;

                    body = "OK - polygone '" + recu._Nom + "' reçu ("
                           + recu._sommets.Count + " sommets)";
                    break;
                }

                // par défaut, génère une erreur car reception d'une mauvaise requete
                default:
                    res.StatusCode = 404;
                    body = "Ressource inconnue : " + path;
                    break;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(body);
            res.ContentType = contentType;
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception e) {
            res.StatusCode = 500;
            byte[] buffer = Encoding.UTF8.GetBytes("Erreur serveur : " + e.Message);
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        finally {
            res.Close();
        }
    }
}