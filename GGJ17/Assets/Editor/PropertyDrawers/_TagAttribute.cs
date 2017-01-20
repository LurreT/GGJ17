using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(TagDropDownAttribute))]
public class _TagAttribute : PropertyDrawer {


	// Draw the int as a list popup
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty (position, label, property);

		if (property.propertyType == SerializedPropertyType.String) {
			property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
		} else {
			EditorGUI.LabelField(position,label.text, "Use Tag attribute with string!");
		}

		EditorGUI.EndProperty();
	}

}
