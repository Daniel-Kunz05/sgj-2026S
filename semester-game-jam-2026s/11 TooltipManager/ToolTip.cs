using Godot;
using System;

public partial class ToolTip : Control
{
	static Vector2 OFFSET = Vector2.One * 20f;
	Tween opacityTween = null;
	[Export] RichTextLabel text;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if(Visible && @event is InputEventMouseMotion)
		{
			GlobalPosition = GetGlobalMousePosition() + OFFSET;
		}
    }

	public void Toggle(bool on)
	{
		if (on)
		{
			Show();
		} else
		{
			Hide();
		}
	}


	//fading
	public Tween TweenOpacity(Color c)
	{
		opacityTween?.Kill();
		opacityTween = GetTree().CreateTween();
		opacityTween.TweenProperty(this, "modulate", c, 0.3);
		return opacityTween;
	}

	public void setText(string newText)
	{
		text.Text = newText;
	}

}
