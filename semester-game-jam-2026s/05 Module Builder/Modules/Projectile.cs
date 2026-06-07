using Godot;
using sgj.Behaviour;
using sgj.Module;

public partial class Projectile : Area2D
{
    [Export] private float speed = 300;
    [Export] private float lifetime = 5;
    private Timer timer;

    private Behaviour originatingBehaviour;
    private Module originatingModule;

    public void Setup(Behaviour originatingBehaviour, float rotationRadians)
    {
        this.originatingBehaviour = originatingBehaviour;
        this.originatingModule = originatingBehaviour.module;
        GlobalRotation = rotationRadians;
        Monitoring = true;

        timer = new Timer();
        timer.WaitTime = lifetime;
        timer.OneShot = true;
        timer.Timeout += () =>
        {
            QueueFree();
            // Terrible way to do this but I don't wanna
            if (originatingBehaviour is MP3Behaviour mp3Behaviour)
            {
                mp3Behaviour.RemoveProjectile(this);
            }
        };
        AddChild(timer);
        timer.Start();
        AreaEntered += Check2;
    }

    public void ApplyTick(double delta)
    {
        Position += new Vector2(speed * (float)delta, 0).Rotated(GlobalRotation);
    }

    public void Check(Node2D other)
    {
        GD.Print("I was entered {}", other);
        if (other is Module mod)
        {
            mod.behaviour.TakeDamage(1);
            timer.Stop();
            timer.EmitSignal(Timer.SignalName.Timeout);
        }
    }
    public void Check2(Area2D other)
    {
        if (other == this || other is Projectile) return;
        if (other is Draggable d)
            GD.Print("I was entered a2 {}", d.OwnerParent);
    }
}