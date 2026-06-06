using Godot;
using System;
using System.Collections.Generic;
using sgj.Behaviour;
using sgj.Module;
public partial class ModuleBuilder : Node2D
{
    [Export] private PackedScene cell;
    [Export] private PackedScene moduleBodyPrefab;

    [Export] private Vector2I gridSize;

    [Export] private CollisionShape2D _collisionShape2D;

    SortedList<(int, int), ModuleBody> usedModules;
    private ModuleBody coreBody;


    public override void _Ready()
    {
        RectangleShape2D shape = new RectangleShape2D();
        shape.Size = new Vector2(ModuleBody.moduleSize.X * gridSize.X, ModuleBody.moduleSize.Y * gridSize.Y);
        _collisionShape2D.Shape = shape;
        _collisionShape2D.Position = shape.Size / 2;

        usedModules = new SortedList<(int, int), ModuleBody>();


        for (int i = 0; i < gridSize.X; i++)
        {
            for (int j = 0; j < gridSize.Y; j++)
            {
                Node2D node = cell.Instantiate<Node2D>();
                AddChild(node);
                node.Position = new Vector2(ModuleBody.moduleSize.X * i, ModuleBody.moduleSize.Y * j) + ModuleBody.moduleSize / 2;
            }
        }

        coreBody = moduleBodyPrefab.Instantiate<ModuleBody>();
        coreBody.Name = "CoreModuleBody";
        Module coreModule = new Module(FileExtension.EXE, "CoreTest", -1, -1);
        coreBody.Setup(coreModule);

        SetModule(gridSize / 2, coreBody);
    }


    public void OnDraggableReceived(DropReceiver receiver, Draggable draggable)
    {
        bool accepted = false;
        if (draggable.OwnerParent is ModuleBody body)
        {

            Vector2 mousePos = GetLocalMousePosition();

            Vector2I index = LocalToGrid(mousePos);

            if (!OutOfBounds(index))
            {
                if (!IsFree(index))
                {
                    SwapModule(index, body);
                }
                else
                {
                    SetModule(index, body);
                    accepted = true;
                }
            }
        }
        if (accepted) draggable.Accept();
        else draggable.Decline();
    }

    private void SwapModule(Vector2I index, ModuleBody module)
    {
        SetModule(index, module);
    }
    private void SetModule(Vector2I index, ModuleBody body)
    {
        if (body.GetParent() != null)
        {
            body.Reparent(this);
        }
        else
        {
           AddChild(body);
        }
        body.Position = GridToLocalPosition(index) + ModuleBody.moduleSize / 2;
        usedModules.Add((index.X, index.Y), body);
        body.module.x = index.X;
        body.module.y = index.Y;
        body.Draggable.DragStart += OnDragStart;
    }

    private ModuleBody RemoveModule(Vector2I index)
    {
        ModuleBody result = usedModules[(index.X, index.Y)];
        usedModules.Remove((index.X, index.Y));
        result.module.x = -1;
        result.module.y = -1;
        
        result.Draggable.OwnerParent.Reparent(GetTree().Root);
        
        result.Draggable.DragStart -= OnDragStart;
        return result;
    }

    private void OnDragStart(Draggable draggable)
    {
        if (draggable.OwnerParent is ModuleBody body)
        {
            if (usedModules.ContainsValue(body))
            {
                RemoveModule(GetIndex(body));

            }
            else
            {
                draggable.DragStart -= OnDragStart;
            }
        }
        else
        {
            draggable.DragStart -= OnDragStart;
        }
    }

    private bool b = true;
    public void OnBuildButtonPress()
    {
        if (b)
        {
            if (coreBody.module.behaviour is EXEBehaviour moduleCore)
            {
                moduleCore.SetupShip(usedModules);
            }
        }
        else
        {
            ResetModules();
        }

        b = !b;

    }

    private Vector2 GridToLocalPosition(Vector2I index)
    {
        return new Vector2(ModuleBody.moduleSize.X * index.X, ModuleBody.moduleSize.Y * index.Y);
    }

    private bool IsFree(Vector2I index)
    {
        return !usedModules.ContainsKey((index.X, index.Y));
    }

    private bool OutOfBounds(Vector2I index)
    {
        return index.X < 0 || index.X >= gridSize.X || index.Y < 0 || index.Y >= gridSize.Y;
    }

    private Vector2I GetIndex(ModuleBody body)
    {
        (int,int) tuple = usedModules.GetKeyAtIndex(usedModules.IndexOfValue(body));
        return new Vector2I(tuple.Item1, tuple.Item2);
    }


    private Vector2I LocalToGrid(Vector2 worldPos)
    {
        Vector2 pos = worldPos - ModuleBody.moduleSize / 2;

        int x = Math.Clamp((int)Math.Round(pos.X / ModuleBody.moduleSize.X), -1, gridSize.X);
        int y = Math.Clamp((int)Math.Round(pos.Y / ModuleBody.moduleSize.Y), -1, gridSize.Y);
        return new Vector2I(x, y);

    }

    public void ResetModules()
    {
        foreach (ModuleBody body in usedModules.Values)
        {
            Vector2I index = new Vector2I(body.module.x, body.module.y);
            if (body.GetParent() != null)
            {
                body.Reparent(this);
            }
            else
            {
                AddChild(body);
            }
            body.Position = GridToLocalPosition(index) + ModuleBody.moduleSize / 2;
            body.SetActive(false);
        }
    }


}
