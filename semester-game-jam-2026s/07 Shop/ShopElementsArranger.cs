using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopElementsArranger : Node2D
{
	[Export] private int elementsPerRow = 4;
	[Export] private Vector2 shopWidthHeight = new Vector2(300, 300);
	[Export] private Vector2 elementSize = new Vector2(118, 118);

	private List<ShopItemField> shopElements = new List<ShopItemField>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LinkItems();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void LinkItems()
	{
		CalcShopElementsPostions();
		shopElements = GetChildren().OfType<ShopItemField>().ToList();
	}

	private void CalcShopElementsPostions()
	{
		var shopNodes = GetChildren().OfType<Node2D>().ToList();

		var initialPosition = elementSize / 2 - shopWidthHeight / 2;
		for (int i = 0; i < shopNodes.Count; i++)
		{
			int row = i / elementsPerRow;
			int column = i % elementsPerRow;

			float xPosition = initialPosition.X + column * (shopWidthHeight.X / elementsPerRow);
			float yPosition = initialPosition.Y + (float)(row * (shopWidthHeight.Y / Math.Ceiling((float)shopNodes.Count / elementsPerRow)));

			shopNodes[i].Position = new Vector2(xPosition, yPosition);
		}
	}
}
