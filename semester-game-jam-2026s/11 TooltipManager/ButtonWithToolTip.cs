using Godot;
using System;

[GlobalClass]
public partial class ButtonWithToolTip : Button, IToolTippable
{
	[Export(PropertyHint.MultilineText)] private string toolTipText;
    public string ToolTipText => toolTipText;

    public void OverrideToolTip(string msg)
    {
	    toolTipText = msg;
    }

    public override void _Ready()
    {
        base._Ready();
		((IToolTippable)this).SetupToolTipConnection();
    }
}
