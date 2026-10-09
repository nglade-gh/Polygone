using System.Numerics;
using System.Xml.Serialization;

namespace XMLSchemaConstraints;

[XmlRoot("polygone", Namespace = "http://myGame/models")]
[Serializable]
public class Polygone {

    [XmlAttribute("nom")] public string _Nom { init; get; }
    [XmlElement("sommet")] public List<Vector2> _sommets;
    
    public override string ToString() {
        string s = "";
        foreach (var sommet in _sommets) {
            s += "(" + sommet.X + "," + sommet.Y + ")\n";
        }
        return s;
    }

    public Polygone(string nom) {
        _Nom = nom;
        _sommets = new List<Vector2>();
    }
    
    public Polygone() {
        _Nom = String.Empty;
        _sommets = new List<Vector2>();
    }
}


public class PolygoneSVG {
    
    private List<Vector2> _points;

    [XmlAttribute("points")]
    public string _Points {
        init {
            string[] pl = value.Split(" ");
            _points = new List<Vector2>();
            foreach(string p in pl) {
                string[] coord = p.Split(",");
                _points.Add(new Vector2(int.Parse(coord[0]), int.Parse(coord[1])));
            }
        }
        get { return _Points; }
    }
    [XmlAttribute("style")] public string _Style { init; get; }
    
    public override string ToString() {
        string s = "";
        foreach (var p in _points) 
            s+="("+p.X+","+p.Y+")\n";
        s += "Styles : ";
        foreach (string style in _Style.Split(";"))
            s += style + ", ";
        return s;
    }

    public Polygone toPolygone(string nom) {
        Polygone pol = new Polygone(nom);
        foreach (var p in _points) 
            pol._sommets.Add(p);
        return pol;
    }
}

[XmlRoot("svg", Namespace = "http://www.w3.org/2000/svg")]
[Serializable]
public class SVG {
    [XmlAttribute("height")] public int _Height { init; get; }
    [XmlAttribute("width")] public int _Width { init; get; }
    [XmlElement("polygon")] public PolygoneSVG _Poly  { init; get; }
    
    public override string ToString() {
        string s = "";
        s += "Width : " + _Width + " - Height : " + _Height + "\n";
        s += _Poly.ToString();
        return s;
    }
}