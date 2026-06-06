using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public partial class TooltipManager : Node
{
	public TooltipManager instance;
	[Export] ToolTip toolTip;
	[Export] Godot.Collections.Dictionary<NodePath, string> modules;

	Vector2 mousePos;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		toolTip = GetNode<ToolTip>("ToolTip");
		foreach (var pair in modules)
		{
			Control c = GetNode(pair.Key) as Control;
			c.MouseEntered += () => TargetMouseEntered(pair.Value);
			c.MouseExited += () => TargetMouseExited();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void TargetMouseEntered(string text)
	{
		GD.Print("Mouse entered");
		toolTip.Toggle(true);
		toolTip.setText(text);
	}

	private void TargetMouseExited()
	{
		GD.Print("Mouse exited");
		toolTip.Toggle(false);
	}
}
