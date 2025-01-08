using System;
using UnityEngine;

class ValidatableScriptableObject : ScriptableObject
{
    public event Action OnValidateConfig;

    private void OnValidate() => OnValidateConfig?.Invoke();
}