using System.Text;
using System.Xml.Serialization;
using XMLSchemaConstraints;

namespace PolygoneHttp;

// Le client interroge le serveur pour obtenir les infos du polygone,
// désérialise le XML reçu en objet Polygone, puis peut renvoyer
// un polygone modifié (sérialisé en XML) au serveur.

public class PolygoneClient {

    // constuit un client http avec l'adresse du serveur
    private static readonly HttpClient _http = new HttpClient() {
        BaseAddress = new Uri("http://localhost:8080/")
    };
    

    // GET simple : renvoie la réponse brute en texte
    public async Task<string> GetAsync(string path) {
        HttpResponseMessage res = await _http.GetAsync(path);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsStringAsync();
    }

    
    
    // GET du XML complet + désérialisation en Polygone
    public async Task<Polygone?> GetPolygoneAsync(string path) {
        HttpResponseMessage res = await _http.GetAsync(path);
        res.EnsureSuccessStatusCode();
        string xml = await res.Content.ReadAsStringAsync();

        var serializer = new XmlSerializer(typeof(Polygone));
        using var reader = new StringReader(xml);
        return (Polygone?)serializer.Deserialize(reader);
    }
    
    
    // POST : sérialise le polygone en XML et l'envoie au serveur
    public async Task<string> PostPolygoneAsync(string path, Polygone p) {
        var serializer = new XmlSerializer(typeof(Polygone));
        using var writer = new StringWriter();
        serializer.Serialize(writer, p);

        var content = new StringContent(writer.ToString(), Encoding.UTF8, "application/xml");
        HttpResponseMessage res = await _http.PostAsync(path, content);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsStringAsync();
    }
}