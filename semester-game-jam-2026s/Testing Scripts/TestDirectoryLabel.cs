using Godot;
using System;

[GlobalClass]
public partial class TestDirectoryLabel : Node
{
	private double timer = 1;
	[Export] private Label label = null!;
	[Export] private Label label2 = null!;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Change();
	}

	public void Change()
	{
		label.Text = sgj.NameGeneration.PathGenerator.Generate();
		label2.Text = sgj.NameGeneration.FilenameGenerator.Generate(sgj.Behaviour.FileExtension.TXT);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer -= delta;
		if (timer <= 0)
		{
			timer = 1;
			Change();
		}
	}
}
