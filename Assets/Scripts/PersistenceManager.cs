using UnityEngine;
using System.Collections;

public class PersistenceManager : UnitySingleton<PersistenceManager> {

	/// <summary>
	/// The difficulty.
	/// </summary>
	int difficulty = 1;

	public int Difficulty {
		get {
			return difficulty;
		}
		set {
			difficulty = Mathf.Clamp(value, 1, 9);
		}
	}

	/// <summary>
	/// The max score.
	/// </summary>
	int maxScore = 0;

	public int MaxScore {
		get {
			if(maxScore <= 0) {
				maxScore = PlayerPrefs.GetInt("MAXSCORE", 0);
			}
			return maxScore;
		}
		set {
			maxScore = value;
		}
	}

	/// <summary>
	/// The last score.
	/// </summary>
	int lastScore = 0;

	public int LastScore {
		get {
			return lastScore;
		}
		set {
			lastScore = value;
		}
	}

	protected override void EnhancedOnDestroy ()
	{
		base.EnhancedOnDestroy ();
		PlayerPrefs.SetInt("MAXSCORE", MaxScore);
		PlayerPrefs.SetInt("DIFFICULTY", Difficulty);
		PlayerPrefs.Save();
	}
}
