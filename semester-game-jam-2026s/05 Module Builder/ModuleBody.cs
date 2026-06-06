using Godot;
using System;
using sgj.Module;

public partial class ModuleBody : Node2D
{

	public const int moduleSizeX = 125;
	public const int moduleSizeY = 125;
	public static Vector2I moduleSize = new Vector2I(ModuleBody.moduleSizeX, ModuleBody.moduleSizeY);
	
	[Signal] public delegate void ModuleDeathEventHandler();
	
	[Export] public Module module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }
	

	public void Setup(Module module)
	{
		this.module = module;
		module.Reparent(this);
		
		

	}






}
