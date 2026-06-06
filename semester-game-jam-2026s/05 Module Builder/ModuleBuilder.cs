using Godot;
using System;
using System.Collections.Generic;
using sgj.Behaviour;
using sgj.Module;
public partial class ModuleBuilder : Node2D
{
    [Export] private bool isPlayer = true;
    [Export] private PackedScene cell;
    [Export] private PackedScene moduleBodyPrefab;

    [Export] private Vector2I gridSize;

    [Export] private CollisionShape2D _collisionShape2D;

    private SortedList<(int, int), ModuleBody> usedModules;
    public SortedList<(int, int), ModuleBody> UsedModules => usedModules;

    private ModuleBody coreBody;

    public enum BuildErrorCode
    {
        ALL_GOOD,
        CORE_MISSING,
        NOT_CONNECTED
    }
    public BuildErrorCode IsBuildAccepted()
    {
        if (!usedModules.ContainsValue(coreBody)) return BuildErrorCode.CORE_MISSING;
        foreach (var key in usedModules.Keys)
        {

            if (!((usedModules.ContainsKey((key.Item1 + 1, key.Item2))) || (usedModules.ContainsKey((key.Item1 - 1,
                                                                            key.Item2)))
                                                                        || (usedModules.ContainsKey((key.Item1,
                                                                            key.Item2 + 1))) ||
                                                                        (usedModules.ContainsKey((key.Item1,
                                                                            key.Item2 - 1)))))
                return BuildErrorCode.NOT_CONNECTED;

        }

        return BuildErrorCode.ALL_GOOD;
    }

    public override void _Ready()
    {
        RectangleShape2D shape = new RectangleShape2D();
        shape.Size = new Vector2(ModuleBody.moduleSize.X * gridSize.X, ModuleBody.moduleSize.Y * gridSize.Y);
        _collisionShape2D.Shape = shape;
        _collisionShape2D.Position = shape.Size / 2;

        usedModules = new SortedList<(int, int), ModuleBody>();

        var root = FindChild("GridBackground") as Node2D;
        for (int i = 0; i < gridSize.X; i++)
        {
            for (int j = 0; j < gridSize.Y; j++)
            {
                Node2D node = cell.Instantiate<Node2D>();
                root.AddChild(node);
                node.Position = new Vector2(ModuleBody.moduleSize.X * i, ModuleBody.moduleSize.Y * j) + ModuleBody.moduleSize / 2;
            }
        }

        if (isPlayer)
        {
            coreBody = moduleBodyPrefab.Instantiate<ModuleBody>();
            coreBody.Name = "CoreModuleBody";
            Module coreModule = new Module(FileExtension.EXE, "CoreTest", -1, -1);
            coreBody.Setup(coreModule);
            SetModule(gridSize / 2, coreBody);
        }
    }


    public void OnDraggableReceived(DropReceiver receiver, Draggable draggable)
    {
        bool accepted = false;
        if (isPlayer)
        {
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
        if (isPlayer)
        {
            body.Draggable.DragStart += OnDragStart;
        }
        else
        {
            body.Draggable.AllowDragging = false;
        }
    }

    private ModuleBody RemoveModule(Vector2I index)
    {
        ModuleBody result = usedModules[(index.X, index.Y)];
        usedModules.Remove((index.X, index.Y));
        result.module.x = -1;
        result.module.y = -1;

        result.Draggable.OwnerParent.Reparent(GetTree().Root);

        if (isPlayer)
        {
            result.Draggable.DragStart -= OnDragStart;
        }
        else
        {
            result.Draggable.AllowDragging = false;
        }
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

    public void SetupShip()
    {
        if (coreBody.module.behaviour is EXEBehaviour moduleCore)
        {
            moduleCore.SetupShip(usedModules);
        }

    }

    public void NPCOverwriteModules(Module[] modules)
    {
        usedModules = new SortedList<(int, int), ModuleBody>();
        foreach (var module in modules)
        {
            ModuleBody body = moduleBodyPrefab.Instantiate<ModuleBody>();
            if (module.fileExtension == FileExtension.EXE)
            {
                coreBody = new ModuleBody();
                coreBody.Name = "CoreModuleBody";
            }
            body.Setup(module);
            SetModule(new Vector2I(module.x, module.y), body);
        }
    }

    public void NPCClearModules()
    {
        foreach (ModuleBody body in usedModules.Values)
        {
            if (body.module.fileExtension == FileExtension.EXE)
            {
                //TODO delete rigidbody
            }
            body.QueueFree();
        }
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
        (int, int) tuple = usedModules.GetKeyAtIndex(usedModules.IndexOfValue(body));
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
            GD.Print($"Resetting module: {body.Name} at index {GetIndex(body)}");
            body.module.behaviour?.Reset();

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
            body.BattleMode(false);
        }
    }

    public void ShowBuilder()
    {
        // Animate open builder
        var grid = FindChild("GridBackground") as Node2D;
        grid.Scale = new Vector2(0, 1);
        grid.Visible = true;
        var tween = CreateTween();
        tween.TweenProperty(grid, "scale:x", 1, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.Play();
    }

    public void HideBuilder()
    {
        ((EXEBehaviour)coreBody.module.behaviour).RigidBody2D.Reparent(GetTree().Root.GetNode("Main"));

        // Animate close builder
        var tween = CreateTween();
        var grid = FindChild("GridBackground") as Node2D;
        tween.TweenProperty(grid, "scale:x", 0, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        tween.Play();
        tween.TweenCallback(new Callable(grid, "Hide"));
    }
}
