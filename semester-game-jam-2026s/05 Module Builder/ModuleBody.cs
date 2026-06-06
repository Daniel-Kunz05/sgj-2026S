using Godot;
using System;
using sgj.Module;
using sgj.Behaviour;

public partial class ModuleBody : Node2D, IToolTippable
{

	public const int moduleSizeX = 125;
	public const int moduleSizeY = 125;
	public static Vector2I moduleSize = new Vector2I(ModuleBody.moduleSizeX, ModuleBody.moduleSizeY);
	
	[Signal] public delegate void ModuleDeathEventHandler();

	[Signal] public delegate void OnCollisionEventHandler(Vector2 colPos);
	
	[Export] public Module? module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }
	[Export] private Sprite2D sprite;
	

    public string ToolTipText => $"[b]{module.fileName}[/b]\n{module.fileExtension.ToolTip}";


    public void SetActive(bool val)
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

	    Vector2? colPos = GetOverlapCenter(Draggable, other,
		    (RectangleShape2D)Draggable.GetChild<CollisionShape2D>(0).Shape,
		    (RectangleShape2D)other.GetChild<CollisionShape2D>(0).Shape);

		
	    if (colPos != null)
	    {
		    EmitSignalOnCollision(colPos.Value);
	    }
    }

    public static Vector2? GetOverlapCenter(
	    Area2D areaA,
	    Area2D areaB,
	    RectangleShape2D shapeA,
	    RectangleShape2D shapeB)
    {
	    Vector2 sizeA = shapeA.Size;
	    Vector2 sizeB = shapeB.Size;

	    Rect2 rectA = new Rect2(
		    areaA.GlobalPosition - sizeA / 2,
		    sizeA
	    );

	    Rect2 rectB = new Rect2(
		    areaB.GlobalPosition - sizeB / 2,
		    sizeB
	    );

	    Rect2 intersection = rectA.Intersection(rectB);

	    if (intersection.Size.X <= 0 || intersection.Size.Y <= 0)
		    return null;

	    return intersection.GetCenter();
    }



}
