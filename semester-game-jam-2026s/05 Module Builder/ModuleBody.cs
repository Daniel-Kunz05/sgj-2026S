using Godot;
using System;
using System.Threading.Tasks;
using sgj.Module;
using sgj.Behaviour;

public partial class ModuleBody : Node2D, IToolTippable
{

	public const int moduleSizeX = 105;
	public const int moduleSizeY = 105;
	public static Vector2I moduleSize = new Vector2I(ModuleBody.moduleSizeX, ModuleBody.moduleSizeY);
	public CollisionShape2D physicsShape;
	
	[Signal] public delegate void OnCollisionEventHandler(Vector2 colPos);
	
	[Export] public Module? module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }
	public bool locked;
	[Export] private Sprite2D sprite;

	[Export] private int debug;
	
	[Export] public ModuleAudioPlayer audioPlayer;

    public string ToolTipText => $"[b]{module.fileName}[/b]\n{module.fileExtension.ToolTip}";


    public void BattleMode(bool val)
    {
	    Draggable.AllowDragging = !val;
		Draggable.SetDeferred(nameof(Draggable.Monitorable), val);
		Draggable.SetDeferred(nameof(Draggable.Monitoring), val);
    }

    public override void _Ready()
    {
	    base._Ready();
	    Material = (Material)Material.Duplicate();
	    BattleMode(false);

		debug = (int)Draggable.CollisionLayer;
    }

    public void Setup(Module module, bool isPlayer)
    {
	    this.module = module;
	    ((IToolTippable) this).SetupToolTipConnection();
	    if (module.GetParent() != null) module.Reparent(this);
	    else AddChild(module);

	    sprite.Texture = ResourceLoader.Load<Texture2D>(module.fileExtension.IconPath);
		module.audioPlayer = audioPlayer;
	    module.OnModuleHit += OnHurt;

	    if (isPlayer)
	    {
		    Draggable.CollisionLayer = (1 << 0);
		    Draggable.CollisionMask = (1 << 3);
	    }
	    else
	    {
		    Draggable.CollisionLayer = (1 << 3);
		    Draggable.CollisionMask = (1 << 0);
		    
	    }

    }
    public void OnAreaEntered(Area2D other)
    {
		
	    if (other.GetParent() is ModuleBody body)
	    {
		    module.EmitSignalOnModuleHitExtern(module, body.module);
	    }
    }

    public void OnShopClosed()
    {
	    if (!locked)
	    {
		    QueueFree();
	    }
    }

    public void KnockOut()
    {
	    SetDeferred(nameof(Node.Owner), GetTree().Root);
	    GlobalPosition = Vector2.One * -1000;
	    physicsShape.SetDeferred(nameof(Node.Owner), GetTree().Root);
	    physicsShape.GlobalPosition = Vector2.One * -1000;
	    BattleMode(false);
    }

    private bool isBlinking;
    private async void OnHurt(Module _, Module __)
    {
	    if (isBlinking) return;
	    Blink(3);
    }

    public async Task Blink(int amount)
    {
	    isBlinking = true;
	    for (int i = 0; i < amount; i++)
	    {
		    Visible = false;
		    await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

		    Visible = true;
		    await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
	    }

	    isBlinking = false;
    }

    public async Task EntryAnimation()
    {
	    Visible = true;
	    if (Material is ShaderMaterial shader)
	    {
		    Vector2 goal = Scale;
		    Scale = Scale * 0.3f;
		    GD.Print("noo");
		    shader.SetShaderParameter("pixel_size", 16);
		    var tween1 = CreateTween();
		    tween1.TweenProperty(shader, "shader_parameter/pixel_size", 4, 2).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.OutIn);
		    tween1.Play();
		    
		    var tween2 = CreateTween();
		    tween2.TweenProperty(this, "scale", goal, 2).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
		    tween2.Play();
		    await ToSignal(tween2, Tween.SignalName.Finished);
	    }
    }
    
}
