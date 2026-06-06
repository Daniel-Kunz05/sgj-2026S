using Godot;
using System;

public partial class ShopItemField : Node2D
{
	[Signal] public delegate void ItemRemovedEventHandler(ShopItemField shopItemField, ModuleBody moduleBody);
	[Signal] public delegate void ItemPlacedEventHandler(ShopItemField shopItemField, ModuleBody moduleBody);

	[Export] private ModuleBody? currentStoredModuleBody;

	public ModuleBody? CurrentStoredModuleBody => currentStoredModuleBody;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnItemPlaced(Area2D _, Area2D draggable)
	{
		// Check if parent is module body
		if (draggable.GetParent() is ModuleBody moduleBody)
		{
			currentStoredModuleBody = moduleBody;
		}
		else
		{
			GD.PrintErr("Draggable is not a ModuleBody");
			return;
		}

		(draggable as Draggable)?.DragStart += OnItemRemoved;

		// Move to the shop item field and reparent to the shop item field
		moduleBody.GetParent().RemoveChild(moduleBody);
		AddChild(moduleBody);
		moduleBody.Position = Vector2.Zero;

		EmitSignalItemPlaced(this, moduleBody);
	}

	private void OnItemRemoved(Draggable originalDraggable)
	{
		if (currentStoredModuleBody == null)
		{
			GD.PrintErr("No module body stored in shop item field");
			return;
		}

		var moduleBody = currentStoredModuleBody;

		currentStoredModuleBody = null;

		// Unsubscribe from the event
		originalDraggable.DragStart -= OnItemRemoved;

		EmitSignalItemRemoved(this, moduleBody);
	}


}
