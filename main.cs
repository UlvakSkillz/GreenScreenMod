﻿using MelonLoader;
using Il2CppRUMBLE.Managers;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace ColorScreenMod
{
	public class main : MelonMod
	{
		private string FILEPATH = @"UserData\ColorScreen";
		private string FILENAME = @"Color.txt";
		private string currentScene = "Loader";
		private bool gKeyReleased = true;
		private bool gKeyPressed = false;
		private bool gScreenActive = false;
		private bool fKeyReleased = true;
		private bool fKeyPressed = false;
		private bool fPlainActive = false;
		private Vector3 wallColor;
		private Vector3 floorColor;

		public override void OnLateInitializeMelon()
		{
			MelonCoroutines.Start(CheckIfFileExists(FILEPATH, FILENAME));
		}

		public IEnumerator CheckIfFileExists(string filePath, string fileName)
		{
			if (!File.Exists($"{filePath}\\{fileName}"))
			{
				if (!Directory.Exists(filePath))
				{
					Log($"Folder Not Found, Creating Folder: {filePath}");
					Directory.CreateDirectory(filePath);
				}
				if (!File.Exists($"{filePath}\\{fileName}"))
				{
					Log($"Creating File {filePath}\\{fileName}");
					File.Create($"{filePath}\\{fileName}");
				}
				wallColor = new Vector3(255, 68, 237);
				floorColor = new Vector3(255, 68, 237);
				for (int i = 0; i < 60; i++) { yield return new WaitForFixedUpdate(); }
				string[] newFileText = new string[8];
				newFileText[0] = "Screen Color:";
				newFileText[1] = "255";
				newFileText[2] = "68";
				newFileText[3] = "237";
				newFileText[4] = "Floor Color:";
				newFileText[5] = "255";
				newFileText[6] = "68";
				newFileText[7] = "237";
				File.WriteAllLines($"{filePath}\\{fileName}", newFileText);
            }
            else
			{
				try
				{
					string[] fileContents = File.ReadAllLines($"{filePath}\\{fileName}");
					wallColor = new Vector3(Int32.Parse(fileContents[1]), Int32.Parse(fileContents[2]), Int32.Parse(fileContents[3]));
					Log($"Wall Color Loaded | R: {wallColor.x} | G: {wallColor.y} | B: {wallColor.z}");
					floorColor = new Vector3(Int32.Parse(fileContents[5]), Int32.Parse(fileContents[6]), Int32.Parse(fileContents[7]));
					Log($"Floor Color Loaded | R: {floorColor.x} | G: {floorColor.y} | B: {floorColor.z}");
				}
				catch
				{
					MelonLogger.Error($"Error Reading {filePath}\\{fileName} | Setting to Blue");
					wallColor = new Vector3(0, 0, 255);
					floorColor = new Vector3(0, 0, 255);
				}
			}
			yield return null;
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			currentScene = sceneName;
		}

        public override void OnUpdate()
		{
			if ((currentScene == "Loader"))
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
				cube.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
				cube.GetComponent<MeshRenderer>().material.color = new Color(wallColor.x / 255, wallColor.y / 255, wallColor.z / 255, 1);
				GameObject localHealthBar = GameObject.Find("Health/Local");
				PlayerManager playerManager = PlayerManager.instance;
				cube.transform.rotation = playerManager.localPlayer.Controller.gameObject.transform.GetChild(5).GetChild(5).GetChild(0).transform.rotation;
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
				cube.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
				cube.GetComponent<MeshRenderer>().material.color = new Color(floorColor.x / 255, floorColor.y / 255, floorColor.z / 255, 1);
				GameObject localHealthBar = GameObject.Find("Health/Local");
				PlayerManager playerManager = PlayerManager.instance;
				cube.transform.position = playerManager.localPlayer.Controller.gameObject.transform.GetChild(5).GetChild(5).GetChild(0).gameObject.transform.position;
				cube.transform.position = new Vector3(cube.transform.position.x, cube.transform.position.y - 0.102f, cube.transform.position.z);
				cube.transform.localScale = new Vector3(1000, 0.01f, 1000);
				cube.SetActive(true);
			}
			catch (Exception e)
			{
				MelonLogger.Error(e);
			}
		}

		public static void Log(string msg)
		{
			MelonLogger.Msg(msg);
		}
	}
}
