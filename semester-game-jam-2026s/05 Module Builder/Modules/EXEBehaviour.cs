
using System.Collections.Generic;
using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class EXEBehaviour(Module module) : Behaviour(module)
{
    public void SetupShip(SortedList<(int,int), ModuleBody> moduleBodies)
    {
        GD.Print("AAAAA");
        foreach (ModuleBody body in moduleBodies.Values)
        {
            body.Reparent(this);
            body.Position = GetRelativePosition(new Vector2I(body.module.x, body.module.y));
        }

        Body.Position = new Vector2(1000, 800);

    }

    public Vector2 GetRelativePosition(Vector2I index)
    {
        return new Vector2(ModuleBody.moduleSizeX * index.X, ModuleBody.moduleSizeY * index.Y);
    }
    
    
    public override void OnModuleHit(Module m1, Module m2)
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(double delta)
    {
        throw new System.NotImplementedException();
    }

    public override void OnModuleDeath(Module cause)
    {
        throw new System.NotImplementedException();
    }
}