using Godot;
using sgj;
using System;
using System.Collections.Generic;

public enum CmdlineAction
{
	NOP,
	POP_DIR
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
	private string currentPath = null!;
	private const double CC_DELAY = 0.3;
	private const double COMMAND_DURATION = 0.5;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ [/font]";
		timer = 0;
		cctimer = 0;
		queuedCommands = new();
		currentCommand = "";
		userName = Database.Instance.userName;
		playerName = Database.Instance.playerName;
		currentPath = Database.Instance.gamePath;
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
		if (timer > COMMAND_DURATION && queuedCommands.Count == 0)
		{
			cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ {currentCommand}[/font]";
			return;
		}
		if (timer > COMMAND_DURATION)
		{
			(currentCommand, nextAction, nextCallback) = queuedCommands.Dequeue();
			timer = 0;
		}
		timer += delta;
		cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ {currentCommand.Substring(0, (int)(currentCommand.Length * double.Clamp(0, 1, timer / COMMAND_DURATION)))}[/font]";
		if (timer > COMMAND_DURATION)
		{
			nextCallback();
			switch (nextAction)
			{
				case CmdlineAction.POP_DIR:
					{
						int i = currentPath.LastIndexOf('/');
						currentPath = currentPath[..i];
						if (currentPath == "") { currentPath = "/"; }
					}
					break;
				default:
					break;
			}
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
