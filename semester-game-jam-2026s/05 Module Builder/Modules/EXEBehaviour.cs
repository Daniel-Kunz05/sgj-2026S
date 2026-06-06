
using System.Collections.Generic;
using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class EXEBehaviour(Module module) : Behaviour(module)
{
    public ModuleBuilder builder;

    private Vector2 direction;
    private float speed = 300;
    private bool isMoving;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isMoving)
        {
            Body.Position += direction * speed * (float)delta;
        }
    }

    private void OnCollision(Vector2 colPos)
    {
        Vector2 diff = (Body.Position - colPos);
        if (Mathf.Abs(diff.X) > Mathf.Abs(diff.Y))
        {
            if (diff.X > 0) //right
            {
                direction.X = -direction.X;

            }
            else //left
            {
                direction.X = -direction.X;

            }
                
        }
        else
        {
            if (diff.Y > 0)
            {
                direction.Y = -direction.Y;
            }
            else
            {
                direction.Y = -direction.Y;
            }
        }

    }


    public void SetupShip(SortedList<(int,int), ModuleBody> moduleBodies)
    {
        foreach (ModuleBody body in moduleBodies.Values)
        {
            if (body != Body)
            {
                body.Reparent(Body);
                body.Position = GetRelativePosition(new Vector2I(body.module.x, body.module.y));
                body.SetActive(true);
                body.OnCollision += OnCollision;
            }
            
        }

        Body.SetActive(true);
        Body.OnCollision += OnCollision;

        
        //REMOVE
        Body.Position = new Vector2(960, 520);
        direction = Vector2.Left;
        isMoving = true;

    }

    public Vector2 GetRelativePosition(Vector2I index)
    {
        index = index - new Vector2I(module.x, module.y);
        return new Vector2(ModuleBody.moduleSizeX * index.X, ModuleBody.moduleSizeY * index.Y) / Body.Scale;
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