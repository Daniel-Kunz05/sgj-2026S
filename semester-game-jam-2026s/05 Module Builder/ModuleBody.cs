using Godot;
using System;
using sgj.Module;
using sgj.Behaviour;

public partial class ModuleBody : Node2D, IToolTippable
{

	public const int moduleSizeX = 82;
	public const int moduleSizeY = 82;
	public static Vector2I moduleSize = new Vector2I(ModuleBody.moduleSizeX, ModuleBody.moduleSizeY);
	
	[Signal] public delegate void OnCollisionEventHandler(Vector2 colPos);
	
	[Export] public Module? module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }
	public bool locked;
	[Export] private Sprite2D sprite;
	

    public string ToolTipText => $"[b]{module.fileName}[/b]\n{module.fileExtension.ToolTip}";


    public void BattleMode(bool val)
    {
	    Draggable.AllowDragging = !val;
	    Draggable.Monitorable = val;
	    Draggable.Monitoring = val;
    }

    public override void _Ready()
    {
	    base._Ready();
	    BattleMode(false);
    }

    public void Setup(Module module, bool isPlayer)
    {
	    this.module = module;
	    ((IToolTippable) this).SetupToolTipConnection();
	    if (module.GetParent() != null) module.Reparent(this);
	    else AddChild(module);

	    sprite.Texture = ResourceLoader.Load<Texture2D>(module.fileExtension.IconPath);

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


}
