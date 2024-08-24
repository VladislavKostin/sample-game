using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Framework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class InjectAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignAttribute : Attribute
    {
        public virtual AssignSource AssignSource => AssignSource.Undefined;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignFromObjectAttribute : AssignAttribute
    {
        public override AssignSource AssignSource => AssignSource.Object;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignFromChildrenAttribute : AssignAttribute
    {
        public override AssignSource AssignSource => AssignSource.Children;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignFromPrefabAttribute : AssignAttribute
    {
        public override AssignSource AssignSource => AssignSource.Prefab;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignFromEntirePrefabAttribute : AssignAttribute
    {
        public override AssignSource AssignSource => AssignSource.Prefab;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssignFromSceneAttribute : AssignAttribute
    {
        public override AssignSource AssignSource => AssignSource.Scene;
    }

    public enum AssignSource
    {
        Undefined,
        Object,
        Children,
        Parent,
        Prefab,
        Scene
    }
}


// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
// public class InjectAttribute : Attribute { }

// [AttributeUsage(AttributeTargets.Class)]
// public abstract class InjectAsAttribute : Attribute
// {
//     public virtual InjectionType InjectionType => InjectionType.AsSingle;
// }

// [AttributeUsage(AttributeTargets.Class)]
// public class InjectAsSingleAttribute : InjectAsAttribute { }

// [AttributeUsage(AttributeTargets.Class)]
// public class InjectAsCurrentAttribute : InjectAsAttribute { }

// [AttributeUsage(AttributeTargets.Class)]
// public class InjectAsOneOfAttribute : InjectAsAttribute { }

// [AttributeUsage(AttributeTargets.Field)]
// public class InjectAttribute : Attribute
// {
//     public string CallbackMethod { get; }

//     public InjectAttribute(string callbackMethod = null)
//     {
//         CallbackMethod = callbackMethod;
//     }
// }

// [AttributeUsage(AttributeTargets.Field)]
// public class InjectMostRecentAttribute : InjectAttribute { }

// [AttributeUsage(AttributeTargets.Field)]
// public class InjectCurrentAttribute : InjectAttribute { }

// [AttributeUsage(AttributeTargets.Field)]
// public class InjectFirstAttribute : Attribute { }

// public enum InjectionType
// {
//     AsSingle,
//     AsCurrent,
//     AsOneOf,
// }