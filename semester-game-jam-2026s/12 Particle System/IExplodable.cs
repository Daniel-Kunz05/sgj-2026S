using Godot;

public interface IExplodable
{
    private const string pathToParticle = "res://12 Particle System/BoomParticle.tscn";
    private static readonly PackedScene scene = ResourceLoader.Load<PackedScene>(pathToParticle);


    void SpawnExplosion(Node2D caller, Vector2 posToSpawn)
    {
        if(scene == null) {
            GD.PrintErr("Particle scene couldn't be found");
            return;
        }
        var particle = scene.Instantiate();
        caller.GetTree().CurrentScene.AddChild(particle);
        if(particle is Node2D node)
        {
            node.Position = posToSpawn;
            node.Set("Emitting", true);
            var lifetime = node.Get("Lifetime");
            Timer t = new Timer
            {
                OneShot = true,
                WaitTime = (double)lifetime
            };
            t.Timeout += node.QueueFree;

            t.QueueFree();
        }
    }
}