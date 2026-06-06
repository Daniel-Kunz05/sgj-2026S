using Godot;
using System;

public partial class DragAndDropManager : Node2D
{
	private bool tryingToDrag;
	private bool isDragging;
	private Draggable? currentDragged;
	
	
	public override void _Process(double delta)
	{
		if (currentDragged != null)
		{
			currentDragged.MoveTo(GetGlobalMousePosition());
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventKey)
		{
			if (eventKey.Pressed && eventKey.ButtonIndex == MouseButton.Left)
			{
				OnMouseClick();
			} else if (!eventKey.Pressed && eventKey.ButtonIndex == MouseButton.Left)
			{
				OnMouseRelease();
			}
		}
	}
	

	private void OnMouseClick()
	{
		Draggable? hit = RaycastDraggable(GetGlobalMousePosition());

		if (hit != null)
		{
			DetermineDrag(hit);
		}
		
	}

	private void OnMouseRelease()
	{
		if (isDragging)
		{
			Drop();
		}
		else
		{
			tryingToDrag = false;
		}
	}

	private async void DetermineDrag(Draggable hit)
	{
		tryingToDrag = true;
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		if (tryingToDrag)
		{
			isDragging = true;
			currentDragged = hit;
			currentDragged.Monitorable = false;
			currentDragged.OwnerParent.Reparent(this);
		}
		else
		{
			hit.OnMouseClick();
		}
	}

	private void Drop()
	{
		isDragging = false;
		tryingToDrag = false;

		if (currentDragged != null)
		{
			DropReceiver? receiver = RaycastDropReceiver(GetGlobalMousePosition());
			if (receiver != null)
			{
				receiver.Receive(currentDragged);
			}
			else
			{
				currentDragged.Decline();
			}
			currentDragged.Monitorable = true;
			currentDragged = null;
		}
		
	}

	private Draggable? RaycastDraggable(Vector2 worldPos)
	{
		Vector2 mousePos = GetGlobalMousePosition();

		var query = new PhysicsPointQueryParameters2D
		{
			Position = mousePos,
			CollideWithAreas = true,
			CollideWithBodies = true
		};

		var results = GetWorld2D()
			.DirectSpaceState
			.IntersectPoint(query);

		foreach (var hit in results)
		{
			Node collider = hit["collider"].As<Node>();

			if (collider is Draggable draggable)
			{
				return draggable;
			}
		}

		return null;
	}
	
	private DropReceiver? RaycastDropReceiver(Vector2 worldPos)
	{
		Vector2 mousePos = GetGlobalMousePosition();

		var query = new PhysicsPointQueryParameters2D
		{
			Position = mousePos,
			CollideWithAreas = true,
			CollideWithBodies = true
		};

		var results = GetWorld2D()
			.DirectSpaceState
			.IntersectPoint(query);

		foreach (var hit in results)
		{
			Node collider = hit["collider"].As<Node>();

			if (collider is DropReceiver dropReceiver)
			{
				return dropReceiver;
			}
		}

		return null;
	}
	
	
}
