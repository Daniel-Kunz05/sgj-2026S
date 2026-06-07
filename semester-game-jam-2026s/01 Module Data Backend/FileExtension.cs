using Godot;
using System;
using System.Reflection;
using sgj._05_Module_Builder.Modules;
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
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/pdf.png"), ToolTip("More complex than a regular text file.")] PDF,
	[Behaviour(typeof(MP3Behaviour)), Icon("res://06 Modules/Sprites/Filetypes/mp3.png"), ToolTip("Shoots musical projectiles.")] MP3,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/txt.png"), ToolTip("The plainest of files.\nTake more.")] TXT,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/mp4.png"), ToolTip("Like an image, but moving.")] MP4,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/sh.png"), ToolTip("Executable code. Be careful ;)")] SH,
	[Behaviour(typeof(ZIPBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/zip.png"), ToolTip("Explodes on impact.\nFriendly fire inbound.")] ZIP,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/png.png"), ToolTip("A still image of something.")] PNG,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/obj.png"), ToolTip("Like an image, but with more dimensions.")] OBJ,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/godot.png"), ToolTip("The better game engine.")] GODOT,
	[Behaviour(typeof(TXTBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/json.png"), ToolTip("Stores structured data.")] JSON,
	[Behaviour(typeof(EXEBehaviour)), Icon("res://06 Modules/Sprites/Filetypes/exe.png"), ToolTip("The core of your system. If it dies, you lose.")] EXE,
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
