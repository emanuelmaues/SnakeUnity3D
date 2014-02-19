using UnityEngine;
using System.Collections;

public class OnTapLaunchScene : EnhancedBehaviour {

	protected override void EnhancedStart ()
	{
		base.EnhancedStart ();

		TouchKit.Instance.OnTap += HandleOnTap;
	}

	Collider2D myCollider;

	public Collider2D MyCollider {
		get {
			if(!myCollider) 
				myCollider = GetOrAddComponent<BoxCollider2D>();
			return myCollider;
		}
	}

	/// <summary>
	/// The name of the scene.
	/// </summary>
	[SerializeField]
	string sceneName;

	void HandleOnTap (TKTapRecognizer obj)
	{
		Application.LoadLevel(sceneName);
	}

	protected override void EnhancedOnDestroy ()
	{
		base.EnhancedOnDestroy ();
		TouchKit.Instance.OnTap -= HandleOnTap;
	}
}
