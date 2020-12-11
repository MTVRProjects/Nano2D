//================================================
//描述 ： 对枚举类型的字段进行解释
//作者 ：HML
//创建时间 ：2019/07/30 10:43:29  
//版本： 1.0 参考雨凇MOMO博文进行编写(http://www.manew.com/forum.php?mod=viewthread&tid=91745&extra=page%3D2%26filter%3Dtypeid%26typeid%3D131)
//================================================
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endif

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public class EnumExplainAttribute : PropertyAttribute
{
    public string label;
    public new int[] order = new int[0];
    public EnumExplainAttribute(string label)
    {
        this.label = label;
    }

    public EnumExplainAttribute(string label, params int[] order)
    {
        this.label = label;
        this.order = order;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumExplainAttribute))]
public class EnumExplainDrawer : PropertyDrawer
{
    private Dictionary<string, string> customEnumNames = new Dictionary<string, string>();


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SetUpCustomEnumNames(property, property.enumNames);

        if (property.propertyType == SerializedPropertyType.Enum)
        {
            EditorGUI.BeginChangeCheck();

            Debug.Log("customEnumNames.Count = " + customEnumNames.Count);

            //被标记的枚举元素的标记内容
            string[] displayedOptions = property.enumNames
                    .Where(enumName => customEnumNames.ContainsKey(enumName))
                    .Select<string, string>(enumName => customEnumNames[enumName])
                    .ToArray();

            //displayedOptions.Length = 实际被标记的枚举元素数量；
            //property.enumNames.Length = 9；

            //indexArray长度为0
            int[] indexArray = GetIndexArray(enumExplainAttribute.order);

            if (indexArray.Length != displayedOptions.Length)
            {
                indexArray = new int[displayedOptions.Length];
                for (int i = 0; i < indexArray.Length; i++)
                {
                    indexArray[i] = i;
                }
            }

            string[] items = new string[displayedOptions.Length];
            items[0] = displayedOptions[0];

            //displayedOptions有8个值（枚举元素的标记内容）
            for (int i = 0; i < displayedOptions.Length; i++)
            {
                //加入枚举元素说明
                items[i] = property.enumNames[i] + "   ("+ displayedOptions[indexArray[i]]+")";
            }
            int index = -1;
            for (int i = 0; i < indexArray.Length; i++)
            {
                //property.enumValueIndex是指标记枚举类型字段的标记顺序
                if (indexArray[i] == property.enumValueIndex)
                {
                    index = i;
                    break;
                }
            }
            if ((index == -1) && (property.enumValueIndex != -1)) { SortingError(position, property, label); return; }
            //1.绘制枚举类型的字段名以及解释：property.name
            //2.绘制被标记的枚举元素：items
            index = EditorGUI.Popup(position, property.name+"  (" + enumExplainAttribute.label+")", index, items);
            if (EditorGUI.EndChangeCheck())
            {
                if (index >= 0)
                    property.enumValueIndex = indexArray[index];
            }
        }
    }

    private EnumExplainAttribute enumExplainAttribute
    {
        get
        {
            return (EnumExplainAttribute)attribute;
        }
    }

    public void SetUpCustomEnumNames(SerializedProperty property, string[] enumNames)
    {

        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumExplainAttribute), false);

        foreach (EnumExplainAttribute customAttribute in customAttributes)
        {
            Type enumType = fieldInfo.FieldType;

            #region 2019.7.31

            Debug.Log("enumNames.Length = "+enumNames.Length);

            Debug.Log("property.enumDisplayNames = " + property.enumDisplayNames.Length+"："+property.enumDisplayNames[1]);
            for (int i = 0; i < enumNames.Length; i++)
            {
                var enumName = enumNames[i];
                FieldInfo field = enumType.GetField(enumName);
                if (field == null) continue;
                EnumExplainAttribute[] attrs = (EnumExplainAttribute[])field.GetCustomAttributes(customAttribute.GetType(), false);
                if (!customEnumNames.ContainsKey(enumName))
                {
                    for (int j = 0; j < attrs.Length; j++)
                    {
                        customEnumNames.Add(enumName, attrs[j].label);
                    }
                }
            }

            #endregion
        }
    }


    int[] GetIndexArray(int[] order)
    {
        int[] indexArray = new int[order.Length];
        for (int i = 0; i < order.Length; i++)
        {
            int index = 0;
            for (int j = 0; j < order.Length; j++)
            {
                if (order[i] > order[j])
                {
                    index++;
                }
            }
            indexArray[i] = index;
        }
        return (indexArray);
    }

    void SortingError(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, new GUIContent(label.text + " (sorting error)"));
        EditorGUI.EndProperty();
    }
}
public class EnumExplain
{
    static public object GetEnum(Type type, SerializedObject serializedObject, string path)
    {
        SerializedProperty property = GetPropety(serializedObject, path);
        return System.Enum.GetValues(type).GetValue(property.enumValueIndex);
    }
    static public object DrawEnum(Type type, SerializedObject serializedObject, string path)
    {
        return DrawEnum(type, serializedObject, GetPropety(serializedObject, path));
    }
    static public object DrawEnum(Type type, SerializedObject serializedObject, SerializedProperty property)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(property);
        serializedObject.ApplyModifiedProperties();
        return System.Enum.GetValues(type).GetValue(property.enumValueIndex);
    }
    static public SerializedProperty GetPropety(SerializedObject serializedObject, string path)
    {
        string[] contents = path.Split('/');
        SerializedProperty property = serializedObject.FindProperty(contents[0]);
        for (int i = 1; i < contents.Length; i++)
        {
            property = property.FindPropertyRelative(contents[i]);
        }
        return property;
    }
}
#endif