using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(ArtilleryMaster))]
public class e_ArtilleryMaster : Editor {

	private ReorderableList units;
	private ReorderableList spawners;
	private ReorderableList strikes;

	private void OnEnable() {
		// Units
		units = new ReorderableList(serializedObject, serializedObject.FindProperty("units"), true, true, true, true);

		units.drawHeaderCallback += rect => {
			EditorGUI.LabelField(rect, units.serializedProperty.displayName);
		};

		units.drawElementCallback += (rect, index, isActive, isFocused) => {
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = units.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(rect, item, GUIContent.none);
		};
		
		// Spawners
		spawners = new ReorderableList(serializedObject, serializedObject.FindProperty("spawners"), true, true, true, true);

		spawners.drawHeaderCallback += rect => {
			EditorGUI.LabelField(rect, spawners.serializedProperty.displayName);
		};

		spawners.drawElementCallback += (rect, index, isActive, isFocused) => {
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = spawners.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(rect, item, GUIContent.none);
		};

		// Strikes
		strikes = new ReorderableList(serializedObject, serializedObject.FindProperty("strikes"), true, true, true, true);

		strikes.drawHeaderCallback += rect => {
			Rect labelRect = new Rect(rect.x, rect.y, rect.width - 68, rect.height);
			Rect timestampRect = new Rect(labelRect.xMax + 4, rect.y, 30, rect.height);
			Rect flytimeRect = new Rect(timestampRect.xMax + 4, rect.y, 30, rect.height);
			EditorGUI.LabelField(labelRect, strikes.serializedProperty.displayName);
			EditorGUI.LabelField(timestampRect, "TIME");
			EditorGUI.LabelField(flytimeRect, "PRE");
		};
		
		strikes.drawElementCallback += (rect, index, isActive, isFocused) => { 
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = strikes.serializedProperty.GetArrayElementAtIndex(index);

			var spawner = item.FindPropertyRelative("spawnerName");
			var unit = item.FindPropertyRelative("unitName");
			var timestamp = item.FindPropertyRelative("timestamp");
			var flytime = item.FindPropertyRelative("flytime");

			if (flytime.floatValue <= float.Epsilon) flytime.floatValue = 3;

			Rect spawnerRect = new Rect(rect.x, rect.y, (rect.width - 68) * 0.5f - 2, rect.height);
			Rect unitRect = new Rect(spawnerRect.xMax + 4, rect.y, spawnerRect.width, rect.height);
			Rect timestampRect = new Rect(unitRect.xMax + 4, rect.y, 30, rect.height);
			Rect flytimeRect = new Rect(timestampRect.xMax + 4, rect.y, 30, rect.height);

			//EditorGUI.PropertyField(spawnerRect, spawner, GUIContent.none);
			//EditorGUI.PropertyField(unitRect, unit, GUIContent.none);

			spawner.stringValue = DrawPopupThingie(spawnerRect, spawner, spawners.serializedProperty);
			unit.stringValue = DrawPopupThingie(unitRect, unit, units.serializedProperty);

			EditorGUI.PropertyField(timestampRect, timestamp, GUIContent.none);
			EditorGUI.PropertyField(flytimeRect, flytime, GUIContent.none);
		};
	}

	private string DrawPopupThingie(Rect rect, SerializedProperty prop, SerializedProperty basedOf) {

		List<string> display = new List<string>();
		display.Add("-");

		// fill array
		for (int i=0; i<basedOf.arraySize; i++) {
			var p = basedOf.GetArrayElementAtIndex(i);
			if (p.objectReferenceValue != null) display.Add(p.objectReferenceValue.name);
		}

		// check which is selected
		int selected = 0;
		for (int i = 1; i < display.Count; i++) {
			if (display[i] == prop.stringValue) {
				selected = i;
				break;
			}
		}

		selected = EditorGUI.Popup(rect, selected, display.ToArray());

		if (selected == 0) return string.Empty;
		return display[selected];
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		serializedObject.Update();
		units.DoLayoutList();
		EditorGUILayout.Space();
		spawners.DoLayoutList();
		EditorGUILayout.Space();
		strikes.DoLayoutList();
		EditorGUILayout.Space();

		if (GUILayout.Button("Sort by TIME")) {

			bool any;
			do {
				any = false;

				for (int i=0; i<strikes.serializedProperty.arraySize-1; i++) {
					var a = strikes.serializedProperty.GetArrayElementAtIndex(i);
					var b = strikes.serializedProperty.GetArrayElementAtIndex(i+1);

					if (a.FindPropertyRelative("timestamp").floatValue > b.FindPropertyRelative("timestamp").floatValue) {
						strikes.serializedProperty.MoveArrayElement(i + 1, i);
						any = true;
					}

				}

			} while (any);

			Debug.Log("SORTED");
		}

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}

}
