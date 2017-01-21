using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCollection : Cannon {

	public ArtilleryMaster parent;
	[HideInInspector]
	public List<ArtilleryMaster.SingleStrike> strikes = new List<ArtilleryMaster.SingleStrike>();

}
