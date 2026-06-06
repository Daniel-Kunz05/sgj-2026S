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

	public override void _Ready()
	{
		SetupShop();
		AnimateOpenShop();
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
		var chosenFileExtension = (FileExtension)(GD.Randi() % (Enum.GetValues<FileExtension>().Length - 2)); // -2 to exclude EXE as core
		var module = new Module(chosenFileExtension, FilenameGenerator.Generate(chosenFileExtension), -1, -1);
		instance.Setup(module);
		attachTo.AddChild(instance);

		attachTo.OnItemPlaced(null, instance.GetNode<Area2D>("Area2D"));
	}

	private void AnimateOpenShop()
	{
		var originalScale = Scale;
		Scale = new Vector2(0, originalScale.Y);
		Visible = true;

		// Wait a bit before starting the animation
		var timer = new Timer();
		timer.WaitTime = 0.2f;
		timer.OneShot = true;
		AddChild(timer);
		timer.Start();
		timer.Timeout += () =>
		{

			var tween = CreateTween();
			tween.TweenProperty(this, "scale:x", originalScale.X, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

			tween.Play();
		};
	}
}
