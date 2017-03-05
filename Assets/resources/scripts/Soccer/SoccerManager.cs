using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using obj = System.Object;

//Enums

public enum Click {Izquierdo, Derecho, Centro}
public enum Teams {Local, Away, Referee}
public enum Positions {None, Keeper, Defense, Midfield, Striker}
public enum Alignments {IV_III_III_A, IV_III_III_B, IV_V_I, IV_II_III_I, IV_IV_II_A, IV_IV_II_B, IV_I_II_I_II, III_II_IV, III_II_III_II, III_III_IV_A, III_III_IV_B, III_IV_II_A, III_IV_III_B, III_III_II_II, III_V_II, IV_II_IV_A, IV_II_IV_B, V_II_III_A, V_II_III_B, V_III_II, V_III_I_I, V_IV_I_A, V_IV_I_B, VI_III_I_A, VI_III_I_B, VII_II_I_A, VII_II_I_B, VIII_I_I_A, VIII_I_I_B}
public enum AlignType {None, A, B}

//Functions

public class SoccerManager : MonoBehaviour {

	public static SoccerManager sm;
	public static int LGs, AGs;

	public bool isLocalMatch = true;

	public List<Player> myTeam, enemyTeam;

	public float FocusOnClickRadius = 4, distanceBtPlayers = 25;

	public Click buttonToPass;

	public Alignments lAlign, aAlign;

	public int LLeader, ALeader;

	public Teams whichIsMine = Teams.Local;

	public Color myColor, awayColor;

	CamFollow cf;

	bool fullScreen;
	int W, H, wWidth, wHeight, i;

	//string Res;

	// Use this for initialization
	void Start() {
		sm = this;
		cf = Camera.main.GetComponent<CamFollow>();
		float cfM = cf.margin;
		cfM = (isLocalMatch == true) ? Mathf.Abs(cfM) : -Mathf.Abs(cfM);
		cf.margin = cfM;
		wWidth = Screen.width;
		wHeight = Screen.height;
		i = Screen.resolutions.Length - 1;
		InstantiatePlayers();
	}
	
