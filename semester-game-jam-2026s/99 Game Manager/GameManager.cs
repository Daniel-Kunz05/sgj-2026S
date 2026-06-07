using Godot;
using sgj;
using System;
using sgj.Module;

public partial class GameManager : Node
{
	public static GameManager instance = null!;

	private ModuleBuilder playerModuleBuilder;
	private ModuleBuilder enemyModuleBuilder;
	private Shop shop;
	private MainCmdlineController mainCmdlineController;

	private bool isBattlePhase = false;
	private int battlePhaseNumber = 5;

	private (string name, string progress, Module[] modules) currentFighter;

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
		playerModuleBuilder = GetTree().Root.GetNode("Main/ModuleBuilder") as ModuleBuilder;
		if (playerModuleBuilder == null)
		{
			GD.PrintErr("ModuleBuilder not found");
			return;
		}
		
		enemyModuleBuilder = GetTree().Root.GetNode("Main/EnemyBuilder") as ModuleBuilder;
		if (enemyModuleBuilder == null)
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
		//shop phase
		if (isBattlePhase)
		{
			isBattlePhase = false;
			mainCmdlineController.EnqueueCommand("ls", CmdlineAction.NOP, () =>
			{
				shop.OpenAndGenerateShop();
				playerModuleBuilder.ShowBuilder();
				
			});
		}
		else //battle phase
		{
			
			shop.CloseShop();
			playerModuleBuilder.SetupShip();

			playerModuleBuilder.HideBuilder();
			BattleSequence();
			
		}
	}

	private async void BattleSequence()
	{
		// Command animation
		mainCmdlineController.EnqueueCommand("cd ..", CmdlineAction.POP_DIR, () =>
		{
			// Continue after command animation is done
			currentFighter = Database.GetFighter(mainCmdlineController.CurrentPath);
			mainCmdlineController.EnqueueCommand($"{Database.Instance.initialGamePath}/{Database.Instance.playerName}/core.exe --fight {currentFighter.name}", CmdlineAction.NOP, async void () =>
			{
				isBattlePhase = true;

				enemyModuleBuilder.ShowBuilder();
				enemyModuleBuilder.NPCOverwriteModules(currentFighter.modules);
				//TODO play enemy entry animation

				// Start battle (aim phase)
				playerModuleBuilder.SetupShip();
				await ToSignal(playerModuleBuilder, ModuleBuilder.SignalName.ShipShot);
				GD.Print("yipiii");
				// Todo wait for battle end
				//EndBattlePhase();


			});

		});
	}

	private void EndBattlePhase()
	{
		battlePhaseNumber--;
		playerModuleBuilder.ResetModules();

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
