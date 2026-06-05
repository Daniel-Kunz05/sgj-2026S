using Godot;
using System;
using System.Collections.Generic;

public partial class ModuleBuilder : Node2D
{
    [Export] private PackedScene cell;
    [Export] private Module moduleTest;
    [Export] private PackedScene bodyTest;
    
    [Export] private Vector2I moduleSize;
    [Export] private Vector2I gridSize;

    [Export] private CollisionShape2D _collisionShape2D;

    SortedList<(int, int), Module> usedModules;




    public override void _Ready()
    {
        RectangleShape2D shape = new RectangleShape2D();
        shape.Size = new Vector2(moduleSize.X * gridSize.X, moduleSize.Y * gridSize.Y);
        _collisionShape2D.Shape = shape;
        _collisionShape2D.Position = shape.Size / 2;

        usedModules = new SortedList<(int, int), Module>();
        

        for (int i = 0; i < gridSize.X; i++)
        {
            for (int j = 0; j < gridSize.Y; j++)
            {
                Node2D node = cell.Instantiate<Node2D>();
                node.Position = new Vector2(moduleSize.X * i, moduleSize.Y * j) + moduleSize /2;
                AddChild(node);
            }
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventKey)
        {
            if (eventKey.Pressed && eventKey.ButtonIndex == MouseButton.Left)
            {
                OnModuleDropped();
            }
        }
    }


    public void OnModuleDropped()
    {
        Vector2 mousePos = GetLocalMousePosition();

        Vector2I index = LocalToGrid(mousePos);

        if (!OutOfBounds(index))
        {
            if (IsFree(index))
            {
                SwapModule(index, moduleTest);
            }
            else
            {
                SetModule(index, moduleTest);
            }
        }
        

    }

    private void SwapModule(Vector2I index, Module module)
    {
        SetModule(index, module);
    }
    private void SetModule(Vector2I index, Module module)
    {
        usedModules.Add((index.X,index.Y), module);
        ModuleBody body = bodyTest.Instantiate<ModuleBody>();
        
        AddChild(body);
        body.Position = GridToLocalPosition(index) + moduleSize /2;

    }

    private Vector2 GridToLocalPosition(Vector2I index)
    {
        return new Vector2(moduleSize.X * index.X, moduleSize.Y * index.Y);
    }

    private bool IsFree(Vector2I index)
    {
        return usedModules.ContainsKey((index.X,index.Y));
    }
    
    private bool OutOfBounds(Vector2I index)
    {
        return index.X < 0 || index.X >= gridSize.X || index.Y < 0 || index.Y >= gridSize.Y;
    }
    
    
    private Vector2I LocalToGrid(Vector2 worldPos)
    {
        Vector2 pos = worldPos - moduleSize / 2;
        
        int x = Math.Clamp((int)Math.Round(pos.X / moduleSize.X), -1, gridSize.X);
        int y = Math.Clamp((int)Math.Round(pos.Y / moduleSize.Y), -1, gridSize.Y);
        return new Vector2I(x,y);

    }
    
    
}
