using Godot;
using sgj;
using System;
using System.Linq;
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
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
				//TODO play enemy entry animation
				enemyModuleBuilder.SetupShip();
				enemyModuleBuilder.HideBuilder();

				// Start battle (aim phase)
				playerModuleBuilder.SetupShip();
				await ToSignal(playerModuleBuilder, ModuleBuilder.SignalName.ShipShot);

				if (enemyModuleBuilder.coreBody.module.behaviour is EXEBehaviour enemyEXE)
				{
					enemyEXE.Shoot(Random.Shared.Next((int)(Math.PI / 2f) * 100, (int)(Math.PI * 3f /2f)*100)/100f);
				}
				
				Database.Instance.modules = [.. playerModuleBuilder.UsedModules.Select((m) => m.Value.module!)];
				Database.AddFighter(Database.Instance.playerName, Database.Instance.gamePath, Database.Instance.modules);
				
				// Todo wait for battle end
				enemyModuleBuilder.coreBody.module.OnModuleDeath += EndBattlePhase;
				playerModuleBuilder.coreBody.module.OnModuleDeath += EndBattlePhase;
				//EndBattlePhase();


			});

		});
	}

	private void EndBattlePhase(Module module)
	{
		enemyModuleBuilder.coreBody.module.OnModuleDeath -= EndBattlePhase;
		playerModuleBuilder.coreBody.module.OnModuleDeath -= EndBattlePhase;

		if (module == playerModuleBuilder.coreBody.module)
		{
			GameOver();
		}
		else
		{
			battlePhaseNumber--;
			playerModuleBuilder.CallDeferred("ResetModules");
			enemyModuleBuilder.CallDeferred("ResetModules");
			enemyModuleBuilder.CallDeferred("NPCClearModules");

			if (battlePhaseNumber == 0)
			{
				// End game, TODO
				GD.Print("You won!"); 
				GetTree().ChangeSceneToFile("res://you_win.tscn");
				return;
			}

			NextPhase();
		}
		
	}

	private void GameOver()
	{
		GetTree().ChangeSceneToFile("res://you_lose.tscn");

	}
}
