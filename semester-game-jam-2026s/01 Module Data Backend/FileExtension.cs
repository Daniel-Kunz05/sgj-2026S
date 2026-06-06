using Godot;
using System;
using System.Reflection;
namespace sgj.Behaviour;

[AttributeUsage(AttributeTargets.Field)]
file class BehaviourAttribute(Type @class) : Attribute
{
	public readonly Func<Behaviour> constructor = () =>
		(Behaviour)@class.GetConstructor(BindingFlags.Public | BindingFlags.Instance, [])!.Invoke([]);
}


public enum FileExtension
{
	PDF,
	MP3,
	TXT,
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

		public Func<Behaviour> constructor => type.GetAttr<BehaviourAttribute>()?.constructor ?? throw new Exception();
	}
}