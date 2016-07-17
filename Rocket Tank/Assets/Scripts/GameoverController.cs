using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LetterCollection {

	public char[] letters = { 'a', 'b', 'c' };
	public int selected = 0;

	public string getName() { 
		return letters [0].ToString () + letters [1].ToString () + letters [2].ToString ();
	}

	public void nextLetter() {
		selected++;
		selected %= 3;
	}

	public void previousLetter() {
		selected = selected-- >= 0 ? selected : 2;
	}

	public void incrementUp() {
		int newVal = System.Convert.ToUInt16 (letters [selected]);
		newVal++;
		newVal = (newVal > 96+26) ? newVal - 26 : newVal; 
		letters [selected] = (char)newVal;
	}

	public void incrementDown() {
		int newVal = System.Convert.ToUInt16 (letters [selected]);
		newVal--;
		newVal = newVal > 96 ? newVal : 96+26;
		letters [selected] = (char)newVal;

	}
}

public class GameoverController : MonoBehaviour {

	public Text scoreText;
	public Image overlay;

	private LetterCollection userName;
	// Use this for initialization
	void Start () {
		userName = new LetterCollection ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
