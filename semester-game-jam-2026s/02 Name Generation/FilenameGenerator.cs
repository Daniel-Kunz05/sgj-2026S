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
		[FileExtension.PDF] = ["timesheet-20%1[01]%1[0-9]-0%1[1-9]-%1[0-2]%1d", "BSc-thesis-final-final", "tutor kickoff", "the-earth-is-a-donut"],
		[FileExtension.MP3] = ["Sk8er Boi", "Megalovania", "I've been watching you", "IRIS OUT", "Beggin'", "Fairytale", "Maestro", "So What", "APT", "Europapa", "Call Me Maybe", "Black Knife"],
		[FileExtension.TXT] = ["todo", "tmp%3d", "%6s", "stuff", "Untitled", "Untitled (%1[1-9])"],
		[FileExtension.MP4] = ["Iron Lung", "The LEGO Movie", "Plagiarism and You(tube)", "gig", "😺"],
		[FileExtension.SH] = ["install", "clone", "mount_win", "turn_off_gpu", "temp", "fix_touchpad"],
		[FileExtension.ZIP] = ["jetbrains-toolbox", "archive", "source", "grnvs-altklausuren"],
		[FileExtension.PNG] = ["ski", "%9d", "coconut", "elizabeth II", "SGJ202%1[0-5] group"],
		[FileExtension.OBJ] = ["garfield", "temp", "100M toothbrush", "face-scan"],
		[FileExtension.GODOT] = ["project", "this-store-is-fine", "legacy-of-magic", "grawitty"],
		[FileExtension.JSON] = ["database", "names", "translation", "config"]
	};

	public static string Generate(FileExtension extension)
	{
		string name = NAMES[extension].PickRandom();
		name = FormatUtil.Format(name);
		return name + "." + extension.ToString().ToLower();
	}


}
