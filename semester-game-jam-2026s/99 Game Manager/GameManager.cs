using Godot;
using sgj;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using sgj.Behaviour;
using sgj.Module;
using Vector2 = Godot.Vector2;

public partial class GameManager : Node
{

	private const string CORE_MISSING_TOOLTIP = "[color=red]YOUR VIRUS FILE HAS TO BE INCLUDED![/color]";
	private const string CONNECTION_MISSING_TOOLTIP = "[color=red]ALL FILES HAVE TO BE ADJACENT TO EACH OTHER![/color]";
	private const string ALL_GOOD_TOOLTIP = "You can start the upcoming battle here.";

	
	public static GameManager instance = null!;

	private ModuleBuilder playerModuleBuilder;
	private ModuleBuilder enemyModuleBuilder;
	private Shop shop;
	private MainCmdlineController mainCmdlineController;
	private ButtonWithToolTip battleButton;

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
		
		battleButton = GetTree().Root.GetNode("Main/Battle Button") as ButtonWithToolTip;
		if (battleButton == null)
		{
			GD.PrintErr("Battle Button not found");
			return;
		}

		battleButton.Pressed += OnBattleButtonPressed;
		battleButton.OverrideToolTip(ALL_GOOD_TOOLTIP);

		battleButton.MouseEntered += () => BattlePhaseAllowed();

		
		// So that we start correctly in the shop phase
		isBattlePhase = true;

