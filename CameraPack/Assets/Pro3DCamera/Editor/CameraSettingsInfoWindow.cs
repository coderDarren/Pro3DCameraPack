using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Pro3DCamera {

	public class CameraSettingsInfoWindow : EditorWindow {

		Texture2D backTex;
        Texture2D topBarTex;
        Color backgroundColor = new Color(0 / 255f, 28 / 255f, 33 / 255f);
        Color topBarColor = new Color(2 / 255f, 88 / 255f, 104 / 255f);
        GUISkinData skin;

        Rect contentRect;
        Rect topBarRect;
        Rect contentImageRect;

        static EditorWindow window;
        static string info;

		public static void OpenWindow(string _title, string _info)
        {
            window = GetWindow(typeof(CameraSettingsInfoWindow));
            window.ShowPopup();
            info = _info;
        }

        void OnEnable()
        {
            backTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            backTex.SetPixel(0, 0, backgroundColor);
            backTex.Apply();

            topBarTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            topBarTex.SetPixel(0, 0, topBarColor);
            topBarTex.Apply();

            InitStyles();
        }

        void OnGUI()
        {
            GUI.changed = false;

            if (!backTex)
            {
                backTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
                backTex.SetPixel(0, 0, backgroundColor);
                backTex.Apply();
            }

            if (!topBarTex)
            {
                topBarTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
                topBarTex.SetPixel(0, 0, topBarColor);
                topBarTex.Apply();
            }

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), backTex);
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, Constants.MENU_TOPBAR_HEIGHT), topBarTex);

            contentRect = new Rect(Constants.CONTENT_PADDING,
                                   Constants.CONTENT_PADDING + Constants.MENU_TOPBAR_HEIGHT,
                                   Screen.width - Constants.CONTENT_PADDING - Constants.CONTENT_PADDING,
                                   Screen.height - Constants.CONTENT_PADDING - Constants.CONTENT_PADDING - Constants.MENU_TOPBAR_HEIGHT);

            topBarRect = new Rect(Constants.SIDEBAR_PADDING / 4,
                                  0,
                                  Screen.width - Constants.MENU_TOPBAR_HEIGHT,
                                  Constants.MENU_TOPBAR_HEIGHT);

		
            UpdateWindow();
		}

		void UpdateWindow()
		{
			#region Top Bar
            GUILayout.BeginArea(topBarRect);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("", infoButton, GUILayout.Width(24), GUILayout.Height(26));
            GUILayout.Label(titleContent, skin.skin.customStyles[0]);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            #region Content
            GUILayout.BeginArea(contentRect);
            GUILayout.Label(info, skin.skin.customStyles[3]);
            if (GUILayout.Button("OK", skin.skin.button, GUILayout.Height(24)))
            	window.Close();
            GUILayout.EndArea();
            #endregion
		}

		void InitStyles()
		{

			infoIcon = (Texture2D)Resources.Load("info_icon", typeof(Texture2D));
            infoButton = InitButtonStyle(infoIcon);

			skin = (GUISkinData)Resources.Load("Data/Style/EditorSkin_Dark", typeof(GUISkinData));
		}

		GUIStyle InitButtonStyle(Texture2D icon)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = icon;
            style.margin = new RectOffset(5, 5, 5, 5);
            style.active.background = icon;
            style.focused.background = icon;
            style.hover.background = icon;
            return style;
        }

		Texture2D infoIcon;
		GUIStyle infoButton;

	}

}