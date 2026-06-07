using Godot;
using Godot.Collections;
using System;

public partial class ModuleAudioPlayer : AudioStreamPlayer2D
{
	[Export] private Array<AudioStream> explosionAudioStreams;
	[Export] private AudioStream projectileAudioStream;
	[Export] private AudioStream deathAudioStream;

	public void PlayExplosionSound(int index)
	{
		RandomPitch();
		var stream = explosionAudioStreams[index];
		Stream = stream;
		Play();
	}

	public void PlayProjectileSound()
	{
		RandomPitch();
		Stream = projectileAudioStream;
		Play();
	}

	public void PlayDeathSound()
	{
		RandomPitch();
		Stream = deathAudioStream;
		Play();
	}

	private void RandomPitch()
	{
		PitchScale = 1 + Random.Shared.Next(-3, 3) * 0.1f;
	}
}