		NextPhase();
	}

	public void OnBattleButtonPressed()
	{
		if (!isBattlePhase)
		{
			if (BattlePhaseAllowed())
			{
				NextPhase();
			}
		}
	}

	private bool BattlePhaseAllowed()
	{
		switch (playerModuleBuilder.IsBuildAccepted())
		{
			case ModuleBuilder.BuildErrorCode.CORE_MISSING:
				(battleButton).OverrideToolTip(CORE_MISSING_TOOLTIP);
				TooltipManager.instance.toolTip.setText(CORE_MISSING_TOOLTIP);
				return false;
			case ModuleBuilder.BuildErrorCode.NOT_CONNECTED:
				battleButton.OverrideToolTip(CONNECTION_MISSING_TOOLTIP);
				TooltipManager.instance.toolTip.setText(CONNECTION_MISSING_TOOLTIP);
				return false;
			default:
				battleButton.OverrideToolTip(ALL_GOOD_TOOLTIP);
				TooltipManager.instance.toolTip.setText(ALL_GOOD_TOOLTIP);
				return true;
		}
	}

	public void NextPhase()
	{
		//shop phase
		if (isBattlePhase)
		{
			gameDecided = false;
			isBattlePhase = false;
			mainCmdlineController.EnqueueCommand("ls", CmdlineAction.NOP, async () =>
			{
				currentFighter = Database.GetFighter(battlePhaseNumber-1);
				
				playerModuleBuilder.EnableDrag(false);
				await playerModuleBuilder.EntryAnimationAllModules();
				await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
				enemyModuleBuilder.NPCOverwriteModules(currentFighter.modules);
				await enemyModuleBuilder.EntryAnimationAllModules();
				await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
				playerModuleBuilder.EnableDrag(true);
				shop.OpenAndGenerateShop();
				await playerModuleBuilder.ShowBuilder();
				ShowBattleButton();

			});
		}
		else //battle phase
		{
			BattleSequence();
			
		}
	}

	private async void BattleSequence()
	{
		// Command animation
		mainCmdlineController.EnqueueCommand("cd ..", CmdlineAction.POP_DIR, async() =>
		{
			HideBattleButton();
			shop.CloseShop();
			playerModuleBuilder.HideBuilder();
			
			await ToSignal(GetTree().CreateTimer(.5), SceneTreeTimer.SignalName.Timeout);
			
			// Continue after command animation is done
			mainCmdlineController.EnqueueCommand($"{Database.Instance.initialGamePath}/{Database.Instance.playerName}/core.exe --fight {currentFighter.name}", CmdlineAction.NOP, async void () =>
			{
				isBattlePhase = true;
				
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
				//TODO play enemy entry animation
				enemyModuleBuilder.SetupShip();

				EXEBehaviour enemyEXE = (EXEBehaviour)enemyModuleBuilder.coreBody.module.behaviour;
				
				// Start battle (aim phase)
				playerModuleBuilder.SetupShip();

				float angle = (float)(Random.Shared.Next(30, 70) * 0.01f * Math.PI);
				enemyEXE.SetArrowRotation(angle);
				enemyEXE.ShowArrow();
				await ToSignal(playerModuleBuilder, ModuleBuilder.SignalName.ShipShot);
				enemyEXE.HideArrow();
				enemyEXE.Shoot(angle); 
				
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
		if (gameDecided) return;
		enemyModuleBuilder.coreBody.module.OnModuleDeath -= EndBattlePhase;
		playerModuleBuilder.coreBody.module.OnModuleDeath -= EndBattlePhase;

		if (module == playerModuleBuilder.coreBody.module)
		{
			GameOver();
		}
		else
		{
			battlePhaseNumber--;
			playerModuleBuilder.HideAllModules();
			playerModuleBuilder.CallDeferred("ResetModules");
			enemyModuleBuilder.CallDeferred("ResetModules");
			enemyModuleBuilder.CallDeferred("NPCClearModules");

			if (battlePhaseNumber == 0)
			{
				GameWon();
			}

			NextPhase();
		}
		
	}
	
	private async Task ShowBattleButton()
	{
		battleButton.Visible = true;
		battleButton.Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(battleButton, "modulate", new Color(1,1,1,1), 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.Play();
		await ToSignal(tween, Tween.SignalName.Finished);
		battleButton.Disabled = false;



	}

	private async Task HideBattleButton()
	{
		battleButton.Disabled = true;
		battleButton.Modulate = new Color(1, 1, 1, 1);
		var tween = CreateTween();
		tween.TweenProperty(battleButton, "modulate", new Color(1,1,1,0), 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.Play();
		await ToSignal(tween, Tween.SignalName.Finished);
		battleButton.Visible = true;
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	private bool gameDecided;
	private async void GameOver()
	{
		if (gameDecided) return;
		Vector2 corePos = playerModuleBuilder.coreBody.GlobalPosition;
		GetTree().Paused = true;
		EXEBehaviour behaviour = (EXEBehaviour)playerModuleBuilder.coreBody.module.behaviour;
		IExplodable asExplodable = behaviour;

		enemyModuleBuilder.coreBody.Visible = false;
		foreach (ModuleBody body in playerModuleBuilder.UsedModules.Values)
		{
			body.Visible = false;
		}

		playerModuleBuilder.coreBody.Visible = true;
		await playerModuleBuilder.coreBody.Blink(5);
		for (int i = 0; i < playerModuleBuilder.UsedModules.Count; i++)
		{
			Vector2 pos =  corePos + Vector2.FromAngle(Random.Shared.Next()) * Random.Shared.Next(200);
			asExplodable.SpawnExplosion(playerModuleBuilder, pos, playerModuleBuilder.UsedModules.GetValueAtIndex(i).module.fileExtension);
			await ToSignal(GetTree().CreateTimer(0.2, true), SceneTreeTimer.SignalName.Timeout);
		}

		for (int i = 0; i < 3; i++)
		{
			Vector2 pos =  corePos + Vector2.FromAngle(Random.Shared.Next()) * Random.Shared.Next(200);
			asExplodable.SpawnExplosion(playerModuleBuilder, pos, (FileExtension)Random.Shared.Next(0, (int)FileExtension.EXE));
			await ToSignal(GetTree().CreateTimer(0.2, true), SceneTreeTimer.SignalName.Timeout);
		}
		
		await ToSignal(GetTree().CreateTimer(playerModuleBuilder.UsedModules.Count * 0.2f, true), SceneTreeTimer.SignalName.Timeout);
		GetTree().Paused = false;
		ResetForNewGame();    
		GetTree().ChangeSceneToFile("res://you_lose.tscn");

	}

	private void GameWon()
	{
		if (gameDecided) return;    
		GD.Print("You won!"); 
		ResetForNewGame();    
		GetTree().ChangeSceneToFile("res://you_win.tscn");
	}

	private void ResetForNewGame()
	{
		battlePhaseNumber = 5;
		gameDecided = false;
	}
}
