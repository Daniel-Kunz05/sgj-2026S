using Godot;
using sgj;
using System;
using System.Collections.Generic;

public enum CmdlineAction
{
	NOP,
	POP_DIR,
	PUSH_PLAYER_DIR,
}

[GlobalClass]
public partial class MainCmdlineController : Node
{
	[Export] public RichTextLabel cmdline = null!;
	[Export] public double timer;
	private bool ctrl_cd;
	private double cctimer;
	private Queue<(string, CmdlineAction, Action)> queuedCommands = null!;
	private string currentCommand = null!;
	private CmdlineAction nextAction = CmdlineAction.NOP;
	private Action nextCallback = delegate { };
	private string userName = null!;
	private string playerName = null!;
	public string CurrentPath { get; private set; } = null!;
	private const double CC_DELAY = 0.3;
	private const double COMMAND_DURATION = 0.5;
	private const double COMMAND_DELAY = 0.25;
	private const double TOTAL_COMMAND_TIME = (COMMAND_DURATION + COMMAND_DELAY);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = COMMAND_DURATION * 2;
		cctimer = 0;
		queuedCommands = new();
		currentCommand = "";
		userName = Database.Instance.userName;
		playerName = Database.Instance.playerName;
		CurrentPath = Database.Instance.gamePath;
		UpdateCmdline();
		EnqueueCommand($"mkdir {playerName}");
		EnqueueCommand($"cd {playerName}", CmdlineAction.PUSH_PLAYER_DIR, GameManager.instance.StartGame);
	}

	private void UpdateCmdline()
	{
		cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{userName}@PC[/color]:[color=#11d116]{CurrentPath}[/color]$ {currentCommand}[/font]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ctrl_cd)
		{
			cctimer += delta;
			if (cctimer > CC_DELAY)
			{
				GetTree().Quit();
			}
			return;
		}
		if (timer > TOTAL_COMMAND_TIME && queuedCommands.Count == 0)
		{
			UpdateCmdline();
			return;
		}
		if (timer > TOTAL_COMMAND_TIME)
		{
			(currentCommand, nextAction, nextCallback) = queuedCommands.Dequeue();
			timer = 0;
		}
		timer += delta;
		cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{userName}@PC[/color]:[color=#11d116]{CurrentPath}[/color]$ {currentCommand.Substring(0, (int)(currentCommand.Length * double.Clamp(timer / COMMAND_DURATION, 0, 1)))}[/font]";
		if (timer > TOTAL_COMMAND_TIME)
		{
			switch (nextAction)
			{
				case CmdlineAction.PUSH_PLAYER_DIR:
					{
						if (CurrentPath == "/") { CurrentPath = $"/{playerName}"; }
						else { CurrentPath = $"{CurrentPath}/{playerName}"; }
						UpdateCmdline();
					}
					break;
				case CmdlineAction.POP_DIR:
					{
						int i = CurrentPath.LastIndexOf('/');
						CurrentPath = CurrentPath[..i];
						if (CurrentPath == "") { CurrentPath = "/"; }
						UpdateCmdline();
					}
					break;
				default:
					break;
			}
			nextCallback();
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (ctrl_cd)
		{
			return;
		}
		if (@event is InputEventKey keyEvent2 && keyEvent2.Pressed && keyEvent2.Keycode == Key.C && Input.IsKeyPressed(Key.Ctrl))
		{
			if (cmdline.Text.Contains('█'))
			{
				cmdline.Text = cmdline.Text.Replace("█[/font]", "^C[/font]");
			}
			else
			{
				cmdline.Text = cmdline.Text.Replace("[/font]", "^C[/font]");
			}
			ctrl_cd = true;
		}
	}
	public void EnqueueCommand(string command, CmdlineAction action = CmdlineAction.NOP, Action? callback = null)
	{
		queuedCommands.Enqueue((command, action, callback ?? delegate { }));
	}
}
