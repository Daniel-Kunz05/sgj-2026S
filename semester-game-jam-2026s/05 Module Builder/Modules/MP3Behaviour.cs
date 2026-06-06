using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class MP3Behaviour(Module module) : Behaviour(module)
{
    private double spawnInterval = 0.5f;
    private double spawnTimer = 0;

    private bool isDead = false;

    private PackedScene musicProjectileScene = GD.Load<PackedScene>("res://05 Module Builder/Projectiles/music_projectile.tscn");

    public override void OnModuleDeath(Module cause)
    {
        isDead = true;
    }

    public override void OnModuleHit(Module m1, Module _)
    {
        m1.EmitSignalOnModuleDeathExtern(m1);    
    }

    public override void Reset()
    {
        isDead = false;
        spawnTimer = 0;
    }

    public override void Tick(double delta)
    {
        if (isDead) return;

        spawnTimer += (float)delta;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;

            for (int i = 0; i < 4; i++)
            {
                GD.Print("Spawning music projectile");
                var projectile = musicProjectileScene.Instantiate<Projectile>();

                projectile.Position = Body.GlobalPosition;
                projectile.Setup(this, Body.GlobalRotation + Mathf.Pi / 2 * i);
            }
        }
    }
}
