using Godot;
using Godot.Collections;
using System;

public partial class ModuleAudioPlayer : AudioStreamPlayer2D
{
	[Export] private Array<AudioStream> explosionAudioStreams;
	[Export] private AudioStream projectileAudioStream;
	[Export] private AudioStream deathAudioStream;

	public void PlayExplosionSound()
	{
		var stream = explosionAudioStreams[(int)GD.Randi() % explosionAudioStreams.Count];
		Stream = stream;
		Play();
	}

	public void PlayProjectileSound()
	{
		Stream = projectileAudioStream;
		Play();
	}

	public void PlayDeathSound()
	{
		Stream = deathAudioStream;
		Play();
	}
}
