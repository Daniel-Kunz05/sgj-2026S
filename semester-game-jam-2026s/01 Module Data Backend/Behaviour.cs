using Godot;
using System;
namespace sgj.Behaviour;

[GlobalClass]
public abstract partial class Behaviour : Node
{
	public static Behaviour Instantiate(FileExtension type) => type.constructor();

	public abstract void OnModuleHit(Module m1, Module m2);
	public abstract void Tick();
	public abstract void ModuleDeath(Module cause);
}
