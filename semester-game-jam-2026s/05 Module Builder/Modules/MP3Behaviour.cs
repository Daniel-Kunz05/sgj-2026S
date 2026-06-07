using System.Collections.Generic;
using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class MP3Behaviour(Module module) : Behaviour(module)
{
    private double spawnInterval = 0.5f;
    private double spawnTimer = 0;
    private bool isDead = false;

    private HashSet<Projectile> projectiles = new HashSet<Projectile>();

    private PackedScene musicProjectileScene = GD.Load<PackedScene>("res://05 Module Builder/Projectiles/music_projectile.tscn");

    public override void OnModuleDeath(Module cause)
    {
        isDead = true;
    }

    public override void OnModuleHit(Module self, Module other)
    {
        self.EmitSignalOnModuleDeathExtern(self);    
    }

    public override void Reset()
    {
        isDead = false;
        spawnTimer = 0;
    }

    public override void TakeDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(double delta)
    {
        foreach (Projectile projectile in projectiles)
        {
            projectile.ApplyTick(delta);
        }

        if (isDead) return;

        spawnTimer += (float)delta;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;

            for (int i = 0; i < 4; i++)
            {
                GD.Print("Spawning music projectile");
                var projectile = musicProjectileScene.Instantiate<Projectile>();

                GetTree().Root.GetNode("Main").AddChild(projectile);
                projectiles.Add(item: projectile);

                projectile.Position = Body.GlobalPosition;
                projectile.Setup(this, Body.GlobalRotation + Mathf.Pi / 2 * i);
            }
        }
    }

    public void RemoveProjectile(Projectile projectile)
    {
        projectiles.Remove(projectile);
    }
}
