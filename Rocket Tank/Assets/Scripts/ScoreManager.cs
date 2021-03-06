﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class UserScore {
	public UserScore() : this("ASS", 0) {}
	public UserScore(string name, int score) {
		userName = name;
		userScore = score;
	}
	public string userName = "ASS";
	public int userScore = 0;
}

public class ScoreManager : MonoBehaviour {

	public static ScoreManager instance;

	public int totalScore;
	[SerializeField]
	public List<UserScore> topScores;

	void Awake () {
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
			instance.resetScore ();
			instance.load ();
		} else if (instance != this.gameObject) {
			Destroy (gameObject);
		}
	}

	void increaseScore(int value)
	{
		instance.totalScore += value;
	}

	void resetScore()
	{
		instance.totalScore = 0;
	}

	public void save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (!File.Exists (Application.persistentDataPath + "/scores.dat")) {
			file = File.Create (Application.persistentDataPath + "/scores.dat");
		} else {
			file = File.Open(Application.persistentDataPath + "/scores.dat", FileMode.Truncate);
		}
		bf.Serialize (file, instance.topScores);
		file.Close ();
	}

	public void load()
	{
		if (!File.Exists (Application.persistentDataPath + "/scores.dat")) {
			return;
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.OpenRead (Application.persistentDataPath + "/scores.dat");

		topScores = (List<UserScore>)bf.Deserialize (file);
		topScores.Sort (delegate(UserScore c1, UserScore c2) {
			return c2.userScore.CompareTo (c1.userScore);
		});
		while (topScores.Count>10) 
			topScores.RemoveAt(topScores.Count-1);
		file.Close ();

	}

	void submitScore(string userName)
	{
		UserScore data = new UserScore (userName, instance.totalScore);
		instance.topScores.Add (data);
		topScores.Sort (delegate(UserScore c1, UserScore c2) {
			return c2.userScore.CompareTo (c1.userScore);
		});
		while (topScores.Count>10) 
			topScores.RemoveAt(topScores.Count-1);
		instance.resetScore ();
	}
}
