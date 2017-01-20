using UnityEngine;

public class CameraDropDownAttribute : PropertyAttribute {
	
	public bool fullHierarchy;

	public CameraDropDownAttribute(bool fullHierarchy = false) {
		this.fullHierarchy = fullHierarchy;
	}

}