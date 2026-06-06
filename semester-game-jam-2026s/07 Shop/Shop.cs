using Godot;
using sgj.Behaviour;
using sgj.Module;
using sgj.NameGeneration;
using System;
using System.Collections.Generic;

public partial class Shop : Node2D
{
	[Export] private PackedScene moduleBodyScene;

	private HashSet<ModuleBody> originalDroppedItems = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var children = FindChild("Shop Elements Holder").GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] is ShopItemField shopItemField)
			{
				GenerateRandomModuleBody(shopItemField);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnItemPlacedInField(ShopItemField shopItemField, ModuleBody moduleBody)
	{
		GD.Print($"Item placed in shop: {shopItemField.Name}, module body: {moduleBody.Name}");
	}

	public void OnItemRemovedFromField(ShopItemField shopItemField, ModuleBody moduleBody)
	{
		GD.Print($"Item removed from shop: {shopItemField.Name}, module body: {moduleBody.Name}");
	}

	private void GenerateRandomModuleBody(ShopItemField attachTo)
	{
		var instance = moduleBodyScene.Instantiate<ModuleBody>();
		var chosenFileExtension = (FileExtension) (GD.Randi() % (Enum.GetValues<FileExtension>().Length - 2)); // -2 to exclude EXE as core
		var module = new Module(chosenFileExtension, FilenameGenerator.Generate(chosenFileExtension), -1, -1);
		instance.Setup(module);
		attachTo.AddChild(instance);

		attachTo.OnItemPlaced(null, instance.GetNode<Area2D>("Area2D"));
	}
}
