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
		units = new ReorderableList(serializedObject, serializedObject.FindProperty("units"), false, true, true, true);

		units.drawHeaderCallback += rect => {
			EditorGUI.LabelField(rect, units.serializedProperty.displayName);
		};

		units.drawElementCallback += (rect, index, isActive, isFocused) => {
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = units.serializedProperty.GetArrayElementAtIndex(index);

			Rect indexRect = new Rect(rect.x, rect.y, 24, rect.height);
			Rect itemRect = new Rect(indexRect.xMax + 4, rect.y, rect.width - indexRect.width - 4, rect.height);

			EditorGUI.LabelField(indexRect, "[" + index + "]");
			EditorGUI.PropertyField(itemRect, item, GUIContent.none);
		};

		// Spawners
		spawners = new ReorderableList(serializedObject, serializedObject.FindProperty("spawners"), false, true, true, true);

		spawners.drawHeaderCallback += rect => {
			EditorGUI.LabelField(rect, spawners.serializedProperty.displayName);
		};

		spawners.drawElementCallback += (rect, index, isActive, isFocused) => {
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = spawners.serializedProperty.GetArrayElementAtIndex(index);

			Rect indexRect = new Rect(rect.x, rect.y, 24, rect.height);
			Rect itemRect = new Rect(indexRect.xMax + 4, rect.y, rect.width - indexRect.width - 4, rect.height);

			EditorGUI.LabelField(indexRect, "[" + index + "]");
			EditorGUI.PropertyField(itemRect, item, GUIContent.none);
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

			var spawner = item.FindPropertyRelative("spawnerIndex");
			var unit = item.FindPropertyRelative("unitIndex");
			var timestamp = item.FindPropertyRelative("timestamp");
			var flytime = item.FindPropertyRelative("flytime");

			if (flytime.floatValue <= float.Epsilon) flytime.floatValue = 3;

			Rect spawnerRect = new Rect(rect.x, rect.y, (rect.width - 68) * 0.5f - 2, rect.height);
			Rect unitRect = new Rect(spawnerRect.xMax + 4, rect.y, spawnerRect.width, rect.height);
			Rect timestampRect = new Rect(unitRect.xMax + 4, rect.y, 30, rect.height);
			Rect flytimeRect = new Rect(timestampRect.xMax + 4, rect.y, 30, rect.height);

			//EditorGUI.PropertyField(spawnerRect, spawner, GUIContent.none);
			//EditorGUI.PropertyField(unitRect, unit, GUIContent.none);

			spawner.intValue = DrawPopupThingie(spawnerRect, spawner, spawners.serializedProperty);
			unit.intValue = DrawPopupThingie(unitRect, unit, units.serializedProperty);

			EditorGUI.PropertyField(timestampRect, timestamp, GUIContent.none);
			EditorGUI.PropertyField(flytimeRect, flytime, GUIContent.none);
		};
	}

	private int DrawPopupThingie(Rect rect, SerializedProperty prop, SerializedProperty basedOf) {

		List<string> display = new List<string>();

		// fill array
		for (int i = 0; i < basedOf.arraySize; i++) {
			var p = basedOf.GetArrayElementAtIndex(i);
			if (p.objectReferenceValue != null) display.Add("[" + i + "] " + p.objectReferenceValue.name);
			else display.Add("[" + i + "] <null>");
		}

		// check which is selected
		int selected = prop.intValue;
		selected = EditorGUI.Popup(rect, selected, display.ToArray());
		
		return selected;
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

		if (GUILayout.Button("Sort strikes by TIME")) {

			bool any;
			do {
				any = false;

				for (int i = 0; i < strikes.serializedProperty.arraySize - 1; i++) {
					var a = strikes.serializedProperty.GetArrayElementAtIndex(i);
					var b = strikes.serializedProperty.GetArrayElementAtIndex(i+1);

					if (a.FindPropertyRelative("timestamp").floatValue > b.FindPropertyRelative("timestamp").floatValue) {
						strikes.serializedProperty.MoveArrayElement(i + 1, i);
						any = true;
					}

				}

			} while (any);

		}

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}

}