using UnityEngine;
using System.Collections;

public class PresentHighScore : EnhancedBehaviour {

	protected override void EnhancedAwake ()
	{
		base.EnhancedAwake ();

		GetComponent<TextMesh>().text = "MAX SCORE: " + PersistenceManager.Instance.MaxScore;
	}
}