	// Update is called once per frame
	void Update() {

		RaycastHit hit = new RaycastHit();
		Collider closestCollider = new Collider();

		if(Input.GetKeyDown(KeyCode.F)) {
			fullScreen = (fullScreen == false) ? true : false;
			Resolution[] resolutions = Screen.resolutions;
			W = (W == wWidth) ? resolutions[i].width : wWidth;
			H = (H == wHeight) ? resolutions[i].height : wHeight;
			Screen.SetResolution(W, H, fullScreen);
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		Player pFocused = GetPlayers().Where(x=> x.MyTeam == sm.whichIsMine).SingleOrDefault(x => x.isFocused == true);
		if(Input.GetMouseButtonDown((int)buttonToPass) && Physics.Raycast(ray, out hit, 1000) && pFocused != null && !pFocused.hasBall) {

			Collider[] colliders = Physics.OverlapSphere(hit.point, FocusOnClickRadius);

			foreach(Collider hit1 in colliders) {
				//checks if it's hitting itself
				if(hit1.collider == transform.collider)
				{
					continue;
				}
				if(closestCollider == null)
				{
					closestCollider = hit1;
				}
				//compares distances
				if(Vector3.Distance(hit.point, hit1.transform.position) <= Vector3.Distance(hit.point, closestCollider.transform.position))
				{
					closestCollider = hit1;
				}
			}

			if(closestCollider.transform.GetComponent<Player>() != null) {
				foreach(Player player in myTeam) {
					player.isFocused = false;
				}
				closestCollider.transform.GetComponent<Player>().isFocused = true;
			}

		}

		if(Input.GetKeyDown(KeyCode.Escape)) {
			Screen.showCursor = (Screen.showCursor == false) ? true : false;
		}

	}

	public static Player[] GetPlayers(obj nulteam = null) {

		List<Player> team = sm.myTeam;

		if(nulteam != null) {
			team = (List<Player>)nulteam;
		}

		return team.ToArray();

	}

	public static void EditPlayer(int pos, Player newvalue, Teams team = Teams.Local) {
		if(team == Teams.Local) {
			try {
				Player[] playerList = sm.myTeam.ToArray();
				playerList[pos] = newvalue;
				sm.myTeam = playerList.ToList();
			} catch(Exception ex) {
				Debug.Log(ex.Message);
			}
		} else if(team == Teams.Away) {
			try {
				Player[] playerList = sm.enemyTeam.ToArray();
				playerList[pos] = newvalue;
				sm.enemyTeam = playerList.ToList();
			} catch(Exception ex) {
				Debug.Log(ex.Message);
			}
		}
	}

	public static Player FindPlayer(int pos, Teams team = Teams.Local, Positions ppos = Positions.None) {
		if(team == Teams.Local) {
			if(ppos == Positions.None) {
				try {
					return sm.myTeam.ToArray()[pos];
				} catch(Exception ex) {
					Debug.Log(ex.Message + ", Length: "+sm.myTeam.ToArray().Length+", Pos: "+pos+", pPos: "+ppos.ToString());
					return null;
				}
			} else {
				try {
					return sm.myTeam.ToArray().Where(x => x.myPosition == ppos).Single(x => x.playerNum == pos+1);
				} catch(Exception ex) {
					Debug.Log(ex.Message + " " + ", Pos: "+(pos+1)+", pPos: "+ppos.ToString());
					return null;
				}
			}
		} else if(team == Teams.Away) {
			if(ppos == Positions.None) {
				try {
					return sm.enemyTeam.ToArray()[pos];
				} catch(Exception ex) {
					Debug.Log(ex.Message + " " + ", Pos: "+pos+", pPos: "+ppos.ToString());
					return null;
				}
			} else {
				try {
					return sm.enemyTeam.ToArray().Where(x => x.myPosition == ppos).Single(x => x.playerNum == pos+1);
				} catch(Exception ex) {
					Debug.Log(ex.Message + " " + ", Pos: "+(pos+1)+", pPos: "+ppos.ToString());
					return null;
				}
			}
		}
		return null;
	}

	public static void InstantiatePlayers() {
		int nDef, nMid, nStr;
		string myAlign = sm.lAlign.ToString(), awayAlign = sm.aAlign.ToString();
		string[] splitMA = myAlign.Split('_'), splitAA = awayAlign.Split('_');
		for(int i = 0; i < 11; i++) {
			GameObject p = (GameObject)Instantiate(Resources.Load("models/Player"), new Vector3(0, 1.05f, 0), Quaternion.identity);
			p.transform.name = "Player "+(i+1);
			Player player = p.GetComponent<Player>();
			Movement mPlayer = p.GetComponent<Movement>();
			mPlayer.onlyClick = true;
			if(i == sm.LLeader) {
				player.isLeader = true;
				if(sm.whichIsMine == Teams.Local) {
					player.isFocused = true;
				}
			}
			player.MyTeam = Teams.Local;
			player.playerNum = i+1;
			nDef = RomanNum.ConvertRomanNumeralToInt(splitMA[0]);
			nMid = RomanNum.ConvertRomanNumeralToInt(splitMA[1]);
			nStr = RomanNum.ConvertRomanNumeralToInt(splitMA[2]);
			if(i == 0) {
				player.myPosition = Positions.Keeper;
				player.transform.position = new Vector3(0, 1.05f, -69);
			} else if(i >= 1 && i < nDef+1) {
				player.myPosition = Positions.Defense;
				player.transform.position = new Vector3(((i-1) - Mathf.CeilToInt(nDef/2) + ((nDef % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, -40);
			} else if(i >= nDef+1 && i < nMid+nDef+1) {
				player.myPosition = Positions.Midfield;
				player.transform.position = new Vector3(((i-nDef-1) - Mathf.CeilToInt(nMid/2) + ((nMid % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, -10);
			} else if(i >= nMid+nDef+1 && i < nStr+nMid+nDef+1) {
				player.myPosition = Positions.Striker;
				player.transform.position = new Vector3(((i-nDef-nMid-1) - Mathf.CeilToInt(nStr/2) + ((nStr % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, 25);
			}
			player.myPos = p.transform.position;
			p.transform.FindChild("Graphics").renderer.material.color = sm.myColor;
			sm.myTeam.Add(player);
		}
		for(int i = 0; i < 11; i++) {
			GameObject p = (GameObject)Instantiate(Resources.Load("models/Player"), new Vector3(0, 1.05f, 0), Quaternion.identity);
			p.transform.name = "aPlayer "+(i+1);
			Player player = p.GetComponent<Player>();
			Movement mPlayer = p.GetComponent<Movement>();
			mPlayer.onlyClick = true;
			if(i == sm.ALeader) {
				player.isLeader = true;
				if(sm.whichIsMine == Teams.Away) {
					player.isFocused = true;
				}
			}
			player.MyTeam = Teams.Away;
			player.playerNum = i+1;
			nDef = RomanNum.ConvertRomanNumeralToInt(splitAA[0]);
			nMid = RomanNum.ConvertRomanNumeralToInt(splitAA[1]);
			nStr = RomanNum.ConvertRomanNumeralToInt(splitAA[2]);
			if(i == 0) {
				player.myPosition = Positions.Keeper;
				player.transform.position = new Vector3(0, 1.05f, 69);
			} else if(i >= 1 && i < nDef+1) {
				player.myPosition = Positions.Defense;
				player.transform.position = new Vector3(((i-1) - Mathf.CeilToInt(nDef/2) + ((nDef % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, 35);
			} else if(i >= nDef+1 && i < nMid+nDef+1) {
				player.myPosition = Positions.Midfield;
				player.transform.position = new Vector3(((i-nDef-1) - Mathf.CeilToInt(nMid/2) + ((nMid % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, 10);
			} else if(i >= nMid+nDef+1 && i < nStr+nMid+nDef+1) {
				player.myPosition = Positions.Striker;
				player.transform.position = new Vector3(((i-nDef-nMid-1) - Mathf.CeilToInt(nStr/2) + ((nStr % 2 == 0) ? 0.5f : 0))*sm.distanceBtPlayers, 1.05f, -25);
			}
			player.myPos = p.transform.position;
			p.transform.FindChild("Graphics").renderer.material.color = sm.awayColor;
			sm.enemyTeam.Add(player);
		}
	}

}
