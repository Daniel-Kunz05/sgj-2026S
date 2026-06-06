using Godot;
using System;
using sgj.Behaviour;
using sgj.Module;

public partial class TestEnemybuilder : Node2D
{

    [Export] private ModuleBuilder _builder;
    private Module[] modules;
    public override void _Ready()
    {
        base._Ready();

        modules = new Module[3];
        modules[0] = new Module(FileExtension.EXE, "CORE", 1, 1);
        
        int x = 2;
        int y = 1;
        for (int i = 1; i < modules.Length; i++)
        {
            modules[i] = new Module(FileExtension.TXT, "bruh" + i, x, y + i);
        }
        
        _builder.ShowBuilder();
    }


    private int stage = 0;
    public void OnButton()
    {
        switch (stage)
        {
            case 0: 
                _builder.NPCOverwriteModules(modules);
                break;
            case 1:
                _builder.SetupShip();
                _builder.HideBuilder();
                break;
            case 2:
                ((EXEBehaviour)_builder.coreBody.module.behaviour).Shoot((float)Math.PI);
                break;
            case 3:
                _builder.ResetModules();
                break;
            case 4:
                _builder.NPCClearModules();
                stage = 0;
                break;
        }

        stage++;
    }
}
