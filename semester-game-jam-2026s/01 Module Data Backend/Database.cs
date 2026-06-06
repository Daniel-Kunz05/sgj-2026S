using Godot;
using sgj.Behaviour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace sgj;

using SaveType = Dictionary<Guid, (string name, string progress, List<(FileExtension ext, string fname, int x, int y)>)>;
[GlobalClass]
public partial class Database : Node
{
	public static Database Instance { get; private set; } = null!;
	private string path = null!;
	private string filename = "fighters.json";
	private string fullPath = null!;

	public SaveType SaveData { get; private set; } = [];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		path = ProjectSettings.GlobalizePath("user://");
		fullPath = Path.Join(path, filename);
		Load();
	}

	public static void Load()
	{
		if (!File.Exists(Instance.fullPath))
		{
			File.Create(Instance.fullPath);
			// Create default fighters here:
			Instance.SaveData = [];
			Save();
			return;
		}
		using FileStream openStream = File.OpenRead(Instance.fullPath);
		var data = JsonSerializer.Deserialize<SaveType>(openStream);
		if (data is null) return;
		Instance.SaveData = data;
	}

	public static void Save()
	{
		using FileStream createStream = File.Create(Instance.fullPath);
		JsonSerializer.Serialize(createStream, Instance.SaveData);
	}

	public static void AddFighter(string name, string progress, Module.Module[] modules)
	{
		var moduleTuples = modules.Select((m) => (m.fileExtension, m.fileName, m.x, m.y)).ToList();
		Instance.SaveData[Guid.NewGuid()] = (name, progress, moduleTuples);
	}

	public static (string name, string progress, Module.Module[] modules) GetFighter(Guid guid)
	{
		var (name, progress, moduleTuples) = Instance.SaveData[guid];
		return (name, progress, moduleTuples.Select((tup) => new Module.Module(tup.ext, tup.fname, tup.x, tup.y)).ToArray());
	}

	public static (string name, string progress, Module.Module[] modules) GetFighter(string progress)
	{
		return GetFighter(Instance.SaveData.Where((e) => e.Value.progress == progress).Select(e => e.Key).PickRandom());
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
