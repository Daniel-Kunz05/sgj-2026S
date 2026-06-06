using Godot;
using System;
using sgj.Module;

public partial class ModuleBody : Node2D
{

	[Signal] public delegate void ModuleDeathEventHandler();
	
	public Module module { get; private set; }
	[Export] public Draggable Draggable { get; private set; }


	public void Setup(Module module)
	{




	}






}
