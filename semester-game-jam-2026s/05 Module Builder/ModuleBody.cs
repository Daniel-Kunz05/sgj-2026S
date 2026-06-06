using Godot;
using System;
using sgj.Module;
using sgj.Behaviour;

public partial class ModuleBody : Node2D, IToolTippable
{

	public const int moduleSizeX = 82;
	public const int moduleSizeY = 82;
	public static Vector2I moduleSize = new Vector2I(ModuleBody.moduleSizeX, ModuleBody.moduleSizeY);
	
	[Signal] public delegate void ModuleDeathEventHandler();

	[Signal] public delegate void OnCollisionEventHandler(Vector2 colPos);
	
	[Export] public Module? module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }
	[Export] private Sprite2D sprite;
	

    public string ToolTipText => $"[b]{module.fileName}[/b]\n{module.fileExtension.ToolTip}";


    public void BattleMode(bool val)
    {
	    Draggable.AllowDragging = !val;
	    Draggable.Monitorable = val;
	    Draggable.Monitoring = val;
    }    
    
    public void Setup(Module module)
    {
	    this.module = module;
	    ((IToolTippable) this).SetupToolTipConnection();
	    if (module.GetParent() != null) module.Reparent(this);
	    else AddChild(module);

	    sprite.Texture = ResourceLoader.Load<Texture2D>(module.fileExtension.IconPath);

    }
    public void OnAreaEntered(Area2D other)
    {
		
	    if (other.GetParent() is ModuleBody body)
	    {
		    module.EmitSignalOnModuleHitExtern(module, body.module);
	    }
    }


}
