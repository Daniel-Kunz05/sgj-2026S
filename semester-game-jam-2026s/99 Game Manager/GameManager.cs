using Godot;
using sgj;
using System;

public partial class GameManager : Node
{
	public static GameManager instance = null!;

	private ModuleBuilder moduleBuilder;
	private Shop shop;
	private MainCmdlineController mainCmdlineController;

	private bool isBattlePhase = false;
	private int battlePhaseNumber = 5;

	public override void _Ready()
	{
		if (instance != null)
		{
			QueueFree();
			return;
		}

		instance = this;
	}

	public void StartGame()
	{
		GD.Print("Game Started");
		// find Module Builder
		moduleBuilder = GetTree().Root.GetNode("Main/ModuleBuilder") as ModuleBuilder;
		if (moduleBuilder == null)
		{
			GD.PrintErr("ModuleBuilder not found");
			return;
		}

		// find Shop
		shop = GetTree().Root.GetNode("Main/Shop") as Shop;
		if (shop == null)
		{
			GD.PrintErr("Shop not found");
			return;
		}

		// find MainCmdlineController
		mainCmdlineController = GetTree().Root.GetNode("Main/MainCmdlineController") as MainCmdlineController;
		if (mainCmdlineController == null)
		{
			GD.PrintErr("MainCmdlineController not found");
			return;
		}

		// So that we start correctly in the shop phase
		isBattlePhase = true;

		NextPhase();
	}

	public void NextPhase()
	{
		if (isBattlePhase)
		{
			isBattlePhase = false;
			mainCmdlineController.EnqueueCommand("ls", CmdlineAction.NOP, () =>
			{
				shop.OpenAndGenerateShop();
				moduleBuilder.ShowBuilder();
			});
		}
		else
		{
			shop.CloseShop();
			moduleBuilder.SetupShip();

			moduleBuilder.HideBuilder();

			// Command animation
			mainCmdlineController.EnqueueCommand("cd ..", CmdlineAction.POP_DIR, () =>
			{
				// Continue after command animation is done

				mainCmdlineController.EnqueueCommand($"{Database.Instance.gamePath}/{Database.Instance.playerName}/core.exe --fight TODO", CmdlineAction.NOP, () =>
				{
					isBattlePhase = true;

					// Start battle (aim phase)
					// moduleBuilder.SetupShip();

					// Todo wait for battle end
					//EndBattlePhase();


				});

			});
		}
	}

	private void EndBattlePhase()
	{
		battlePhaseNumber--;
		moduleBuilder.ResetModules();

		if (battlePhaseNumber == 0)
		{
			// End game, TODO
			GD.Print("You won!");
			Database.AddFighter(Database.Instance.playerName, "/", Database.Instance.modules);
			GetTree().ChangeSceneToFile("res://you_win.tscn");
			return;
		}

		NextPhase();
	}
}
