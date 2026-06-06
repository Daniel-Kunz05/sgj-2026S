using Godot;
using System;
using System.Reflection;
using sgj.Module;
namespace sgj.Behaviour;

[AttributeUsage(AttributeTargets.Field)]
file class BehaviourAttribute(Type @class) : Attribute
{
	public readonly Func<Module.Module, Behaviour> constructor = (module) =>
		(Behaviour)@class.GetConstructor(BindingFlags.Public | BindingFlags.Instance, [typeof(Module.Module)])!.Invoke([module]);
}
[AttributeUsage(AttributeTargets.Field)]
file class IconAttribute(string iconPath) : Attribute
{
	public readonly string iconPath = iconPath;
}
[AttributeUsage(AttributeTargets.Field)]
file class CostAttribute(int cost) : Attribute
{
	public readonly int cost = cost;
}
[AttributeUsage(AttributeTargets.Field)]
file class ToolTipAttribute(string tooltipBody) : Attribute
{
	public readonly string tooltipBody = tooltipBody;
}


public enum FileExtension
{
	[Icon("res://06 Modules/Sprites/Filetypes/pdf.png"), ToolTip("awesome")] PDF,
	[Behaviour(typeof(MP3Behaviour)), Icon("res://06 Modules/Sprites/Filetypes/mp3.png")] MP3,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/txt.png")] TXT,
	[Icon("res://06 Modules/Sprites/Filetypes/mp4.png")] MP4,
	[Icon("res://06 Modules/Sprites/Filetypes/sh.png")] SH,
	[Icon("res://06 Modules/Sprites/Filetypes/zip.png")] ZIP,
	[Icon("res://06 Modules/Sprites/Filetypes/png.png")] PNG,
	[Icon("res://06 Modules/Sprites/Filetypes/obj.png")] OBJ,
	[Icon("res://06 Modules/Sprites/Filetypes/godot.png")] GODOT,
	[Icon("res://06 Modules/Sprites/Filetypes/json.png")] JSON,
	[Behaviour(typeof(EXEBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/test.png")] EXE,
}

public static class BehaviourTypeExtensions
{
	extension(FileExtension type)
	{
		private T? GetAttr<T>() where T : Attribute
		{
			return typeof(FileExtension).GetMember(type.ToString())[0]?.GetCustomAttribute<T>();
		}

		public Func<Module.Module, Behaviour> Constructor => type.GetAttr<BehaviourAttribute>()?.constructor ?? throw new Exception();
		public string IconPath => type.GetAttr<IconAttribute>()?.iconPath ?? throw new Exception();
		public int Cost => type.GetAttr<CostAttribute>()?.cost ?? throw new Exception();
		public string ToolTip => type.GetAttr<ToolTipAttribute>()?.tooltipBody ?? throw new Exception();
	}
}
