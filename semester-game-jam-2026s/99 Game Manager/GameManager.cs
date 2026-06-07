using Godot;
using sgj;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
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

	public async void NextPhase()
	{
		//shop phase
		if (isBattlePhase)
		{
			isBattlePhase = false;
			mainCmdlineController.EnqueueCommand("ls", CmdlineAction.NOP, async () =>
			{
				await playerModuleBuilder.EntryAnimationAllModules();
				await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
				shop.OpenAndGenerateShop();
				await playerModuleBuilder.ShowBuilder();
				ShowBattleButton();
				// Continue after command animation is done
				currentFighter = Database.GetFighter(mainCmdlineController.CurrentPath);
				enemyModuleBuilder.NPCOverwriteModules(currentFighter.modules);

				await enemyModuleBuilder.EntryAnimationAllModules();
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
				//TODO play enemy entry animation
				enemyModuleBuilder.SetupShip();

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
			
			
			mainCmdlineController.EnqueueCommand($"{Database.Instance.initialGamePath}/{Database.Instance.playerName}/core.exe --fight {currentFighter.name}", CmdlineAction.NOP, async void () =>
			{
				isBattlePhase = true;
				


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

	private async void GameOver()
	{
		GetTree().Paused = true;
		EXEBehaviour behaviour = (EXEBehaviour)playerModuleBuilder.coreBody.module.behaviour;
		IExplodable asExplodable = (IExplodable)behaviour;
		await ToSignal(GetTree().CreateTimer(0.8, true), SceneTreeTimer.SignalName.Timeout);
		
		for (int i = 0; i < playerModuleBuilder.UsedModules.Count; i++)
		{
			Vector2 pos = playerModuleBuilder.coreBody.GlobalPosition + new Vector2(Random.Shared.Next(-250, 150), Random.Shared.Next(-100, 100));
			asExplodable.SpawnExplosion(playerModuleBuilder, pos, playerModuleBuilder.UsedModules.GetValueAtIndex(i).module.fileExtension);
			await ToSignal(GetTree().CreateTimer(0.2, true), SceneTreeTimer.SignalName.Timeout);


		}
		await ToSignal(GetTree().CreateTimer(playerModuleBuilder.UsedModules.Count * 0.7f, true), SceneTreeTimer.SignalName.Timeout);
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://you_lose.tscn");

	}

	private void GameWon()
	{
		// End game, TODO
		GD.Print("You won!"); 
		GetTree().ChangeSceneToFile("res://you_win.tscn");
	}
}
