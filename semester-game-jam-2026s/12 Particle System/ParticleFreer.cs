using Godot;
using System;

public partial class ParticleFreer : GpuParticles2D
{
	ulong timeCreated;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeCreated = Time.GetTicksMsec();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Time.GetTicksMsec() - timeCreated > 1000)
			QueueFree();
	}
}
