using Godot;
using System;

public partial class Shop : Node2D
{
	[Export] private PackedScene moduleBodyScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var children = FindChild("Shop Elements Holder").GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] is ShopItemField shopItemField)
			{
				var instance = moduleBodyScene.Instantiate<ModuleBody>();
				shopItemField.AddChild(instance);
				instance.Position = Vector2.Zero;
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
}
