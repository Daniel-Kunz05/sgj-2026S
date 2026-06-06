using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
namespace sgj.NameGeneration;

[GlobalClass]
public partial class PathGenerator : Node
{
	public static readonly string PREFIX = "/home";
	public static readonly string[] USERS = ["alex", "victor", "dawin", "felix", "meilun", "wafiro", "quirlight", "phillip"];
	public static readonly string[] HOME_DIRECTORIES = [
		"anaconda3",
		"Desktop",
		"Documents",
		"Downloads",
		"Music",
		"Pictures",
		"Programs",
		"snap",
		"Videos",
		"tum"
	];

	public static readonly System.Collections.Generic.Dictionary<string, string[]> SUBDIRECTORIES = new()
	{
		["anaconda3"] = ["bin", "doc", "etc", "lib", "man", "share", "pkgs"],
		["Desktop"] = ["Shortcuts", "Untitled Folder", "Stuff", "Old", "TODO", "untitled-final-final-v%1[1-6]"],
		["Documents"] = ["job", "scans", "Audacity", "notebooks", "books", "ebooks", "to-read", "backup"],
		["Downloads"] = ["SuperTux-v0.%1[0-6].3", "__MACOSX", "klausuren", "ffmpeg-6.1.1-full-build", "archive", "tmp"],
		["Music"] = ["lo-fi", "pop", "backup", "rock", "jazz", "rap"],
		["Pictures"] = ["ski-trip", "20%1[01]%1d", "202%1[0-6]", "Alpen", "family", "Ea-nasir"],
		["Programs"] = ["Godot-4.%1[0-6].3", "ffmpeg", "zulip"],
		["snap"] = ["xournalpp", "code", "rider", "rustup", "supertux"],
		["Videos"] = ["Screencasts", "minecraft", "supertux", "cube earth evidence"],
		["tum"] = ["EIST", "GDB", "GBS", "GRNVS", "SGJ202%1[0-6]", "PGDP"]
	};
	public static (string username, string path) Generate()
	{
		string home_dir = HOME_DIRECTORIES.PickRandom();
		string user = USERS.PickRandom();
		return (user, FormatUtil.Format(PREFIX + "/" + user + "/" + home_dir + "/" + SUBDIRECTORIES[home_dir].PickRandom()));
	}
}
