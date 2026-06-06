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
	
	[Export] public Module module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }

    public string ToolTipText => $"[b]{module.fileName}[/b]\n{module.fileExtension.ToolTip}";

    public override void _Ready()
	{
		// TODO Remove
		Setup(new Module(FileExtension.PDF, "asdf", -1, -1));
	}


    public void Setup(Module module)
	{
		this.module = module;
		module.Reparent(this);
		((IToolTippable) this).SetupToolTipConnection();
		
		

	}






}
