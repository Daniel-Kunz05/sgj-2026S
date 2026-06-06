using Godot;
using sgj;
using System;

[GlobalClass]
public partial class IntroController : Node
{
	private const string NextScene = "res://main.tscn";
	[Export] public RichTextLabel cmdline = null!;
	[Export] public double timer;
	public double blinkTimer;
	public bool showCursor;
	private bool ctrl_cd;
	private double cctimer;
	private string playerName = null!;
	private const int MAX_PLAYER_NAME_LENGTH = 20;
	private const double CC_DELAY = 0.3;
	private const double START_COMMAND_DELAY = 1;
	private const double START_COMMAND_DURATION = 2;
	private const double TOTAL_DELAY = START_COMMAND_DELAY
									 + START_COMMAND_DURATION;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		(Database.Instance.userName, Database.Instance.gamePath) = sgj.NameGeneration.PathGenerator.Generate();
		cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ [/font]";
		timer = 0;
		blinkTimer = 0;
		showCursor = true;
		cctimer = 0;
		playerName = "";
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
		if (timer > TOTAL_DELAY)
		{
			cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ /usr/bin/register-player {playerName}{(showCursor ? "█" : "")}[/font]";
			if (blinkTimer > 1)
			{
				blinkTimer = 0;
				showCursor = !showCursor;
			}
			blinkTimer += delta;
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
			string register_player = "/usr/bin/register-player █";
			cmdline.Text = $"[font=res://NotoSansMono.ttf][color=#11d116]{Database.Instance.userName}@PC[/color]:[color=#11d116]{Database.Instance.gamePath}[/color]$ {register_player.Substring(0, (int)(register_player.Length * ((timer - bar) / START_COMMAND_DURATION)))}[/font]";
			return;
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
		if (timer < TOTAL_DELAY)
		{
			return;
		}
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (Key.A <= keyEvent.Keycode && keyEvent.Keycode <= Key.Z)
			{
				if (playerName.Length > MAX_PLAYER_NAME_LENGTH)
				{
					return;
				}
				if (Input.IsKeyPressed(Key.Shift))
				{
					playerName += (char)keyEvent.Keycode;
				}
				else
				{
					playerName += (char)(keyEvent.Keycode + 32);
				}
			}
			if (Key.Key0 <= keyEvent.Keycode && keyEvent.Keycode <= Key.Key9)
			{
				if (playerName.Length > MAX_PLAYER_NAME_LENGTH)
				{
					return;
				}
				playerName += (char)(keyEvent.Keycode - Key.Key0 + '0');
			}
			else if (keyEvent.Keycode == Key.Backspace && playerName.Length > 0)
			{
				playerName = playerName[..^1];
			}
			else if (keyEvent.Keycode == Key.Enter && playerName.Length > 0)
			{
				Database.Instance.playerName = playerName;
				//Database.Instance.gamePath += $"/{playerName}";
				GetTree().ChangeSceneToFile(NextScene);
			}
		}
	}
}
