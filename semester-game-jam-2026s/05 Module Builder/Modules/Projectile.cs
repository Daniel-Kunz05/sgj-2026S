using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class Projectile : Area2D
{
    [Export] private float speed = 300;
    [Export] private float lifetime = 5;

    private Behaviour originatingBehaviour;
    private Module originatingModule;

    public void Setup(Behaviour originatingBehaviour, float rotationRadians)
    {
        this.originatingBehaviour = originatingBehaviour;
        this.originatingModule = originatingBehaviour.module;
        GlobalRotation = rotationRadians;

        var timer = new Timer();
        timer.WaitTime = lifetime;
        timer.OneShot = true;
        timer.Timeout += () => QueueFree();
        AddChild(timer);
        timer.Start();
    }

    public void ApplyTick(double delta)
    {
        Position += new Vector2(speed * (float)delta, 0).Rotated(GlobalRotation);
    }
}