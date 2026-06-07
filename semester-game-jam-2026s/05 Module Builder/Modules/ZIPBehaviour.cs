using Godot;

namespace sgj._05_Module_Builder.Modules;

public partial class ZIPBehaviour(Module.Module module) : Behaviour.Behaviour(module), IExplodable
{
    private const int max_health = 1;
    private int current_health = max_health;
    
    public override void OnModuleHit(Module.Module self, Module.Module other)
    {
        
    }

    public override void Tick(double delta)
    {
        
    }

    public override void OnModuleDeath(Module.Module cause)
    {
        ((IExplodable) this).SpawnExplosion(Body, Body.GlobalPosition);
        Body.KnockOut();
    }

    public override void Reset()
    {
        current_health = max_health;
    }

    public override void TakeDamage(int amount)
    {
        current_health -= amount;
        if (current_health <= 0)
        {
            module.EmitSignalOnModuleDeathExtern(module);
        }
    }
}