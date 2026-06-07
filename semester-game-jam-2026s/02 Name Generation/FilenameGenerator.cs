using Godot;
using sgj.Behaviour;
using System;
using System.Collections.Generic;
namespace sgj.NameGeneration;

[GlobalClass]
public partial class FilenameGenerator : Node
{
	public static readonly System.Collections.Generic.Dictionary<FileExtension, string[]> NAMES = new()
	{
		[FileExtension.PDF] = ["timesheet-20%1[01]%1[0-9]-0%1[1-9]-%1[0-2]%1d", "BSc-thesis-final-final", "tutor kickoff", "the-earth-is-a-donut", "Keep Talking and Nobody Explodes Manual", "Sunshine blue coin locations"],
		[FileExtension.MP3] = ["Sk8er Boi", "Megalovania", "I've been watching you", "IRIS OUT", "Beggin'", "Fairytale", "Maestro", "So What", "APT", "Europapa", "Call Me Maybe", "Black Knife", "Sweden"],
		[FileExtension.TXT] = ["todo", "tmp%3d", "%6s", "stuff", "Untitled", "Untitled (%1[1-9])", "top10gamesof20%1[01]%1d"],
		[FileExtension.MP4] = ["Iron Lung", "The LEGO Movie", "Plagiarism and You(tube)", "gig", "😺", "File Frenzy Walkthrough"],
		[FileExtension.SH] = ["install", "clone", "mount_win", "turn_off_gpu", "temp", "fix_touchpad", "copyfail poc"],
		[FileExtension.ZIP] = ["jetbrains-toolbox", "archive", "source", "grnvs-altklausuren", "artemis-source"],
		[FileExtension.PNG] = ["ski", "%9d", "coconut", "elizabeth II", "SGJ202%1[0-5] group", "ea-nasir"],
		[FileExtension.OBJ] = ["garfield", "temp", "100M toothbrush", "face-scan", "nomai statue"],
		[FileExtension.GODOT] = ["project", "this-store-is-fine", "legacy-of-magic", "grawitty", "file-frenzy"],
		[FileExtension.JSON] = ["database", "names", "translation", "config", "roles", "users", "sgj-teams"]
	};

	public static string Generate(FileExtension extension)
	{
		string name = NAMES[extension].PickRandom();
		name = FormatUtil.Format(name);
		return name + "." + extension.ToString().ToLower();
	}


}
