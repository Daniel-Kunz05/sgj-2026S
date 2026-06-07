using Godot;
using System;
using sgj.Module;
namespace sgj.Behaviour;

[GlobalClass]
public abstract partial class Behaviour(Module.Module module) : Node
{
	public readonly Module.Module module = module;
	public ModuleBody? Body => GetParent().GetParent() as ModuleBody;
	public static Behaviour Instantiate(FileExtension type, Module.Module module) => type.Constructor(module);

	public abstract void OnModuleHit(Module.Module self, Module.Module other);
	public abstract void Tick(double delta);
	public abstract void OnModuleDeath(Module.Module cause);
	public abstract void Reset();
	public abstract void TakeDamage(int amount);
}
