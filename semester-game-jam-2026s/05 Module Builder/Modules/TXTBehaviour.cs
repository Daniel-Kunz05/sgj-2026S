
namespace sgj.Behaviour;

public partial class TXTBehaviour(Module.Module module) : Behaviour(module), IExplodable
{
    private const int max_health = 3;
    private int current_health = max_health;
    
    public override void OnModuleDeath(Module.Module cause)
    {
        Body.audioPlayer.PlayExplosionSound(1);
        ((IExplodable) this).SpawnExplosion(Body, Body.GlobalPosition, module.fileExtension);
        Body.KnockOut();
    }

    public override void Reset()
    {
        current_health = max_health;
    }

    public override void TakeDamage(int amount)
    {
        current_health -= amount;
        if (current_health <= 0) module.EmitSignalOnModuleDeathExtern(module);
    }

    public override void OnModuleHit(Module.Module self, Module.Module other)
    {
        other.behaviour.TakeDamage(1);
    }

    public override void Tick(double delta)
    {

    }
}