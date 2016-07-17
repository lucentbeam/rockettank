using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateScoreSheet : MonoBehaviour {

	public Text scores;
	public Text names;

	// Use this for initialization
	void Awake () {
		scores.text = "";

		setScoreList();
	}

	void Update() {
		setScoreList();
	}

	void setScoreList() {
		scores.text = "";
		names.text = "";
		foreach (UserScore s in ScoreManager.instance.topScores)
		{
			scores.text += s.userScore.ToString () + "\n";
			names.text += s.userName + "\n";
		}
	}
}
