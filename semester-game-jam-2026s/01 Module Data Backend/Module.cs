using Godot;
using sgj.Behaviour;
using System;
namespace sgj.Module;

[GlobalClass]
public partial class Module : Node2D
{
    [Signal] public delegate void OnModuleHitEventHandler(Module m1, Module m2);
    [Signal] public delegate void TickEventHandler(double delta);
    [Signal] public delegate void OnModuleDeathEventHandler(Module m);
    [Export] public FileExtension fileExtension;
    [Export] public string fileName;
    [Export] public int x;
    [Export] public int y;
    public Behaviour.Behaviour behaviour = null!;

    public Module(FileExtension fileExtension, string fileName, int x, int y)
    {
        this.fileExtension = fileExtension;
        this.fileName = fileName;
        this.x = x;
        this.y = y;
    }

    public override void _Ready()
    {
        //
        behaviour = fileExtension.Constructor(this);
        AddChild(behaviour);
        OnModuleHit += behaviour.OnModuleHit;
        Tick += behaviour.Tick;
        OnModuleDeath += behaviour.OnModuleDeath;
    }

    public Module() : this(FileExtension.TXT, "", 0, 0) { }
}
