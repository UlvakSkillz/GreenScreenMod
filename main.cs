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
		private bool keyReleased = true;
		private bool keyPressed = false;
		private bool ScreenActive = false;
		private Vector3 color;
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
			if ((Input.GetKeyDown(KeyCode.G)) && (keyReleased))
			{
				keyPressed = true;
				keyReleased = false;
			}
			if ((Input.GetKeyUp(KeyCode.G)) && (!keyReleased))
			{
				keyReleased = true;
			}
		}

        public override void OnFixedUpdate()
		{
			if (sceneChanged)
			{
				if (currentScene == "Loader")
				{
					if (System.IO.File.Exists(settingsFile))
					{
						try
						{
							string[] fileContents = System.IO.File.ReadAllLines(settingsFile);
							color = new Vector3(Int32.Parse(fileContents[0]), Int32.Parse(fileContents[1]), Int32.Parse(fileContents[2]));
							MelonLogger.Msg($"Color Loaded | R: {color.x} | G: {color.y} | B: {color.z}");
						}
						catch
						{
							MelonLogger.Error($"Error Reading {settingsFile} | Setting to Blue");
							color = new Vector3(0, 0, 255);
						}
					}
					else
					{
						MelonLogger.Error($"File not Found: {settingsFile} | Setting to Blue");
						color = new Vector3(0, 0, 255);
					}
				}
				if (currentScene == "Gym")
                {
					material = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Telephone 2.0 REDUX special edition/Frame/Shell/Shell_Mesh").GetComponent<MeshRenderer>().material;
					shader = GameObject.Find("--------------LOGIC--------------/Heinhouser products/ShiftstoneCabinet/Cabinet/ShiftstoneBox/AdamantStone/Mesh").GetComponent<MeshRenderer>().material.shader;
				}
				sceneChanged = false;
            }
			if (keyPressed)
			{
				if (ScreenActive)
                {
					GameObject.Destroy(GameObject.Find("PinkScreen"));
					ScreenActive = false;
				}
                else
				{
                    try
					{
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.name = "PinkScreen";
						cube.GetComponent<MeshRenderer>().material = material;
						cube.GetComponent<MeshRenderer>().material.shader = shader;
						cube.GetComponent<MeshRenderer>().material.color = new Color(color.x / 255, color.y / 255, color.z / 255, 1);
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
					ScreenActive = true;
				}
				keyPressed = false;
			}
		}
	}
}
