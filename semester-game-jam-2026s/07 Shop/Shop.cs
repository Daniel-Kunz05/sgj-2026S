using Godot;
using sgj.Behaviour;
using sgj.Module;
using sgj.NameGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Shop : Node2D
{
	[Signal] private delegate void ShopClosedEventHandler();
	
	[Export] private PackedScene moduleBodyScene;
	[Export] private Button rerollButton;

	private List<Node2D> itemsInSlots = new();

	private int rerollTries = 3;

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
		EmitSignalShopClosed();
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

	public void RerollShop()
	{
		// Check which slots are still occupied, and only reroll those
		var itemHoldersWithItems = itemsInSlots.ConvertAll((item) => item.FindParent("ShopItemField*") as ShopItemField).FindAll((field) => field != null).Select(x => x!).ToList();

		foreach (var shopItemField in itemHoldersWithItems)
		{
			// Remove the old item from the field
			shopItemField.RemoveCurrentItem();

			// Generate a new item for the field
			GenerateRandomModuleBody(shopItemField);
		}

		AnimateOpenShop();

		rerollTries--;
		rerollButton.Disabled = rerollTries <= 0;

		rerollButton.Text = $"({rerollTries}/3)";
	}

	public void ClearShop()
	{
		var itemHolders = FindChild("Shop Elements Holder").GetChildren();

		foreach (var itemHolder in itemHolders)
		{
			if (itemHolder is ShopItemField shopItemField)
			{
				shopItemField.RemoveCurrentItem();
			}
		}
		
		itemsInSlots.Clear();
	}

	public void OnItemPlacedInField(ShopItemField shopItemField, ModuleBody moduleBody)
	{
		itemsInSlots.Add(moduleBody);
		GD.Print($"Item placed in shop: {shopItemField.Name}, module body: {moduleBody.Name}");

		// Enable reroll button if there is at least 1 item in the shop
		rerollButton.Disabled = itemsInSlots.Count == 0 || rerollTries <= 0;
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

		// If shop now empty, disable reroll button
		rerollButton.Disabled = itemsInSlots.Count == 0 || rerollTries <= 0;
	}

	private void GenerateRandomModuleBody(ShopItemField attachTo)
	{
		var instance = moduleBodyScene.Instantiate<ModuleBody>();
		//var chosenFileExtension = (FileExtension)(GD.Randi() % (Enum.GetValues<FileExtension>().Length - 2)); // -2 to exclude EXE as core
		var chosenFileExtension = FileExtension.MP3;
		var module = new Module(chosenFileExtension, FilenameGenerator.Generate(chosenFileExtension), -1, -1);
		instance.Setup(module, true);
		Connect(SignalName.ShopClosed, Callable.From(instance.OnShopClosed));
		attachTo.AddChild(instance);

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
