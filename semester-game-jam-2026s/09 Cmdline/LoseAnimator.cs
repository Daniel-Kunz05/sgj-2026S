using Godot;
using System;

[GlobalClass]
public partial class LoseAnimator : Node
{
	private const string NextScene = "res://main_menu.tscn";
	[Export] public RichTextLabel endCommand = null!;
	[Export] public RichTextLabel confirm = null!;
	[Export] public double timer;
	private bool yesInput;
	private bool noInput;
	private bool ctrl_cd;
	private double cctimer;
	private const double CC_DELAY = 0.3;
	private const double START_COMMAND_DELAY = 0.5;
	private const double START_COMMAND_DURATION = 1;
	private const double CONFIRM_TEXT_DELAY = 0.5;
	private const double CONFIRM_TEXT_DURATION = 2;
	private const double TOTAL_DELAY = START_COMMAND_DELAY
									 + START_COMMAND_DURATION
									 + CONFIRM_TEXT_DELAY
									 + CONFIRM_TEXT_DURATION;
	private static readonly string confirm_text = @"You have failed.
You have been slain and another virus has achieved ownership of Root.
Do you wish to play again [Y/n]:";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = 0;
		endCommand.Text = "[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# [/font]";
		confirm.Text = "";
		yesInput = false;
		noInput = false;
		ctrl_cd = false;
		cctimer = 0;
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
		if (timer > TOTAL_DELAY + 1)
		{
			return;
		}
		timer += delta;
		if (timer < START_COMMAND_DELAY)
		{
			return;
		}
		double bar = START_COMMAND_DELAY;
		if (timer < bar + START_COMMAND_DURATION)
		{
			string start_game = "./end-game";
			endCommand.Text = $"[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# {start_game.Substring(0, (int)(start_game.Length * ((timer - bar) / START_COMMAND_DURATION)))}[/font]";
			return;
		}
		bar += START_COMMAND_DURATION;
		endCommand.Text = "[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# ./end-game[/font]";
		if (timer < bar + CONFIRM_TEXT_DELAY)
		{
			return;
		}
		bar += CONFIRM_TEXT_DELAY;
		var split = confirm_text.Split('\n');
		if (timer < bar + CONFIRM_TEXT_DURATION)
		{
			confirm.Text = $"[font=res://NotoSansMono.ttf]{string.Join("\n", split[..(int)(split.Length * (timer - bar) / CONFIRM_TEXT_DURATION)])}[/font]";
			return;
		}
		if (!yesInput && !noInput)
		{ confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text}[/font]"; }
	}

	public override void _Input(InputEvent @event)
	{
		if (ctrl_cd)
		{
			return;
		}
		if (timer < TOTAL_DELAY)
		{
			if (@event is InputEventKey keyEvent2 && keyEvent2.Pressed && keyEvent2.Keycode == Key.C && Input.IsKeyPressed(Key.Ctrl))
			{
				if (timer < START_COMMAND_DELAY + START_COMMAND_DURATION + CONFIRM_TEXT_DELAY)
				{
					endCommand.Text = endCommand.Text.Replace("[/font]", "^C[/font]");
				}
				else
				{
					confirm.Text = confirm.Text.Replace("[/font]", "^C[/font]");
				}
				ctrl_cd = true;
			}
			return;
		}
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Y && !noInput && !yesInput)
			{
				yesInput = true;
				confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} y[/font]";
				return;
			}
			else if (keyEvent.Keycode == Key.N && !noInput && !yesInput)
			{
				noInput = true;
				confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} n[/font]";
				return;
			}
			else if (keyEvent.Keycode == Key.Backspace && (noInput || yesInput))
			{
				noInput = yesInput = false;
				confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text}[/font]";
			}
			else if (keyEvent.Keycode == Key.Enter)
			{
				if (noInput)
				{
					GetTree().Quit();
				}
				else
				{
					GetTree().ChangeSceneToFile(NextScene);
				}
			}
			else if (keyEvent.Keycode == Key.C && Input.IsKeyPressed(Key.Ctrl))
			{
				if (yesInput)
				{
					confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} y^C[/font]";
				}
				else if (noInput)
				{
					confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} n^C[/font]";
				}
				else
				{
					confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} ^C[/font]";
				}
				ctrl_cd = true;
			}
		}
	}
}
