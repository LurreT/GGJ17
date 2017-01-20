using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class StringOptionsAttribute : PropertyAttribute {

	public string[] options;

	public StringOptionsAttribute() {
		this.options = new string[] { "-" };
	}

	public StringOptionsAttribute(string[] options) {
		this.options = options;
	}

}