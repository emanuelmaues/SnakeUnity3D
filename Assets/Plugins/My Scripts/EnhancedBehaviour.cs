using UnityEngine;
using System.Collections;

public class EnhancedBehaviour : MonoBehaviour {

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		EnhancedAwake();
	}

	protected virtual void EnhancedAwake() { }

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		EnhancedStart();
	}

	protected virtual void EnhancedStart() { }
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		EnhancedUpdate();
	}

	protected virtual void EnhancedUpdate() { }

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable() {
		EnhancedOnEnable();
	}

	protected virtual void EnhancedOnEnable() { }

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnDisable() {
		EnhancedOnDisable();
	}
	
	protected virtual void EnhancedOnDisable() { }

	/// <summary>
	/// Raises the application focus event.
	/// </summary>
	/// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
	void OnApplicationFocus(bool pauseStatus) {
		EnhancedOnApplicationFocus(pauseStatus);
	}

	protected virtual void EnhancedOnApplicationFocus(bool pauseStatus) { }

	/// <summary>
	/// Lates the update.
	/// </summary>
	void LateUpdate() {
		EnhancedLateUpdate();
	}

	protected virtual void EnhancedLateUpdate() { }

	void FixedUpdate() {
		EnhancedFixedUpdate();
	}
	
	protected virtual void EnhancedFixedUpdate() { }

	void OnTriggerEnter(Collider col) {
		EnhancedOnTriggerEnter (col);
	}

	protected virtual void EnhancedOnTriggerEnter(Collider col) { }

	void OnTriggerExit(Collider col) {
		EnhancedOnTriggerExit(col);
	}

	protected virtual void EnhancedOnTriggerExit(Collider col) { }

	/// <summary>
	/// Raises the became visible event.
	/// </summary>
	void OnBecameVisible() {
		EnhancedOnBecameVisible();
	}

	protected virtual void EnhancedOnBecameVisible() { }

	void OnDestroy() {
		EnhancedOnDestroy();
	}

	protected virtual void EnhancedOnDestroy() { }

	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	void OnApplicationQuit() {
		EnhancedOnApplicationQuit();
	}

	protected virtual void EnhancedOnApplicationQuit() { }

	void OnParticleCollision(GameObject other) {
		EnhancedOnParticleCollision(other);
	}

	protected virtual void EnhancedOnParticleCollision(GameObject other) { }
	
	/// <summary>
	/// Raises the became invisible event.
	/// </summary>
	void OnBecameInvisible() {
		EnhancedOnBecameInvisible();
	}
	
	protected virtual void EnhancedOnBecameInvisible() { }

	/// <summary>
	/// Gets the or add component.
	/// </summary>
	/// <returns>The or add component.</returns>
	/// <param name="child">Child.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	protected T GetOrAddComponent<T> () where T: Component {
		return gameObject.GetOrAddComponent<T>();
	}
}

#region Extensions
static public class EnhancedExtensionMethods {

	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	static public T GetOrAddComponent<T> (this Component child, bool searchChild = false) where T: Component {

		T result = null;

		if(!searchChild) {
			result = child.GetComponent<T>();
		}
		else {
			result = child.GetComponentInChildren<T>();
		}

		if (result == null) {
			result = child.gameObject.AddComponent<T>();
		}
		return result;
	}

	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	static public T GetOrAddComponent<T> (this GameObject child, bool searchChild = false) where T: Component {

		T result = null;
		
		if(!searchChild) {
			result = child.GetComponent<T>();
		}
		else {
			result = child.GetComponentInChildren<T>();
		}
		if (result == null) {
			result = child.AddComponent<T>();
		}
		return result;
	}
}
#endregion
