using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateScoreSheet : MonoBehaviour {
	public Text scores;
	public Text names;

	// Use this for initialization
	void Awake () {
//		text.text = ScoreManager.scores.totalScore.ToString ();
		setScoreList();
	}

	void Update() {
		setScoreList();
	}

	void setScoreList() {
		scores.text = "";
		names.text = "";
		foreach (UserScore s in ScoreManager.scores.topScores)
		{
			scores.text += s.userScore.ToString () + "\n";
			names.text += s.userName + "\n";
		}
	}
}
