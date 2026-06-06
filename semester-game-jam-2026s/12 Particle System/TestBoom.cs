using Godot;
using System;

public partial class TestBoom : Node2D, IExplodable
{
	bool done = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("I exist");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!done)
		{
			done = true;
			BoomTest();
		}
	}

	private void BoomTest()
	{
		GD.Print(GlobalPosition);
		((IExplodable) this).SpawnExplosion(this.GetNode<Node2D>("%TestBoom"), this.GlobalPosition);
		GD.Print("Boom");
	}
}
