using Godot;
using System;

[GlobalClass]
public partial class TestDirectoryLabel : Node
{
	private double timer = 1;
	private double animPerc = 0;
	private string currentPath = null!;
	private string currentFname = null!;
	private string username = null!;
	private string command = "./virus.exe";
	[Export] private RichTextLabel label = null!;
	[Export] private Label label2 = null!;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Change();
	}

	public void Change()
	{
		(username, currentPath) = sgj.NameGeneration.PathGenerator.Generate();
		currentFname = sgj.NameGeneration.FilenameGenerator.Generate(sgj.Behaviour.FileExtension.TXT);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer -= delta;
		animPerc += delta;
		if (timer <= 0)
		{
			timer = 1;
			animPerc = 0;
			Change();
		}
		string incomplete = command.Substring(0, (int)(command.Length * double.Clamp(1.5 * animPerc, 0, 1)));
		label.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{username}@PC[/color]:[color=#11d116]{currentPath}[/color]$ {incomplete}[/font]";
		label2.Text = currentFname.Substring(0, (int)(currentFname.Length * double.Clamp(1.5 * animPerc, 0, 1)));
	}
}
