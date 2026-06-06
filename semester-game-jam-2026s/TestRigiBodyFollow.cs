using Godot;
using System;

public partial class TestRigiBodyFollow : Node2D
{
    [Export] private RigidBody2D _rigidBody2D;

    private float speed = 30;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition = _rigidBody2D.GlobalPosition;

        _rigidBody2D.LinearVelocity = _rigidBody2D.LinearVelocity.Normalized() * speed;

    }
}
