using Godot;
using sgj.Behaviour;
using sgj.Module;
using sgj.NameGeneration;
using System;
using System.Collections.Generic;

public partial class Shop : Node2D
{
	[Export] private PackedScene moduleBodyScene;

	private HashSet<Module> originalDroppedItems = new();

	private List<Node2D> itemsInSlots = new();

	public void OpenAndGenerateShop()
	{
		ClearShop();
		SetupShop();
		AnimateOpenShop();
	}

	public void CloseShop()
	{
		AnimateCloseShop();
		ClearShop();
	}

	public void SetupShop()
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

	public void ClearShop()
	{
		GD.Print($"Clearing shop, items in slots: {itemsInSlots.Count}, original dropped items: {originalDroppedItems.Count}");
		foreach (var item in itemsInSlots)
		{
			item.QueueFree();
		}
		itemsInSlots.Clear();
		originalDroppedItems.Clear();
	}

	public void OnItemPlacedInField(ShopItemField shopItemField, ModuleBody moduleBody)
	{
		itemsInSlots.Add(moduleBody);
		GD.Print($"Item placed in shop: {shopItemField.Name}, module body: {moduleBody.Name}");
	}

	public void OnItemRemovedFromField(ShopItemField shopItemField, ModuleBody moduleBody)
	{
		if (itemsInSlots.Remove(moduleBody))
		{
			GD.Print($"Item removed from shop: {shopItemField.Name}, module body: {moduleBody.Name}");
		}
		else
		{
			GD.Print($"Attempted to remove item that was not in shop: {shopItemField.Name}, module body: {moduleBody.Name}");
		}
	}

	private void GenerateRandomModuleBody(ShopItemField attachTo)
	{
		var instance = moduleBodyScene.Instantiate<ModuleBody>();
		var chosenFileExtension = (FileExtension)(GD.Randi() % (Enum.GetValues<FileExtension>().Length - 2)); // -2 to exclude EXE as core
		var module = new Module(chosenFileExtension, FilenameGenerator.Generate(chosenFileExtension), -1, -1);
		originalDroppedItems.Add(module);
		instance.Setup(module);
		attachTo.AddChild(instance);

		originalDroppedItems.Add(module);
		attachTo.OnItemPlaced(null, instance.GetNode<Area2D>("Area2D"));
	}

	private void AnimateOpenShop()
	{
		Scale = new Vector2(0, 1);
		Visible = true;

		var tween = CreateTween();
		tween.TweenProperty(this, "scale:x", 1, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

		tween.Play();

	}

	private void AnimateCloseShop()
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "scale:x", 0, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
		tween.TweenCallback(new Callable(this, nameof(Hide)));
		tween.Play();
	}
}
