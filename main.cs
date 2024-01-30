using MelonLoader;
using RUMBLE.Managers;
using System;
using UnityEngine;

namespace ColorScreenMod
{
	public class main : MelonMod
	{
		private string settingsFile = @"UserData\ColorScreen\Color.txt";
		public static main _instance;
		private string currentScene = "";
		private bool sceneChanged = false;
		private bool gKeyReleased = true;
		private bool gKeyPressed = false;
		private bool gScreenActive = false;
		private bool fKeyReleased = true;
		private bool fKeyPressed = false;
		private bool fPlainActive = false;
		private Vector3 wallColor;
		private Vector3 floorColor;
		Material material;
		Shader shader;

		public override void OnEarlyInitializeMelon()
		{
			_instance = this;
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			currentScene = sceneName;
			sceneChanged = true;
		}

        public override void OnUpdate()
		{
			if ((currentScene != "Park") && (currentScene != "Gym"))
			{
				return;
			}
			if ((Input.GetKeyDown(KeyCode.G)) && (gKeyReleased))
			{
				gKeyPressed = true;
				gKeyReleased = false;
			}
			if ((Input.GetKeyUp(KeyCode.G)) && (!gKeyReleased))
			{
				gKeyReleased = true;
			}
			if ((Input.GetKeyDown(KeyCode.F)) && (fKeyReleased))
			{
				fKeyPressed = true;
				fKeyReleased = false;
			}
			if ((Input.GetKeyUp(KeyCode.F)) && (!fKeyReleased))
			{
				fKeyReleased = true;
			}
		}

        public override void OnFixedUpdate()
		{
			if (sceneChanged)
			{
				if (currentScene == "Loader")
				{
					//read color File
					if (System.IO.File.Exists(settingsFile))
					{
						try
						{
							string[] fileContents = System.IO.File.ReadAllLines(settingsFile);
							wallColor = new Vector3(Int32.Parse(fileContents[1]), Int32.Parse(fileContents[2]), Int32.Parse(fileContents[3]));
							MelonLogger.Msg($"Wall Color Loaded | R: {wallColor.x} | G: {wallColor.y} | B: {wallColor.z}");
							floorColor = new Vector3(Int32.Parse(fileContents[5]), Int32.Parse(fileContents[6]), Int32.Parse(fileContents[7]));
							MelonLogger.Msg($"Floor Color Loaded | R: {floorColor.x} | G: {floorColor.y} | B: {floorColor.z}");
						}
						catch
						{
							MelonLogger.Error($"Error Reading {settingsFile} | Setting to Blue");
							wallColor = new Vector3(0, 0, 255);
							floorColor = new Vector3(0, 0, 255);
						}
					}
					else
					{
						MelonLogger.Error($"File not Found: {settingsFile} | Setting to Blue");
						wallColor = new Vector3(0, 0, 255);
						floorColor = new Vector3(0, 0, 255);
					}
				}
				if (currentScene == "Gym")
                {
					//save shader and material to use for in other scenes
					material = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Telephone 2.0 REDUX special edition/Frame/Shell/Shell_Mesh").GetComponent<MeshRenderer>().material;
					shader = GameObject.Find("--------------LOGIC--------------/Heinhouser products/ShiftstoneCabinet/Cabinet/ShiftstoneBox/AdamantStone/Mesh").GetComponent<MeshRenderer>().material.shader;
				}
				sceneChanged = false;
			}
			//if G Pressed
			if (gKeyPressed)
			{
				if (gScreenActive)
				{
					GameObject.Destroy(GameObject.Find("ColorScreen"));
					gScreenActive = false;
				}
				else
				{
					CreateWall();
					gScreenActive = true;
				}
				gKeyPressed = false;
			}
			//if F Pressed
			if (fKeyPressed)
			{
				if (fPlainActive)
				{
					GameObject.Destroy(GameObject.Find("ColorPlain"));
					fPlainActive = false;
				}
				else
				{
					CreatePlane();
					fPlainActive = true;
				}
				fKeyPressed = false;
			}
		}

		//Creates the Screen
		private void CreateWall()
		{
			try
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.name = "ColorScreen";
				cube.GetComponent<MeshRenderer>().material = material;
				cube.GetComponent<MeshRenderer>().material.shader = shader;
				cube.GetComponent<MeshRenderer>().material.color = new Color(wallColor.x / 255, wallColor.y / 255, wallColor.z / 255, 1);
				cube.GetComponent<MeshRenderer>().material.SetFloat("_METALLICSPECGLOSSMAP", 0f);
				GameObject localHealthBar = GameObject.Find("Health/Local");
				PlayerManager playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager").GetComponent<PlayerManager>();
				cube.transform.rotation = playerManager.localPlayer.Controller.gameObject.transform.GetChild(2).GetChild(12).transform.rotation;
				cube.transform.position = localHealthBar.transform.position;
				cube.transform.localScale = new Vector3(0.01f, 1000, 1000);
				cube.SetActive(true);
			}
			catch (Exception e)
			{
				MelonLogger.Error(e);
			}
		}

		//creates the Floor Plain
		private void CreatePlane()
		{
			try
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.name = "ColorPlain";
				cube.GetComponent<MeshRenderer>().material = material;
				cube.GetComponent<MeshRenderer>().material.shader = shader;
				cube.GetComponent<MeshRenderer>().material.color = new Color(floorColor.x / 255, floorColor.y / 255, floorColor.z / 255, 1);
				cube.GetComponent<MeshRenderer>().material.SetFloat("_METALLICSPECGLOSSMAP", 0f);
				GameObject localHealthBar = GameObject.Find("Health/Local");
				PlayerManager playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager").GetComponent<PlayerManager>();
				cube.transform.position = localHealthBar.transform.position;
				cube.transform.localScale = new Vector3(1000, 0.01f, 1000);
				cube.SetActive(true);
			}
			catch (Exception e)
			{
				MelonLogger.Error(e);
			}
		}
	}
}
