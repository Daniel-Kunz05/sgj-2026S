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


public enum FileExtension
{
	PDF,
	MP3,
	[Behaviour(typeof(TXTBehaviour))] TXT,
	MP4,
	SH,
	ZIP,
	PNG,
	OBJ,
	GODOT,
	JSON,
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
	}
}