using MelonLoader;
using Il2CppRUMBLE.Managers;
using System;
using UnityEngine;
using RumbleModUI;
using System.Globalization;

namespace ColorScreen
{
    public static class BuildInfo
    {
        public const string ModName = "Color Screen";
        public const string ModVersion = "2.4.1";
        public const string Author = "UlvakSkillz";
    }
    public class main : MelonMod
	{
		private string currentScene = "Loader";
		private bool gKeyReleased = true;
		private bool gScreenActive = false;
		private bool fKeyReleased = true;
		private bool fPlainActive = false;
        private static string wallColorText = "FF44EC";
        private static string floorColorText = "FF44EC";
        private Color wallColor = hexToColor(wallColorText);
        private Color floorColor = hexToColor(floorColorText);
        private static Mod ColorScreen = new Mod();
		private Shader unlit;

        private void UIInit()
        {
            UI.instance.AddMod(ColorScreen);
        }

        public override void OnLateInitializeMelon()
        {
            unlit = Shader.Find("Universal Render Pipeline/Unlit");
            ColorScreen.ModName = BuildInfo.ModName;
            ColorScreen.ModVersion = BuildInfo.ModVersion;
            ColorScreen.SetFolder("ColorScreen");
            ColorScreen.AddToList("Wall Color", "FF44EC", "Sets The Wall Color to the Supplied Color", new Tags { });
            ColorScreen.AddToList("Floor Color", "FF44EC", "Sets The Floor Color to the Supplied Color", new Tags { });
            ColorScreen.GetFromFile();
            ColorScreen.ModSaved += Save;
            UI.instance.UI_Initialized += UIInit;
			Save();
		}

		private void Save()
		{
			wallColorText = (string)ColorScreen.Settings[0].SavedValue;
            floorColorText = (string)ColorScreen.Settings[1].SavedValue;
            if (wallColorText.Length < 6)
            {
                Log("Wall Color too Short!");
                wallColorText = "FF44EC";
                ColorScreen.Settings[0].SavedValue = "FFFFFF";
                ColorScreen.Settings[0].Value = "FFFFFF";
            }
            if (floorColorText.Length < 6)
            {
                Log("Floor Color too Short!");
                floorColorText = "FF44EC";
                ColorScreen.Settings[1].SavedValue = "000000";
                ColorScreen.Settings[1].Value = "000000";
            }
			wallColor = hexToColor(wallColorText);
			floorColor = hexToColor(floorColorText);
        }
        public static Color hexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, a);
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
				gKeyReleased = false;
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
            }
			if ((Input.GetKeyUp(KeyCode.G)) && (!gKeyReleased))
			{
				gKeyReleased = true;
			}
			if ((Input.GetKeyDown(KeyCode.F)) && (fKeyReleased))
			{
				fKeyReleased = false;
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
            }
			if ((Input.GetKeyUp(KeyCode.F)) && (!fKeyReleased))
			{
				fKeyReleased = true;
			}
		}

		//Creates the Screen
		private void CreateWall()
		{
			try
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.name = "ColorScreen";
				cube.GetComponent<Renderer>().material.shader = unlit;
				cube.GetComponent<MeshRenderer>().material.color = wallColor;
				PlayerManager playerManager = PlayerManager.instance;
				cube.transform.rotation = Quaternion.Euler(0, playerManager.localPlayer.Controller.gameObject.transform.GetChild(2).GetChild(0).GetChild(0).transform.rotation.eulerAngles.y, 0);
				cube.transform.position = playerManager.localPlayer.Controller.gameObject.transform.GetChild(2).GetChild(0).GetChild(0).transform.position;
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
				cube.GetComponent<Renderer>().material.shader = unlit;
				cube.GetComponent<MeshRenderer>().material.color = floorColor;
				cube.transform.position = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(2).GetChild(3).position;
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
