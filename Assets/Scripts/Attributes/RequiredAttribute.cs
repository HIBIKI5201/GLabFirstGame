using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class RequiredAttribute : PropertyAttribute { }
