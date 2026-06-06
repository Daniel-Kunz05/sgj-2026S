using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public partial class TooltipManager : Node
{
	public static TooltipManager instance;
	[Export] ToolTip toolTip;

	//[Export(PropertyHint.MultilineText)] public Godot.Collections.Array<string> texts;

	Vector2 mousePos;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		toolTip = GetNode<ToolTip>("ToolTip");
		/*foreach (var pair in modules)
		{
			Control c = GetNode(pair.Key) as Control;
			c.MouseEntered += () => TargetMouseEntered(String.Join('\n', pair.Value));
			c.MouseExited += () => TargetMouseExited();
		}
		*/
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShowToolTip(string text)
	{
		//GD.Print("Mouse entered");
		toolTip.Toggle(true);
		toolTip.setText(text);
	}

	public void HideToolTip()
	{
		//GD.Print("Mouse exited");
		toolTip.Toggle(false);
	}
}
