using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderVariantCollectionImpl
{
    private ShaderVariantCollection m_ShaderVariantCollection;
    private SerializedObject m_SerializedObject;
    
    private Dictionary<Shader, Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>>> m_ShaderVariants;
    public Dictionary<Shader, Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>>> ShaderVariants => m_ShaderVariants;
    
    public ShaderVariantCollectionImpl(ShaderVariantCollection shaderVariantCollection)
    {
        m_ShaderVariantCollection = shaderVariantCollection;
        m_SerializedObject = new SerializedObject(shaderVariantCollection);
        m_ShaderVariants = new (shaderVariantCollection.shaderCount);
        var shadersProp = m_SerializedObject.FindProperty("m_Shaders");
        for (int i = 0; i < shadersProp.arraySize; i++)
        {
            var shaderProp = shadersProp.GetArrayElementAtIndex(i);
            ProcessShaderProp(shaderProp);
        }
    }
    
    public void ProcessShaderProp(SerializedProperty shaderProp)
    {
        Shader shader = (Shader)shaderProp.FindPropertyRelative("first").objectReferenceValue;
        var variantsProp = shaderProp.FindPropertyRelative("second.variants");
        var shaderVariantDict = new Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>>(variantsProp.arraySize);
        m_ShaderVariants.Add(shader, shaderVariantDict);
        
        for (var i = 0; i < variantsProp.arraySize; ++i)
        {
            var prop = variantsProp.GetArrayElementAtIndex(i);
            string[] keywords = prop.FindPropertyRelative("keywords").stringValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            PassType passType = (PassType)prop.FindPropertyRelative("passType").intValue;
            ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(shader, (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue, keywords);
            if (!shaderVariantDict.TryGetValue(passType, out List<ShaderVariantCollection.ShaderVariant> variants))
            {
                variants = new List<ShaderVariantCollection.ShaderVariant>();
                shaderVariantDict.Add(passType, variants);
            }
            variants.Add(variant);
        }
    }
}