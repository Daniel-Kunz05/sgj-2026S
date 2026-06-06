using Godot;
using System;

[GlobalClass]
public partial class MainMenuAnimator : Node
{
	private const string NextScene = "res://post_process_test.tscn";
	[Export] public RichTextLabel startCommand = null!;
	[Export] public RichTextLabel titleText = null!;
	[Export] public RichTextLabel confirm = null!;
	private double timer;
	private bool yesInput;
	private bool noInput;
	private bool ctrl_cd;
	private double cctimer;
	private const double CC_DELAY = 0.3;
	private const double START_COMMAND_DELAY = 1;
	private const double START_COMMAND_DURATION = 2;
	private const double TITLE_TEXT_DELAY = 0.5;
	private const double TITLE_TEXT_DURATION = 3;
	private const double CONFIRM_TEXT_DELAY = 0.5;
	private const double CONFIRM_TEXT_DURATION = 2;
	private const double TOTAL_DELAY = START_COMMAND_DELAY
									 + START_COMMAND_DURATION
									 + TITLE_TEXT_DELAY
									 + TITLE_TEXT_DURATION
									 + CONFIRM_TEXT_DELAY
									 + CONFIRM_TEXT_DURATION;
	private static readonly string title_text = @"███████╗██╗██╗     ███████╗    ███████╗██████╗ ███████╗███╗   ██╗███████╗██╗   ██╗
██╔════╝██║██║     ██╔════╝    ██╔════╝██╔══██╗██╔════╝████╗  ██║╚══███╔╝╚██╗ ██╔╝
█████╗  ██║██║     █████╗      █████╗  ██████╔╝█████╗  ██╔██╗ ██║  ███╔╝  ╚████╔╝ 
██╔══╝  ██║██║     ██╔══╝      ██╔══╝  ██╔══██╗██╔══╝  ██║╚██╗██║ ███╔╝    ╚██╔╝  
██║     ██║███████╗███████╗    ██║     ██║  ██║███████╗██║ ╚████║███████╗   ██║   
╚═╝     ╚═╝╚══════╝╚══════╝    ╚═╝     ╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚══════╝   ╚═╝   
                                                                                  ";
	private static readonly string confirm_text = @"Viruses run rampant on this disk.
After this operation, only one will remain.
Is this ok [y/N]:";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = 0;
		startCommand.Text = "[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# [/font]";
		titleText.Text = "";
		confirm.Text = "";
		yesInput = false;
		noInput = false;
		ctrl_cd = false;
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
			string start_game = "./start-game";
			startCommand.Text = $"[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# {start_game.Substring(0, (int)(start_game.Length * ((timer - bar) / START_COMMAND_DURATION)))}[/font]";
			return;
		}
		bar += START_COMMAND_DURATION;
		startCommand.Text = "[font=res://NotoSansMono.ttf][color=#8e44ad]root@PC[/color]:[color=#8e44ad]/bin[/color]# ./start-game[/font]";
		if (timer < bar + TITLE_TEXT_DELAY)
		{
			return;
		}
		bar += TITLE_TEXT_DELAY;
		var split = title_text.Split('\n');
		if (timer < bar + TITLE_TEXT_DURATION)
		{
			titleText.Text = $"[font=res://NotoSansMono.ttf]{string.Join("\n", split[..(int)(split.Length * (timer - bar) / TITLE_TEXT_DURATION)])}[/font]";
			return;
		}
		bar += TITLE_TEXT_DURATION;
		titleText.Text = $"[font=res://NotoSansMono.ttf]{title_text}[/font]";
		if (timer < bar + CONFIRM_TEXT_DELAY)
		{
			return;
		}
		bar += CONFIRM_TEXT_DELAY;
		split = confirm_text.Split('\n');
		if (timer < bar + CONFIRM_TEXT_DURATION)
		{
			confirm.Text = $"[font=res://NotoSansMono.ttf]{string.Join("\n", split[..(int)(split.Length * (timer - bar) / CONFIRM_TEXT_DURATION)])}[/font]";
			return;
		}
		confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text}[/font]";
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
				if (timer < START_COMMAND_DELAY + START_COMMAND_DURATION + TITLE_TEXT_DELAY)
				{
					startCommand.Text = startCommand.Text.Replace("[/font]", "^C[/font]");
				}
				else if (timer < START_COMMAND_DELAY + START_COMMAND_DURATION + TITLE_TEXT_DELAY + TITLE_TEXT_DURATION + CONFIRM_TEXT_DELAY)
				{
					titleText.Text = titleText.Text[..(^9)] + "^C[/font]";
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
			else if (keyEvent.Keycode == Key.N && !noInput && !yesInput && Input.IsKeyPressed(Key.Shift))
			{
				noInput = true;
				confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} N[/font]";
				return;
			}
			else if (keyEvent.Keycode == Key.Backspace && (noInput || yesInput))
			{
				noInput = yesInput = false;
				confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text}[/font]";
			}
			else if (keyEvent.Keycode == Key.Enter && (noInput || yesInput))
			{
				if (noInput)
				{
					GetTree().Quit();
				}
				if (yesInput)
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
					confirm.Text = $"[font=res://NotoSansMono.ttf]{confirm_text} N^C[/font]";
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
