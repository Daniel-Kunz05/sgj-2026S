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
        if (Body != null)
        {
            ((IExplodable)this).SpawnExplosion(Body, Body.Position);
            
            var spaceState = module.GetWorld2D().DirectSpaceState;

            var rectShape = new RectangleShape2D();
            rectShape.Size = ((RectangleShape2D)((CollisionShape2D)Body.Draggable.GetChild(0)).Shape).Size * 2;

            var query = new PhysicsShapeQueryParameters2D
            {
                Shape = rectShape,
                Transform = new Transform2D(0, module.GlobalPosition),
                CollideWithAreas = true,
                CollideWithBodies = false
            };

            var results = spaceState.IntersectShape(query);

            foreach (var hit in results)
            {
                var collider = (Node)hit["collider"];
                GD.Print(collider);
                if (collider is Draggable draggable)
                {
                    if (draggable.OwnerParent is ModuleBody body)
                    {
                        body.module.behaviour.TakeDamage(10);
                    }
                }
            }

        }
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