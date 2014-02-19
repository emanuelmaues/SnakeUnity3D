using UnityEngine;
using System;
using System.Collections;

public partial class TouchKit {

	/// <summary>
	/// The drag.
	/// </summary>
	TKPanRecognizer drag;

	/// <summary>
	/// The tap.
	/// </summary>
	TKTapRecognizer tap;

	/// <summary>
	/// The swipe.
	/// </summary>
	TKSwipeRecognizer swipe;

	/// <summary>
	/// Any touch.
	/// </summary>
	TKAnyTouchRecognizer anyTouch;

	void SetPanRecognizer() {
		if(drag == null) {
			drag = new TKPanRecognizer();
			TouchKit.addGestureRecognizer(drag);
		}
	}

	/// <summary>
	/// Occurs when on drag.
	/// </summary>
	public event Action<TKPanRecognizer> OnDrag {
		add {
			SetPanRecognizer();
			drag.gestureRecognizedEvent += value;
		}
		remove {
			SetPanRecognizer();
			drag.gestureRecognizedEvent -= value;
		}
	}
	
	/// <summary>
	/// Occurs when on drag complete.
	/// </summary>
	public event Action<TKPanRecognizer> OnDragComplete {
		add {
			SetPanRecognizer();
			drag.gestureCompleteEvent += value;
		}
		remove {
			SetPanRecognizer();
			drag.gestureCompleteEvent -= value;
		}
	}

	void SetTapRecognizer() {
		if(tap == null) {
			tap = new TKTapRecognizer();
			TouchKit.addGestureRecognizer(tap);
		}
	}

	/// <summary>
	/// Occurs when on tap.
	/// </summary>
	public event Action<TKTapRecognizer> OnTap {
		add {
			SetTapRecognizer();
			tap.gestureRecognizedEvent += value;
		}
		remove {
			SetTapRecognizer();
			tap.gestureRecognizedEvent -= value;
		}
	}

	void SetSwipeRecognizer() {
		if(swipe == null) {
			swipe = new TKSwipeRecognizer();
			TouchKit.addGestureRecognizer(swipe);
		}
	}

	public event Action<TKSwipeRecognizer> OnSwipe {
		add {
			SetSwipeRecognizer();
			swipe.gestureRecognizedEvent += value;
		}
		remove {
			SetSwipeRecognizer();
			swipe.gestureRecognizedEvent += value;
		}
	}
}
