using Godot;

public interface IExplodable
{
    private const string pathToParticle = "res://12 Particle System/BoomParticle.tscn";
    private static readonly PackedScene scene = ResourceLoader.Load<PackedScene>(pathToParticle);


    public void SpawnExplosion(Node2D caller, Vector2 posToSpawn)
    {
        if(caller == null) {
            GD.PrintErr("Caller is null");
            return;
        }

        //GD.Print("Entered SpawnExplosion()");

        if(scene == null) {
            GD.PrintErr("Particle scene couldn't be found");
            return;
        }

        Node particle = scene.Instantiate();
        if(particle == null) {
            GD.PrintErr("Particle  is null");
            return;
        }

        caller.GetTree().CurrentScene.AddChild(particle);
        if(particle is Node2D node)
        {
            //GD.Print("Ready to boom");
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