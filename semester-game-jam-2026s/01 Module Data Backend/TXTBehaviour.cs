
namespace sgj.Behaviour;

public partial class TXTBehaviour(Module.Module module) : Behaviour(module)
{
    public override void OnModuleDeath(Module.Module cause)
    {

    }

    public override void Reset()
    {
        
    }

    public override void OnModuleHit(Module.Module m1, Module.Module m2)
    {
        // TODO death
        if (m1.behaviour == this)
        {

        }
        else if (m2.behaviour == this)
        {

        }
    }

    public override void Tick(double delta)
    {

    }
}