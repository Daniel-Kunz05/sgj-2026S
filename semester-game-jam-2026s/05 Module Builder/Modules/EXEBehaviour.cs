
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;
using sgj;
using sgj.Behaviour;
using sgj.Module;
using Vector2 = Godot.Vector2;

public partial class EXEBehaviour(Module module) : Behaviour(module)
{
    enum CoreState
    {
        DRAGGING,
        AIMING,
        FIGHTING
    }

    private CoreState _state;
    private CoreState State
    {
        get => _state;
        set
        {
            _state = value;
            GD.Print(value);
            switch (_state)
            {
                case CoreState.AIMING:
                    _rigidBody2D.GlobalPosition = Body.GlobalPosition;
                    Body.Reparent(_rigidBody2D);
                    arrowPivot.Modulate = new Color(1, 1, 1, 1);
                    _rigidBody2D.Freeze = true;

                    break;
                case CoreState.FIGHTING:
                    arrowPivot.Modulate = new Color(1, 1, 1, 0);
                    _rigidBody2D.Freeze = false;
                    break;
                case CoreState.DRAGGING:
                    arrowPivot.Modulate = new Color(1, 1, 1, 0);
                    _rigidBody2D.Freeze = true;
                    break;
            }

        }
    }

    public ModuleBuilder builder;

    private float speed = 300;

    private RigidBody2D? _rigidBody2D;

    public RigidBody2D RigidBody2D => _rigidBody2D!;

    private List<CollisionShape2D> _shape2Ds;

    private List<ModuleBody> moduleBodiesList = new List<ModuleBody>();

    private Node2D arrowPivot;

    public override void _Ready()
    {
        base._Ready();

        Setup();
    }

    private async void Setup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        _rigidBody2D = new RigidBody2D();
        _rigidBody2D.Freeze = true;

        _rigidBody2D.CollisionLayer = 2;
        _rigidBody2D.CollisionMask = 2;

        PhysicsMaterial physicsMaterial = new PhysicsMaterial();
        physicsMaterial.Friction = 0;
        physicsMaterial.Bounce = 1;
        _rigidBody2D.PhysicsMaterialOverride = physicsMaterial;

        _rigidBody2D.GravityScale = 0;
        _rigidBody2D.LockRotation = true;
        _rigidBody2D.ContinuousCd = RigidBody2D.CcdMode.CastShape;

        Body.GetParent().AddChild(_rigidBody2D);
        _rigidBody2D.GlobalPosition = Body.GlobalPosition;
        Body.Reparent(_rigidBody2D);


        arrowPivot = ResourceLoader.Load<PackedScene>("res://05 Module Builder/ArrowPivot.tscn").Instantiate<Node2D>();
        Body.AddChild(arrowPivot);

        State = CoreState.DRAGGING;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (State == CoreState.FIGHTING)
        {
            _rigidBody2D.LinearVelocity = _rigidBody2D.LinearVelocity.Normalized() * speed;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (State == CoreState.AIMING)
        {
            arrowPivot.Rotation = (Body.GetGlobalMousePosition() - arrowPivot.GlobalPosition).Angle();
        }
        else if (State == CoreState.FIGHTING)
        {
            GD.Print("Ticking ship");
            Tick(delta);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (State != CoreState.AIMING) return;
        if (@event is InputEventMouseButton eventKey)
        {
            if (eventKey.Pressed && eventKey.ButtonIndex == MouseButton.Left)
            {
                Shoot(arrowPivot.Rotation);
            }
        }
    }

    public async void Shoot(float angle)
    {
        Vector2 pos = _rigidBody2D.GlobalPosition;

        State = CoreState.FIGHTING;
        _rigidBody2D.LinearVelocity = Vector2.FromAngle(angle) * speed;

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        _rigidBody2D.GlobalPosition = pos;

    }


    public void SetupShip(SortedList<(int, int), ModuleBody> moduleBodies)
    {

        if (State == CoreState.DRAGGING)
        {
            _shape2Ds = new List<CollisionShape2D>();
            moduleBodiesList = new List<ModuleBody>();
            Database.Instance.modules = [.. moduleBodies.Select((m) => m.Value.module!)];

            foreach (ModuleBody body in moduleBodies.Values)
            {
                moduleBodiesList.Add(body);
                if (body != Body)
                {
                    GD.Print("asdfasdfasfasdf");
                    body.Reparent(Body);
                    body.Position = GetRelativePosition(new Vector2I(body.module.x, body.module.y));

                    body.BattleMode(true);
                }

                var rect = Body.Draggable.GetChild<CollisionShape2D>(0).Shape as RectangleShape2D;

                var rect2 = new RectangleShape2D();
                rect2.Size = rect.Size * Body.Scale * 0.8f;

                var shape = new CollisionShape2D();
                shape.Shape = rect2;
                shape.DebugColor = Colors.Goldenrod;
                _shape2Ds.Add(shape);
                _rigidBody2D.AddChild(shape);
                shape.Position = GetRelativePosition(new Vector2I(body.module.x, body.module.y)) * Body.Scale;

            }
            Body.BattleMode(true);
            State = CoreState.AIMING;
        }
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

        // tick all modules in ship
        foreach (ModuleBody body in moduleBodiesList)
        {
            if (body == Body) return;
            body.module.behaviour?.Tick(delta);
        }
    }

    public override void OnModuleDeath(Module cause)
    {
        Database.AddFighter(Database.Instance.playerName, Database.Instance.gamePath, Database.Instance.modules);
        GetTree().ChangeSceneToFile("res://you_lose.tscn");
        return;
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
        State = CoreState.DRAGGING;
        foreach (var shape in _shape2Ds)
        {
            shape.Free();
        }
    }


}