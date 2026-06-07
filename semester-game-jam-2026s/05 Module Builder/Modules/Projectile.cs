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

    private uint opponentLayer;

    public void Setup(Behaviour originatingBehaviour, float rotationRadians)
    {
        this.originatingBehaviour = originatingBehaviour;
        this.originatingModule = originatingBehaviour.module;
        GlobalRotation = rotationRadians;
        Monitoring = true;

        opponentLayer = (uint)(originatingBehaviour.Body.Draggable.CollisionLayer == 8 ? 1 : 8);
        GD.Print("Opponent layer is {}", opponentLayer);

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
        AreaEntered += CheckCollision;
    }

    public void ApplyTick(double delta)
    {
        Position += new Vector2(speed * (float)delta, 0).Rotated(GlobalRotation);
    }

    public void CheckCollision(Area2D other)
    {        
        // Layer check
        if (other == this || other is Projectile) return;
        if (other is Draggable d && d.GetParent() is ModuleBody body)
        {
            if (d.CollisionLayer == opponentLayer)
            {
                Module mod = body.module;

                mod.behaviour.TakeDamage(1);
                timer.Stop();
                timer.EmitSignal(Timer.SignalName.Timeout);
            }
        }
    }
}