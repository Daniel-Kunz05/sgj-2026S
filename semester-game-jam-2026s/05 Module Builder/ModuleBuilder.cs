using Godot;
using System;
using System.Collections.Generic;
using sgj.Module;
public partial class ModuleBuilder : Node2D
{
    [Export] private PackedScene cell;
    [Export] private Module moduleTest;
    [Export] private PackedScene bodyTest;

    [Export] private Vector2I moduleSize;
    [Export] private Vector2I gridSize;

    [Export] private CollisionShape2D _collisionShape2D;

    SortedList<(int, int), ModuleBody> usedModules;


    public override void _Ready()
    {
        RectangleShape2D shape = new RectangleShape2D();
        shape.Size = new Vector2(moduleSize.X * gridSize.X, moduleSize.Y * gridSize.Y);
        _collisionShape2D.Shape = shape;
        _collisionShape2D.Position = shape.Size / 2;

        usedModules = new SortedList<(int, int), ModuleBody>();


        for (int i = 0; i < gridSize.X; i++)
        {
            for (int j = 0; j < gridSize.Y; j++)
            {
                Node2D node = cell.Instantiate<Node2D>();
                node.Position = new Vector2(moduleSize.X * i, moduleSize.Y * j) + moduleSize / 2;
                AddChild(node);
            }
        }
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
        body.Reparent(this);
        body.Position = GridToLocalPosition(index) + moduleSize / 2;
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

    private Vector2 GridToLocalPosition(Vector2I index)
    {
        return new Vector2(moduleSize.X * index.X, moduleSize.Y * index.Y);
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
        Vector2 pos = worldPos - moduleSize / 2;

        int x = Math.Clamp((int)Math.Round(pos.X / moduleSize.X), -1, gridSize.X);
        int y = Math.Clamp((int)Math.Round(pos.Y / moduleSize.Y), -1, gridSize.Y);
        return new Vector2I(x, y);

    }


}
