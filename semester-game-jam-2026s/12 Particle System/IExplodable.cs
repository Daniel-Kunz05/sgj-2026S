using Godot;
using sgj.Behaviour;

public interface IExplodable
{

    private const string pathToParticle = "res://12 Particle System/BoomParticle.tscn";
    private static readonly PackedScene scene = ResourceLoader.Load<PackedScene>(pathToParticle);

    private const string pathToGodot = "res://12 Particle System/GodotBoom.tscn";
    private static readonly PackedScene GODOTBoom = ResourceLoader.Load<PackedScene>(pathToGodot);
    private const string pathToPDF = "res://12 Particle System/PDFBoom.tscn";
    private static readonly PackedScene PDFBoom = ResourceLoader.Load<PackedScene>(pathToPDF);
    private const string pathToEXE = "res://12 Particle System/EXEBoom.tscn";
    private static readonly PackedScene EXEBoom = ResourceLoader.Load<PackedScene>(pathToEXE);
    private const string pathToJSON = "res://12 Particle System/JSONBoom.tscn";
    private static readonly PackedScene JSONBoom = ResourceLoader.Load<PackedScene>(pathToJSON);
    private const string pathToMP3 = "res://12 Particle System/MP3Boom.tscn";
    private static readonly PackedScene MP3Boom = ResourceLoader.Load<PackedScene>(pathToMP3);
    private const string pathToMP4 = "res://12 Particle System/MP4Boom.tscn";
    private static readonly PackedScene MP4Boom = ResourceLoader.Load<PackedScene>(pathToMP4);
    private const string pathToOBJ = "res://12 Particle System/OBJBoom.tscn";
    private static readonly PackedScene OBJBoom = ResourceLoader.Load<PackedScene>(pathToOBJ);
    private const string pathToPNG = "res://12 Particle System/PNGBoom.tscn";
    private static readonly PackedScene PNGBoom = ResourceLoader.Load<PackedScene>(pathToPNG);
    private const string pathToSH = "res://12 Particle System/SHBoom.tscn";
    private static readonly PackedScene SHBoom = ResourceLoader.Load<PackedScene>(pathToSH);
    private const string pathToTXT = "res://12 Particle System/TXTBoom.tscn";
    private static readonly PackedScene TXTBoom = ResourceLoader.Load<PackedScene>(pathToTXT);
    private const string pathToZIP = "res://12 Particle System/ZIPBoom.tscn";
    private static readonly PackedScene ZIPBoom = ResourceLoader.Load<PackedScene>(pathToZIP);


    public void SpawnExplosion(Node2D caller, Vector2 posToSpawn, FileExtension fileType)
    {
        if(caller == null) {
            GD.PrintErr("Caller is null");
            return;
        }

        if(scene == null) {
            GD.PrintErr("Particle scene couldn't be found");
            return;
        }

        Node particle;
        switch (fileType)
        {
            case FileExtension.PDF: particle = PDFBoom.Instantiate(); break;
            case FileExtension.EXE: particle = EXEBoom.Instantiate(); break;
            case FileExtension.GODOT: particle = GODOTBoom.Instantiate(); break;
            case FileExtension.JSON: particle = JSONBoom.Instantiate(); break;
            case FileExtension.MP3: particle = MP3Boom.Instantiate(); break;
            case FileExtension.MP4: particle = MP4Boom.Instantiate(); break;
            case FileExtension.OBJ: particle = OBJBoom.Instantiate(); break;
            case FileExtension.PNG: particle = PNGBoom.Instantiate(); break;
            case FileExtension.SH: particle = SHBoom.Instantiate(); break;
            case FileExtension.TXT: particle = TXTBoom.Instantiate(); break;
            case FileExtension.ZIP: particle = ZIPBoom.Instantiate(); break;
            default: particle = null; break;
        }

        if(particle == null) {
            GD.PrintErr("Particle  is null");
            return;
        }

        caller.GetTree().CurrentScene.AddChild(particle);
        if(particle is Node2D node)
        {
            node.GlobalPosition = posToSpawn;
            node.Set("emitting", true);
            var lifetime = node.Get("lifetime");
            Timer t = new Timer
            {
                OneShot = true,
                WaitTime = (double)lifetime
            };
            t.Timeout += node.QueueFree;

            t.QueueFree();
        } else
        {
            GD.Print("Sth went wrong");
        }
    }
}