using UnityEngine;
using System.Collections;

public class PresentScore : EnhancedBehaviour {

	/// <summary>
	/// The score.
	/// </summary>
	[SerializeField]
	TextMesh score;

	/// <summary>
	/// The minimum size of the score.
	/// </summary>
	float minScoreSize = 0.02f;

	/// <summary>
	/// The size of the max score.
	/// </summary>
	float maxScoreSize = 0.03f;

	protected override void EnhancedAwake ()
	{
		base.EnhancedAwake ();
		score.text = "0";
		newRecordMessage.SetActive(false);

		StartCoroutine(IncrementScore());
	}

	[SerializeField]
	GameObject newRecordMessage;

	IEnumerator IncrementScore() {

		int lastScore = PersistenceManager.Instance.LastScore;

		float t = 0;

		do {
			score.text = Mathf.RoundToInt(Mathf.Lerp(0, lastScore, t)).ToString();
			score.characterSize = Mathf.Lerp(minScoreSize, maxScoreSize, t);
			yield return null;

			t += Time.deltaTime;
		}
		while (t <= 1);

		score.text = lastScore.ToString();
		score.characterSize = maxScoreSize;
		yield return null;

		if(lastScore > PersistenceManager.Instance.MaxScore) {
			PersistenceManager.Instance.MaxScore = lastScore;
			yield return new WaitForSeconds(0.5f);
			newRecordMessage.SetActive(true);
		}

		yield return new WaitForSeconds(3f);

		Application.LoadLevel("start");
	}
}
