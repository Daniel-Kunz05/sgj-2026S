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


public enum BehaviourType
{

}

public static class BehaviourTypeExtensions
{
	extension (BehaviourType type)
	{
		private T? GetAttr<T>() where T : Attribute
		{
			return typeof(BehaviourType).GetMember(type.ToString())[0]?.GetCustomAttribute<T>();
		}

		public Func<Behaviour> constructor => type.GetAttr<BehaviourAttribute>()?.constructor ?? throw new Exception();
	}
}