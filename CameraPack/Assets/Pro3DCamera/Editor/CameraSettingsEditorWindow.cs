using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Pro3DCamera {
    public static class Constants
    {
        public static string RPG_DATA_PATH = "Data/RPG";
        public static string FPS_DATA_PATH = "Data/FPS";
        public static string RTS_DATA_PATH = "Data/RTS";
        public static string TOPDOWN_DATA_PATH = "Data/TopDown";
        public static string SHAKE_DATA_PATH = "Data/CameraShakeData";
        public static string PLAYER_CONTROLLER_DATA_PATH = "Data/PlayerControllerData";
        public static string OBSTRUCTION_HANDLER_DATA_PATH = "Data/ObstructionHandlerData";

        public static string LAST_RPG = "LastRPGSet";
        public static string LAST_FPS = "LastFPSSet";
        public static string LAST_RTS = "LastRTSSet";
        public static string LAST_TOPDOWN = "LastTopDownSet";

        public static string ACTIVE_AWAKE_CAM = "ActiveAwakeCam";

        public static float CONTENT_PADDING = 24;
        public static float SIDEBAR_PADDING = 8;

        public static float CONTENT_IMAGE_SIZE = 50;

        public static float MENU_SIDEBAR_WIDTH = 100;
        public static float MENU_TOPBAR_HEIGHT = 36;

        public static float CONTENT_FIELD_POSITION_X = MENU_SIDEBAR_WIDTH + CONTENT_PADDING;
        public static float CONTENT_FIELD_POSITION_Y = MENU_TOPBAR_HEIGHT + CONTENT_PADDING;

        public static float SIDEBAR_CONTENT_RECT_HEIGHT = 1000;
        public static float CONTENT_BOTTOM_BAR_HEIGHT = 50;
        public static Rect SIDEBAR_CONTENT_RECT = new Rect(SIDEBAR_PADDING,
                                                            SIDEBAR_PADDING,
                                                            MENU_SIDEBAR_WIDTH - SIDEBAR_PADDING * 2,
                                                            SIDEBAR_CONTENT_RECT_HEIGHT);

    }

    public class CameraSettingsEditorWindow : EditorWindow {

        Texture2D backTex;
        Texture2D sideBarTex;
        Texture2D sideBarTopTex;
        Texture2D topBarTex;
        Color backgroundColor = new Color(0 / 255f, 28 / 255f, 33 / 255f);
        Color sideBarColor = new Color(1 / 255f, 22 / 255f, 26 / 255f);
        Color sideBarTopColor = new Color(3 / 255f, 37 / 255f, 43 / 255f);
        Color topBarColor = new Color(2 / 255f, 88 / 255f, 104 / 255f);
        GUISkinData skin;

        Rect contentRect;
        Rect topBarButtonRect;
        Rect bottomBarRect;
        Rect topBarRect;
        Rect contentImageRect;
        float contentWidth;

        enum ActivePage
        {
            RPG,
            FPS,
            RTS,
            TOP_DOWN,
            SHAKE,
            PLAYER_CONTROLLER,
            OBSTRUCTION
        }
        ActivePage activePage;

        enum ActiveSubPage
        {
            POSITION,
            ORBIT,
            INPUT,
            MOBILE,
            DEBUG,
            PAN
        }
        ActiveSubPage activeSubPage;

        enum ActiveGeneralSubpage
        {
            SHAKE
        }
        ActiveGeneralSubpage activeGenSubPage;

        Object[] rpgDatas, fpsDatas, rtsDatas, topDownDatas;
        string[] rpgDataNames, fpsDataNames, rtsDataNames, topDownDataNames;
        int selectedRPGData, selectedFPSData, selectedRTSData, selectedTopDownData;

        public static void OpenWindow()
        {
            EditorWindow window = GetWindow(typeof(CameraSettingsEditorWindow));
            window.ShowPopup();
        }

        void OnEnable()
        {
            backTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            backTex.SetPixel(0, 0, backgroundColor);
            backTex.Apply();

            sideBarTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            sideBarTex.SetPixel(0, 0, sideBarColor);
            sideBarTex.Apply();

            sideBarTopTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            sideBarTopTex.SetPixel(0, 0, sideBarTopColor);
            sideBarTopTex.Apply();

            topBarTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
            topBarTex.SetPixel(0, 0, topBarColor);
            topBarTex.Apply();

            RefreshDataLists();

            selectedRPGData = PlayerPrefs.GetInt(Constants.LAST_RPG);
            if (selectedRPGData < 0 || selectedRPGData > rpgDatas.Length)
                selectedRPGData = 0;
            selectedFPSData = PlayerPrefs.GetInt(Constants.LAST_FPS);
            if (selectedFPSData < 0 || selectedFPSData > fpsDatas.Length)
                selectedFPSData = 0;
            selectedRTSData = PlayerPrefs.GetInt(Constants.LAST_RTS);
            if (selectedRTSData < 0 || selectedRTSData > rtsDatas.Length)
                selectedRTSData = 0;
            selectedTopDownData = PlayerPrefs.GetInt(Constants.LAST_TOPDOWN);
            if (selectedTopDownData < 0 || selectedTopDownData > topDownDatas.Length)
                selectedTopDownData = 0;

            InitStyles();
            RefreshDataLists();

            int awakeID = PlayerPrefs.GetInt(Constants.ACTIVE_AWAKE_CAM);
            switch (awakeID)
            {
                case 0: awakeCam = CameraControl.CameraType.RPG; break;
                case 1: awakeCam = CameraControl.CameraType.FPS; break;
                case 2: awakeCam = CameraControl.CameraType.RTS; break;
                case 3: awakeCam = CameraControl.CameraType.TOP_DOWN; break;
            }
        }
        
        void OnDisable()
        {
            PlayerPrefs.SetInt(Constants.LAST_RPG, selectedRPGData);
            PlayerPrefs.SetInt(Constants.LAST_FPS, selectedFPSData);
            PlayerPrefs.SetInt(Constants.LAST_RTS, selectedRTSData);
            PlayerPrefs.SetInt(Constants.LAST_TOPDOWN, selectedTopDownData);
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

            if (!sideBarTex)
            {
                sideBarTex = new Texture2D(1, 1, TextureFormat.RGB24, false);
                sideBarTex.SetPixel(0, 0, sideBarColor);
                sideBarTex.Apply();
            }

            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), backTex);
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, Constants.MENU_TOPBAR_HEIGHT), topBarTex);
            GUI.DrawTexture(new Rect(0, 0, Constants.MENU_SIDEBAR_WIDTH, maxSize.y), sideBarTex);

            contentRect = new Rect(Constants.CONTENT_FIELD_POSITION_X,
                                   Constants.CONTENT_FIELD_POSITION_Y,
                                   Screen.width - Constants.CONTENT_FIELD_POSITION_X - Constants.CONTENT_PADDING,
                                   Screen.height - Constants.CONTENT_FIELD_POSITION_Y - Constants.CONTENT_PADDING - Constants.CONTENT_BOTTOM_BAR_HEIGHT);

            contentImageRect = new Rect((Screen.width - Constants.MENU_SIDEBAR_WIDTH) / 2 - Constants.CONTENT_IMAGE_SIZE / 1f,
                                        Constants.CONTENT_PADDING*1.8f,
                                        Constants.CONTENT_IMAGE_SIZE,
                                        Constants.CONTENT_IMAGE_SIZE);

            bottomBarRect = new Rect(Constants.MENU_SIDEBAR_WIDTH + Constants.CONTENT_PADDING,
                                     Screen.height - Constants.CONTENT_BOTTOM_BAR_HEIGHT,
                                     Screen.width - Constants.MENU_SIDEBAR_WIDTH - Constants.CONTENT_PADDING * 2,
                                     Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING);

            topBarRect = new Rect(Constants.MENU_SIDEBAR_WIDTH + Constants.SIDEBAR_PADDING / 4,
                                  Constants.SIDEBAR_PADDING / 3.5f,
                                  Screen.width - Constants.MENU_SIDEBAR_WIDTH - Constants.MENU_TOPBAR_HEIGHT,
                                  Constants.MENU_TOPBAR_HEIGHT);

            contentWidth = Screen.width - Constants.MENU_SIDEBAR_WIDTH - Constants.CONTENT_PADDING * 2;


            UpdateSideBar();

            switch (activePage)
            {
                case ActivePage.RPG: UpdateRPGPage(); break;
                case ActivePage.FPS: UpdateFPSPage(); break;
                case ActivePage.RTS: UpdateRTSPage(); break;
                case ActivePage.TOP_DOWN: UpdateTopDownPage(); break;
                case ActivePage.SHAKE: UpdateShakePage(); break;
                case ActivePage.PLAYER_CONTROLLER: UpdatePlayerControllerPage(); break;
                case ActivePage.OBSTRUCTION: UpdateObstructionHandlerPage(); break;
            }

            if (GUI.changed)
            {
                RefreshDataLists();
                GUI.changed = false;
            }
        }

        CameraControl.CameraType awakeCam;
        Vector2 sideBarScrollPos;
        void UpdateSideBar()
        {
            GUILayout.BeginArea(Constants.SIDEBAR_CONTENT_RECT);
            EditorGUILayout.BeginHorizontal();

            sideBarScrollPos = EditorGUILayout.BeginScrollView(sideBarScrollPos, GUILayout.Height(Screen.height - Constants.SIDEBAR_PADDING*2));

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("", rpgButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING*4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.RPG;
                rpgButton = InitButtonStyle(rpgIconSelected);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", fpsButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.FPS;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIconSelected);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", rtsButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.RTS;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIconSelected);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", topDownButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.TOP_DOWN;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIconSelected);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", generalButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.SHAKE;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIconSelected);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", obstructionButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.OBSTRUCTION;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIconSelected);
                playerButton = InitButtonStyle(playerIcon);
            }
            if (GUILayout.Button("", playerButton, GUILayout.Height(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4), GUILayout.Width(Constants.MENU_SIDEBAR_WIDTH - Constants.SIDEBAR_PADDING * 4)))
            {
                activePage = ActivePage.PLAYER_CONTROLLER;
                rpgButton = InitButtonStyle(rpgIcon);
                fpsButton = InitButtonStyle(fpsIcon);
                rtsButton = InitButtonStyle(rtsIcon);
                topDownButton = InitButtonStyle(topDownIcon);
                generalButton = InitButtonStyle(generalIcon);
                obstructionButton = InitButtonStyle(obstructionIcon);
                playerButton = InitButtonStyle(playerIconSelected);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        CameraData rpgData;
        CameraControl cameraObject = null;
        string cameraObjectTag = "MainCamera";
        int awakeID;
        void UpdateRPGPage()
        {

            #region Content

            if (!rpgData)
            {
                GUILayout.BeginArea(contentRect);
                GUILayout.Label("No Camera Data could be found. Make sure you have an RPG Data object located at Resources/Data/RPG", skin.skin.customStyles[3]);
                GUILayout.EndArea();
            }
            else
            {
                #region Top Bar
                GUILayout.BeginArea(topBarRect);
                GUILayout.Label("Data source: " + rpgDatas[selectedRPGData].name, skin.skin.customStyles[0]);
                GUILayout.EndArea();
                #endregion

                GUILayout.BeginArea(contentRect);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Awake Camera", "The camera that is used on startup. The camera will pull data from whichever preset is active in the editor.");
                GUILayout.Label("Awake Cam ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                awakeID = (int)awakeCam;
                awakeCam = (CameraControl.CameraType)EditorGUILayout.EnumPopup(awakeCam);
                EditorGUILayout.EndHorizontal();
                switch (awakeCam)
                {
                    case CameraControl.CameraType.RPG: awakeID = 0; break;
                    case CameraControl.CameraType.FPS: awakeID = 1; break;
                    case CameraControl.CameraType.RTS: awakeID = 2; break;
                    case CameraControl.CameraType.TOP_DOWN: awakeID = 3; break;
                }
                PlayerPrefs.SetInt(Constants.ACTIVE_AWAKE_CAM, awakeID);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Camera Tag", "Select the tag of the camera you will be using. This will give the editor information about the camera when it needs to apply certain settings.");
                GUILayout.Label("Camera Tag", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                cameraObjectTag = EditorGUILayout.TagField(cameraObjectTag);
                EditorGUILayout.EndHorizontal();
                try {
                    cameraObject = GameObject.FindGameObjectWithTag(cameraObjectTag).GetComponent<CameraControl>();
                }
                catch {
                    GUILayout.Label("No camera with the tag '"+cameraObjectTag+"' with the CameraControl component could be found.\nMake sure the camera exists - and that it has the CameraControl component.", skin.skin.customStyles[3]);
                    GUILayout.EndArea();
                    return;
                }

                if (activeSubPage == ActiveSubPage.MOBILE || activeSubPage == ActiveSubPage.PAN)
                {
                    activeSubPage = ActiveSubPage.POSITION;
                    positionButton = InitButtonStyle(positionIconSelected);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIcon);
                }
                
                DrawSettingsTitle();
                UpdateSubMenu();
                
                switch (activeSubPage)
                {
                    case ActiveSubPage.POSITION: UpdatePositionSettings(ref rpgData.pos); break;
                    case ActiveSubPage.ORBIT: UpdateOrbitSettings(ref rpgData.orbit); break;
                    case ActiveSubPage.INPUT: UpdateInputSettings(ref rpgData.input); break;
                    case ActiveSubPage.DEBUG: UpdateDebugSettings(ref rpgData); break;
                }

                GUILayout.EndArea();
            }

            #endregion

            #region Bottom Bar
            GUILayout.BeginArea(bottomBarRect);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth));
            GUILayout.Label("Choose Data Preset", skin.skin.label);
            selectedRPGData = EditorGUILayout.Popup(selectedRPGData, rpgDataNames);
            if (rpgDatas.Length > 0)
                rpgData = (CameraData)rpgDatas[selectedRPGData];
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            EditorUtility.SetDirty(rpgData);
        }

        CameraData fpsData;
        void UpdateFPSPage()
        {
            #region Content

            if (!fpsData)
            {
                GUILayout.BeginArea(contentRect);
                GUILayout.Label("No Camera Data could be found. Make sure you have an FPS Data object located at Resources/Data/FPS", skin.skin.customStyles[3]);
                GUILayout.EndArea();
            }
            else
            {
                #region Top Bar
                GUILayout.BeginArea(topBarRect);
                GUILayout.Label("Data source: " + fpsDatas[selectedFPSData].name, skin.skin.customStyles[0]);
                GUILayout.EndArea();
                #endregion

                GUILayout.BeginArea(contentRect);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Awake Camera", "The camera that is used on startup. The camera will pull data from whichever preset is active in the editor.");
                GUILayout.Label("Awake Cam ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                awakeID = (int)awakeCam;
                awakeCam = (CameraControl.CameraType)EditorGUILayout.EnumPopup(awakeCam);
                EditorGUILayout.EndHorizontal();
                switch (awakeCam)
                {
                    case CameraControl.CameraType.RPG: awakeID = 0; break;
                    case CameraControl.CameraType.FPS: awakeID = 1; break;
                    case CameraControl.CameraType.RTS: awakeID = 2; break;
                    case CameraControl.CameraType.TOP_DOWN: awakeID = 3; break;
                }
                PlayerPrefs.SetInt(Constants.ACTIVE_AWAKE_CAM, awakeID);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Camera Tag", "Select the tag of the camera you will be using. This will give the editor information about the camera when it needs to apply certain settings.");
                GUILayout.Label("Camera Tag", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                cameraObjectTag = EditorGUILayout.TagField(cameraObjectTag);
                EditorGUILayout.EndHorizontal();
                try {
                    cameraObject = GameObject.FindGameObjectWithTag(cameraObjectTag).GetComponent<CameraControl>();
                }
                catch {
                    GUILayout.Label("No camera with the tag '" + cameraObjectTag + "' with the CameraControl component could be found.\nMake sure the camera exists - and that it has the CameraControl component.", skin.skin.customStyles[3]);
                    GUILayout.EndArea();
                    return;
                }
                
                DrawSettingsTitle();
                UpdateSubMenu();

                if (activeSubPage != ActiveSubPage.ORBIT && activeSubPage != ActiveSubPage.POSITION)
                {
                    activeSubPage = ActiveSubPage.POSITION;
                    positionButton = InitButtonStyle(positionIconSelected);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIcon);
                }

                switch (activeSubPage)
                {
                    case ActiveSubPage.POSITION: UpdateFPSPosSettings(); break;
                    case ActiveSubPage.ORBIT: UpdateFPSOrbitSettings(); break;
                }

                GUILayout.EndArea();
            }
            
            #endregion

            #region Bottom Bar
            GUILayout.BeginArea(bottomBarRect);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth));
            GUILayout.Label("Choose Data Preset", skin.skin.label);
            selectedFPSData = EditorGUILayout.Popup(selectedFPSData, fpsDataNames);
            if (fpsDatas.Length > 0)
                fpsData = (CameraData)fpsDatas[selectedFPSData];
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            EditorUtility.SetDirty(fpsData);
        }

        CameraData rtsData;
        void UpdateRTSPage()
        {
            #region Content

            if (!rtsData)
            {
                GUILayout.BeginArea(contentRect);
                GUILayout.Label("No Camera Data could be found. Make sure you have an RTS Data object located at Resources/Data/RTS", skin.skin.customStyles[3]);
                GUILayout.EndArea();
            }
            else
            {
                #region Top Bar
                GUILayout.BeginArea(topBarRect);
                GUILayout.Label("Data source: " + rtsDatas[selectedRTSData].name, skin.skin.customStyles[0]);
                GUILayout.EndArea();
                #endregion

                GUILayout.BeginArea(contentRect);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Awake Camera", "The camera that is used on startup. The camera will pull data from whichever preset is active in the editor.");
                GUILayout.Label("Awake Cam ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                awakeID = (int)awakeCam;
                awakeCam = (CameraControl.CameraType)EditorGUILayout.EnumPopup(awakeCam);
                EditorGUILayout.EndHorizontal();
                switch (awakeCam)
                {
                    case CameraControl.CameraType.RPG: awakeID = 0; break;
                    case CameraControl.CameraType.FPS: awakeID = 1; break;
                    case CameraControl.CameraType.RTS: awakeID = 2; break;
                    case CameraControl.CameraType.TOP_DOWN: awakeID = 3; break;
                }
                PlayerPrefs.SetInt(Constants.ACTIVE_AWAKE_CAM, awakeID);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Camera Tag", "Select the tag of the camera you will be using. This will give the editor information about the camera when it needs to apply certain settings.");
                GUILayout.Label("Camera Tag", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                cameraObjectTag = EditorGUILayout.TagField(cameraObjectTag);
                EditorGUILayout.EndHorizontal();
                try {
                    cameraObject = GameObject.FindGameObjectWithTag(cameraObjectTag).GetComponent<CameraControl>();
                }
                catch {
                    GUILayout.Label("No camera with the tag '" + cameraObjectTag + "' with the CameraControl component could be found.\nMake sure the camera exists - and that it has the CameraControl component.", skin.skin.customStyles[3]);
                    GUILayout.EndArea();
                    return;
                }

                if (activeSubPage == ActiveSubPage.DEBUG)
                {
                    activeSubPage = ActiveSubPage.POSITION;
                    positionButton = InitButtonStyle(positionIconSelected);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIcon);
                }
                
                DrawSettingsTitle();
                UpdateSubMenu();

                switch (activeSubPage)
                {
                    case ActiveSubPage.POSITION: UpdatePositionSettings(ref rtsData.pos); break;
                    case ActiveSubPage.ORBIT: UpdateOrbitSettings(ref rtsData.orbit); break;
                    case ActiveSubPage.INPUT: UpdateInputSettings(ref rtsData.input); break;
                    //case ActiveSubPage.DEBUG: UpdateDebugSettings(ref rtsData.debug); break;
                    //case ActiveSubPage.MOBILE: UpdateMobileSettings(); break;
                    case ActiveSubPage.PAN: UpdatePanSettings(); break;
                }

                GUILayout.EndArea();
            }

            #endregion

            #region Bottom Bar
            GUILayout.BeginArea(bottomBarRect);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth));
            GUILayout.Label("Choose Data Preset", skin.skin.label);
            selectedRTSData = EditorGUILayout.Popup(selectedRTSData, rtsDataNames);
            if (rtsDatas.Length > 0)
                rtsData = (CameraData)rtsDatas[selectedRTSData];
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            EditorUtility.SetDirty(rtsData);
        }

        CameraData topDownData;
        void UpdateTopDownPage()
        {
            #region Content
            
            if (!topDownData)
            {
                GUILayout.BeginArea(contentRect);
                GUILayout.Label("No Camera Data could be found. Make sure you have an TopDown Data object located at Resources/Data/TopDown", skin.skin.customStyles[3]);
                GUILayout.EndArea();
            }
            else
            {
                #region Top Bar
                GUILayout.BeginArea(topBarRect);
                GUILayout.Label("Data source: " + topDownDatas[selectedTopDownData].name, skin.skin.customStyles[0]);
                GUILayout.EndArea();
                #endregion

                GUILayout.BeginArea(contentRect);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Awake Camera", "The camera that is used on startup. The camera will pull data from whichever preset is active in the editor.");
                GUILayout.Label("Awake Cam ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                awakeID = (int)awakeCam;
                awakeCam = (CameraControl.CameraType)EditorGUILayout.EnumPopup(awakeCam);
                EditorGUILayout.EndHorizontal();
                switch (awakeCam)
                {
                    case CameraControl.CameraType.RPG: awakeID = 0; break;
                    case CameraControl.CameraType.FPS: awakeID = 1; break;
                    case CameraControl.CameraType.RTS: awakeID = 2; break;
                    case CameraControl.CameraType.TOP_DOWN: awakeID = 3; break;
                }
                PlayerPrefs.SetInt(Constants.ACTIVE_AWAKE_CAM, awakeID);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                InfoButton("Camera Tag", "Select the tag of the camera you will be using. This will give the editor information about the camera when it needs to apply certain settings.");
                GUILayout.Label("Camera Tag", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                cameraObjectTag = EditorGUILayout.TagField(cameraObjectTag);
                EditorGUILayout.EndHorizontal();
                try {
                    cameraObject = GameObject.FindGameObjectWithTag(cameraObjectTag).GetComponent<CameraControl>();
                }
                catch {
                    GUILayout.Label("No camera with the tag '" + cameraObjectTag + "' with the CameraControl component could be found.\nMake sure the camera exists - and that it has the CameraControl component.", skin.skin.customStyles[3]);
                    GUILayout.EndArea();
                    return;
                }

                if (activeSubPage == ActiveSubPage.MOBILE || activeSubPage == ActiveSubPage.PAN || activeSubPage == ActiveSubPage.DEBUG)
                {
                    activeSubPage = ActiveSubPage.POSITION;
                    positionButton = InitButtonStyle(positionIconSelected);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIcon);
                }

                DrawSettingsTitle();
                UpdateSubMenu();

                switch (activeSubPage)
                {
                    case ActiveSubPage.POSITION: UpdatePositionSettings(ref topDownData.pos); break;
                    case ActiveSubPage.ORBIT: UpdateOrbitSettings(ref topDownData.orbit); break;
                    case ActiveSubPage.INPUT: UpdateInputSettings(ref topDownData.input); break;
                    //case ActiveSubPage.DEBUG: UpdateDebugSettings(ref topDownData); break;
                }

                GUILayout.EndArea();
            }

            #endregion

            #region Bottom Bar
            GUILayout.BeginArea(bottomBarRect);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth));
            GUILayout.Label("Choose Data Preset", skin.skin.label);
            selectedTopDownData = EditorGUILayout.Popup(selectedTopDownData, topDownDataNames);
            if (topDownDatas.Length > 0)
                topDownData = (CameraData)topDownDatas[selectedTopDownData];
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            EditorUtility.SetDirty(topDownData);
        }

        Rect modifiedContentRect;
        void UpdateShakePage()
        {
            modifiedContentRect = contentRect;
            modifiedContentRect.height = Screen.height - Constants.CONTENT_PADDING - Constants.CONTENT_BOTTOM_BAR_HEIGHT;
            GUILayout.BeginArea(modifiedContentRect);

            DrawGeneralSettingsTitle();
            UpdateShakeSettings();

            GUILayout.EndArea();
        }

        void UpdatePlayerControllerPage()
        {
            GUILayout.BeginArea(contentRect);

            DrawGeneralSettingsTitle();
            UpdatePlayerControllerSettings();

            GUILayout.EndArea();
        }

        void UpdateObstructionHandlerPage()
        {
            GUILayout.BeginArea(contentRect);

            DrawGeneralSettingsTitle();
            UpdateObstructionSettings();

            GUILayout.EndArea();
        }

        Vector2 settingsScrollPos;
        void UpdatePositionSettings(ref CameraData.PositionSet pos)
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            if (activePage != ActivePage.TOP_DOWN && activePage != ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Camera Offset X", "Dictates the horizontal offset of the camera's look position relative to the target.");
                GUILayout.Label("Camera Offset X ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.targetPosOffset.x = EditorGUILayout.Slider(pos.targetPosOffset.x, -10, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Camera Offset Y", "Dictates the vertical offset of the camera's look position relative to the target.");
                GUILayout.Label("Camera Offset Y ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.targetPosOffset.y = EditorGUILayout.Slider(pos.targetPosOffset.y, -10, 10);
                EditorGUILayout.EndHorizontal();
            }

            if (activePage == ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Initial Position", "Gives an initial position for the RTS camera.");
                GUILayout.Label("Initial Position ", skin.skin.label);
                GUILayout.Label("X", skin.skin.label);
                pos.initialPos.x = EditorGUILayout.FloatField(pos.initialPos.x);
                GUILayout.Label("Y", skin.skin.label);
                pos.initialPos.y = EditorGUILayout.FloatField(pos.initialPos.y);
                GUILayout.Label("Z", skin.skin.label);
                pos.initialPos.z = EditorGUILayout.FloatField(pos.initialPos.z);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Zoom Feature", "Enabling this provides options for zooming in and out on the target.");
            GUILayout.Label("Use Zoom Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            pos.allowZoom = EditorGUILayout.Toggle(pos.allowZoom);
            EditorGUILayout.EndHorizontal();
            if (pos.allowZoom)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Zoom Speed", "How fast does the camera zoom through each zoom increment?");
                GUILayout.Label("Zoom Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.zoomSmooth = EditorGUILayout.Slider(pos.zoomSmooth, 0.1f, 100);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Zoom Increment", "For each zoom input, how far should the camera zoom? Think of this like a zoom sensitivity, but not speed.");
                GUILayout.Label("Zoom Increment ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.zoomStep = EditorGUILayout.Slider(pos.zoomStep, 0.1f, 100);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Max Zoom", "The closest position allowed, relative to the target.");
                GUILayout.Label("Max Zoom ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.maxZoom = EditorGUILayout.Slider(pos.maxZoom, (activePage == ActivePage.RPG) ? fpsData.pos.minZoom + 1 : 0f, 200);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Min Zoom", "The furthest position allowed, relative to the target.");
                GUILayout.Label("Min Zoom ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.minZoom = EditorGUILayout.Slider(pos.minZoom, pos.maxZoom + 1, (activePage == ActivePage.FPS) ? rpgData.pos.maxZoom - 1 : 200f);
                EditorGUILayout.EndHorizontal();

                if (activePage == ActivePage.RPG || activePage == ActivePage.FPS)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Zoom Transition", "Allows the camera to transition back and forth between FPS and RPG camera types via zooming.");
                    GUILayout.Label((activePage == ActivePage.RPG)?"Zoom To FPS Camera ":"Zoom Out To RPG Camera ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    rpgData.pos.rpgFpsTransition = EditorGUILayout.Toggle(rpgData.pos.rpgFpsTransition);
                    EditorGUILayout.EndHorizontal();
                    fpsData.pos.rpgFpsTransition = rpgData.pos.rpgFpsTransition;
                }
                if (fpsData.pos.rpgFpsTransition)
                    fpsData.pos.allowZoom = true;
                if (rpgData.pos.rpgFpsTransition)
                    rpgData.pos.allowZoom = true;
            }

            if (activePage != ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Smooth Follow Feature", "Gives the camera some drag when following the target.");
                GUILayout.Label("Use Smooth Follow Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.smoothFollow = EditorGUILayout.Toggle(pos.smoothFollow);
                EditorGUILayout.EndHorizontal();
                if (pos.smoothFollow)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Smooth Time", "The time taken for the camera to reach it's destination position.");
                    GUILayout.Label("Smooth Time ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    pos.smooth = EditorGUILayout.Slider(pos.smooth, 0.01f, 2);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Boundary Feature", "Provides means for restricting the RTS camera to a box region.");
                GUILayout.Label("Use Boundary Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                pos.useBoundary = EditorGUILayout.Toggle(pos.useBoundary);
                EditorGUILayout.EndHorizontal();
                if (pos.useBoundary)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("X-Axis Boundary", "The threshold the camera is permitted to move to on the X axis.");
                    GUILayout.Label("Min Bounds X ", skin.skin.label);
                    pos.minBoundary.x = EditorGUILayout.FloatField(pos.minBoundary.x);
                    GUILayout.Label("Max Bounds X ", skin.skin.label);
                    pos.maxBoundary.x = EditorGUILayout.FloatField(pos.maxBoundary.x);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Z-Axis Boundary", "The threshold the camera is permitted to move to on the Z axis.");
                    GUILayout.Label("Min Bounds Z ", skin.skin.label);
                    pos.minBoundary.y = EditorGUILayout.FloatField(pos.minBoundary.y);
                    GUILayout.Label("Max Bounds Z ", skin.skin.label);
                    pos.maxBoundary.y = EditorGUILayout.FloatField(pos.maxBoundary.y);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Elastic Boundary", "Gives the camera a bounce threshold when traveling beyond the permissible boundary region.");
                    GUILayout.Label("Use Elastic Boundary ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    pos.useElasticBoundary = EditorGUILayout.Toggle(pos.useElasticBoundary);
                    EditorGUILayout.EndHorizontal();
                    if (pos.useElasticBoundary)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                        InfoButton("Elasticity", "The threshold the camera is permitted to travel beyond the boundary region.");
                        GUILayout.Label("Elasticity ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                        pos.boundaryElasticity = EditorGUILayout.Slider(pos.boundaryElasticity, 0.01f, 100);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Ground Layer", "The layer the RTS camera looks for when determining a distance to maintain.");
                GUILayout.Label("Ground Layer", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                var serializedObject = new SerializedObject(rtsData);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("groundLayer"), new GUIContent(""));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        void UpdateOrbitSettings(ref CameraData.OrbitSet orbit)
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            if (activePage != ActivePage.TOP_DOWN && activePage != ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Max X Angle ", "The highest angle the camera can rotate 'up and down' around the player.");
                GUILayout.Label("Max X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.maxXRotation = EditorGUILayout.Slider(orbit.maxXRotation, 0, 89);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Min X Angle", "The lowest angle the camera can rotate 'up and down' around the player.");
                GUILayout.Label("Min X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.minXRotation = EditorGUILayout.Slider(orbit.minXRotation, -89, orbit.maxXRotation);
                EditorGUILayout.EndHorizontal();
            }
            if (activePage == ActivePage.TOP_DOWN || activePage == ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("X Angle", "On the top down and RTS cameras, the angle at which the camera looks down on the environment or the target.");
                GUILayout.Label("X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.xRotation = EditorGUILayout.Slider(orbit.xRotation, 40, 89);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Use Orbit Feature", "Toggles the orbit feature, which allows the player to orbit the camera around the target.");
            GUILayout.Label("Orbit Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            orbit.allowOrbit = EditorGUILayout.Toggle(orbit.allowOrbit);
            EditorGUILayout.EndHorizontal();
            if (orbit.allowOrbit)
            {
                if (activePage != ActivePage.TOP_DOWN && activePage != ActivePage.RTS)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("X Orbit Speed", "The speed at which the camera rotates around the target on the X axis.");
                    GUILayout.Label("X Orbit Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    orbit.xOrbitSmooth = EditorGUILayout.Slider(orbit.xOrbitSmooth, 0.1f, 200);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Y Orbit Speed", "The speed at which the camera rotates around the target on the Y axis.");
                GUILayout.Label("Y Orbit Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.yOrbitSmooth = EditorGUILayout.Slider(orbit.yOrbitSmooth, 0.1f, 200);
                EditorGUILayout.EndHorizontal();
            }

            if (activePage != ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Orbit With Target", "Enabling this keeps the camera aligned with the 'Default X Angle' and 'Default Y Angle'.");
                GUILayout.Label("Orbit With Target ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.rotateWithTarget = EditorGUILayout.Toggle(orbit.rotateWithTarget);
                EditorGUILayout.EndHorizontal();
            }

            if (activePage == ActivePage.RPG || activePage == ActivePage.TOP_DOWN)
            {
                if (activePage == ActivePage.RPG)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Auto Return X Angle", "Enabling this will automatically align the camera's X angle according to 'Default X Angle'.");
                    GUILayout.Label("Auto Return X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    orbit.alwaysFindXAngle = EditorGUILayout.Toggle(orbit.alwaysFindXAngle);
                    EditorGUILayout.EndHorizontal();
                    if (orbit.alwaysFindXAngle)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                        InfoButton("Default X Angle", "The X angle the camera will automatically move to.");
                        GUILayout.Label("Default X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                        orbit.defaultXAngle = EditorGUILayout.Slider(orbit.defaultXAngle, -40, 89);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                        InfoButton("X Return Time", "The time it takes for the camera to move from it's current X angle to 'Default X Angle'.");
                        GUILayout.Label("X Return Time ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                        orbit.timeToRevertX = EditorGUILayout.Slider(orbit.timeToRevertX, 0.01f, 2);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Default X Angle", "info");
                    GUILayout.Label("Default X Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    orbit.defaultXAngle = EditorGUILayout.Slider(orbit.defaultXAngle, -40, 89);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Auto Return Y Angle", "Enabling this will automatically align the camera's Y angle according to 'Default Y Angle'.");
                GUILayout.Label("Auto Return Y Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                orbit.alwaysFindYAngle = EditorGUILayout.Toggle(orbit.alwaysFindYAngle);
                EditorGUILayout.EndHorizontal();
                if (orbit.alwaysFindYAngle)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Default Y Angle ", "The Y angle the camera will automatically move to.");
                    GUILayout.Label("Default Y Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    orbit.defaultYAngle = EditorGUILayout.Slider(orbit.defaultYAngle, 0, 359);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Y Return Time", "The time it takes for the camera to move from it's current Y angle to 'Default Y Angle'.");
                    GUILayout.Label("Y Return Time ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    orbit.timeToRevertY = EditorGUILayout.Slider(orbit.timeToRevertY, 0.01f, 2);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void UpdateInputSettings(ref CameraData.InputSet input)
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Orbit Input", "The KeyCode that provides input to orbit the camera around the target.");
            GUILayout.Label("Orbit Input ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            input.MOUSE_ORBIT = (CameraData.InputOption)EditorGUILayout.EnumPopup(input.MOUSE_ORBIT);
            EditorGUILayout.EndHorizontal();
            if (activePage == ActivePage.RTS)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Pan Input", "The KeyCode that provides input to pan the camera around the environment.");
                GUILayout.Label("Pan Input ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                input.PAN = (CameraData.InputOption)EditorGUILayout.EnumPopup(input.PAN);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Zoom Input", "The string value Input Axis name that provides input to zoom in and out on the target.");
            GUILayout.Label("Zoom Input ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            input.ZOOM = EditorGUILayout.TextField(input.ZOOM);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        void UpdateDebugSettings(ref CameraData data)
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Use Collision Feature ", "Enable if you want the camera to respond to collision and occlusion events.");
            GUILayout.Label("Use Collision Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            data.debug.useCollision = EditorGUILayout.Toggle(data.debug.useCollision);
            EditorGUILayout.EndHorizontal();
            if (data.debug.useCollision)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Collision Layer", "The camera will collide with objects on these layers. Objects on these layers will not occlude the target.");
                GUILayout.Label("Collision Layer", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                var serializedObject = new SerializedObject(data);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionLayer"), new GUIContent(""));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Collision Smooth Time", "The time taken for the camera to reach it's destination position while colliding with an object.");
                GUILayout.Label("Collision Smooth Time ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.pos.collisionSmooth = EditorGUILayout.FloatField(data.pos.collisionSmooth);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Collision Padding", "When colliding, the camera will move forward an additional amount according to this value.");
                GUILayout.Label("Collision Padding ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.collisionPadding = EditorGUILayout.Slider(data.debug.collisionPadding, 0.01f, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Close Quarters Feature", "If enabled, the camera will move up and look down on the target when the target's back is against the wall.");
                GUILayout.Label("Use Close Quarters Feature", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.useCloseQuartersTechnique = EditorGUILayout.Toggle(data.debug.useCloseQuartersTechnique);
                EditorGUILayout.EndHorizontal();
                if (data.debug.useCloseQuartersTechnique)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Close Quarters Distance", "The distance the camera is away from the target before the camera moves upward.");
                    GUILayout.Label("Close Quarters Distance ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    data.debug.closeQuartersDistance = EditorGUILayout.Slider(data.debug.closeQuartersDistance, 0.01f, 10);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Close Quarters Height", "The height the camera moves to when close quarters is activated.");
                    GUILayout.Label("Close Quarters Height ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    data.debug.closeQuartersHeightAdjust = EditorGUILayout.Slider(data.debug.closeQuartersHeightAdjust, 1, 50);
                    EditorGUILayout.EndHorizontal();
                }
                HorizontalLine(contentWidth * 0.94f);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Draw Desired Collision Lines", "Enable to view the desired collision lines from the editor view. Desired collision lines represent the location of the camera with no collision.");
                GUILayout.Label("Draw Desired Collision Lines ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.drawDesiredCollisionLines = EditorGUILayout.Toggle(data.debug.drawDesiredCollisionLines);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Draw Adjusted Collision Lines", "Enable to view the adjusted collision lines from the editor view. Adjusted collision lines represent the location of the camera while colliding.");
                GUILayout.Label("Draw Adjusted Collision Lines ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.drawAdjustedCollisionLines = EditorGUILayout.Toggle(data.debug.drawAdjustedCollisionLines);
                EditorGUILayout.EndHorizontal();
            }
            HorizontalLine(contentWidth * 0.94f);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Close Fade Feature", "If enabled, the camera's target will fade to your desired alpha when the camera is within a certain distance. This feature only works on targets whose materials use the Standard Shader - or a variation of it.");
            GUILayout.Label("Use Close Fade Feature", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            data.debug.useCloseQuartersFade = EditorGUILayout.Toggle(data.debug.useCloseQuartersFade);
            EditorGUILayout.EndHorizontal();
            if (data.debug.useCloseQuartersFade)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Fade At Distance", "The distance the camera must be to the target before the target fades.");
                GUILayout.Label("Fade At Distance ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.distanceToFadeTarget = EditorGUILayout.Slider(data.debug.distanceToFadeTarget, 0.1f, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Fade Alpha", "The opacity level of the target when the camera comes within the specified range.");
                GUILayout.Label("Fade Alpha ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                data.debug.targetFadeAlpha = EditorGUILayout.Slider(data.debug.targetFadeAlpha, 0f, 0.99f);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        /*
        void UpdateMobileSettings()
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 11));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
            GUILayout.Label("Use Mobile Input Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
            rtsData.mobile.useMobileInput = EditorGUILayout.Toggle(rtsData.mobile.useMobileInput);
            EditorGUILayout.EndHorizontal();
            if (rtsData.mobile.useMobileInput)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                GUILayout.Label("Min Pinch Delta ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                rtsData.mobile.minimumPinchDelta = EditorGUILayout.FloatField(rtsData.mobile.minimumPinchDelta);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
                GUILayout.Label("Min Turn Angle ", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                rtsData.mobile.minimumTurnAngle = EditorGUILayout.FloatField(rtsData.mobile.minimumTurnAngle);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        */

        void UpdatePanSettings()
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Pan Speed", "The distance the camera travels when responding to pan input.");
            GUILayout.Label("Pan Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            rtsData.pos.panSmooth = EditorGUILayout.Slider(rtsData.pos.panSmooth, 0.1f, 500);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Pan Drag", "The time taken for the camera to reach the desired pan destination.");
            GUILayout.Label("Pan Drag ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            rtsData.pos.panDrag = EditorGUILayout.Slider(rtsData.pos.panDrag, 0.01f, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Invert Pan Direction", "Enable this to invert the pan direction. When enabled, the camera will pan in the opposite direction of the input. i.e. If dragging right, the camera will move left.");
            GUILayout.Label("Invert Pan Direction ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            rtsData.pos.invertPan = EditorGUILayout.Toggle(rtsData.pos.invertPan);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        void UpdateFPSPosSettings()
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Camera Offset Y", "Dictates the vertical offset of the camera’s position, relative to the target.");
            GUILayout.Label("Camera Offset Y ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.pos.targetPosOffset.y = EditorGUILayout.Slider(fpsData.pos.targetPosOffset.y, -10, 10);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Use Bounce Feature", "Enable this if you want to use head bobbing when running.");
            GUILayout.Label("Use Bounce Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.useBounce = EditorGUILayout.Toggle(fpsData.useBounce);
            EditorGUILayout.EndHorizontal();
            if (fpsData.useBounce)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Bounce Frequency", "The speed of the head bob.");
                GUILayout.Label("Bounce Frequency ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.bounceFrequency = EditorGUILayout.Slider(fpsData.bounceFrequency, 0.01f, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Bounce Amplitude", "The intensity of the head bob.");
                GUILayout.Label("Bounce Amplitude ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.bounceAmplitude = EditorGUILayout.Slider(fpsData.bounceAmplitude, 0.01f, 10);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Use Zoom Feature", "Enabling this provides options for zooming in and out on the target.");
            GUILayout.Label("Use Zoom Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.pos.allowZoom = EditorGUILayout.Toggle(fpsData.pos.allowZoom);
            EditorGUILayout.EndHorizontal();
            if (fpsData.pos.allowZoom)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Zoom Speed", "How fast does the camera zoom through each zoom increment?");
                GUILayout.Label("Zoom Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.pos.zoomSmooth = EditorGUILayout.Slider(fpsData.pos.zoomSmooth, 0.1f, 100);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Zoom Increment", "For each zoom input, how far should the camera zoom? Think of this like a zoom sensitivity, but not speed.");
                GUILayout.Label("Zoom Increment ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.pos.zoomStep = EditorGUILayout.Slider(fpsData.pos.zoomStep, 0.1f, 100);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Max Zoom", "The closest position allowed, relative to the target.");
                GUILayout.Label("Max Zoom ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.pos.maxZoom = EditorGUILayout.Slider(fpsData.pos.maxZoom, 0f, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Min Zoom", "The furthest position allowed, relative to the target.");
                GUILayout.Label("Min Zoom ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.pos.minZoom = EditorGUILayout.Slider(fpsData.pos.minZoom, fpsData.pos.maxZoom + 1, 10);
                EditorGUILayout.EndHorizontal();

                if (activePage == ActivePage.RPG || activePage == ActivePage.FPS)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Zoom To RPG Camera", "Allows the camera to transition back and forth between FPS and RPG camera types via zooming.");
                    GUILayout.Label((activePage == ActivePage.RPG) ? "Zoom To FPS Camera " : "Zoom Out To RPG Camera ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    rpgData.pos.rpgFpsTransition = EditorGUILayout.Toggle(rpgData.pos.rpgFpsTransition);
                    EditorGUILayout.EndHorizontal();
                    fpsData.pos.rpgFpsTransition = rpgData.pos.rpgFpsTransition;
                }
                if (fpsData.pos.rpgFpsTransition)
                    fpsData.pos.allowZoom = true;
                if (rpgData.pos.rpgFpsTransition)
                    rpgData.pos.allowZoom = true;
            }
            EditorGUILayout.EndScrollView();
        }

        void UpdateFPSOrbitSettings()
        {
            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 13));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("X Sensitivity", "The speed the camera will look left and right.");
            GUILayout.Label("X Sensitivity ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.XSensitivity = EditorGUILayout.Slider(fpsData.XSensitivity, 10, 500);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Y Sensitivity", "The speed the camera will look up and down.");
            GUILayout.Label("Y Sensitivity ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.YSensitivity = EditorGUILayout.Slider(fpsData.YSensitivity, 10, 500);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Smooth Look Feature", "Enable this if you do not want the look speed to be instantaneous.");
            GUILayout.Label("Use Smooth Look Feature ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            fpsData.smoothLook = EditorGUILayout.Toggle(fpsData.smoothLook);
            EditorGUILayout.EndHorizontal();
            if (fpsData.smoothLook)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Smooth Look Speed", "The speed the camera looks around in response to input.");
                GUILayout.Label("Smooth Look Speed ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                fpsData.smoothLookTime = EditorGUILayout.Slider(fpsData.smoothLookTime, 0.01f, 30);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        CameraShakeData shakeData;
        Rect addButtonRect;
        void UpdateShakeSettings()
        {
            if (!shakeData)
            {
                GUILayout.Label("Unable to retrieve camera shake data.", skin.skin.label);
                return;
            }

            addButtonRect = new Rect((Screen.width - Constants.MENU_SIDEBAR_WIDTH) / 2 - 42,
                                        Constants.CONTENT_PADDING*7.5f,
                                        32,
                                        32);

            GUILayout.Label("Add New Sequence", skin.skin.customStyles[3]);

            GUILayout.BeginArea(addButtonRect);

            if (GUILayout.Button("", addButton, GUILayout.Width(32), GUILayout.Height(32)))
            {
                shakeData.shakeSequences.Add(new CameraShakeData.ShakeSequence());
                GUILayout.EndArea();
                return;
            }
            else {
                GUILayout.EndArea();
            }

            GUILayout.Space(36);

            HorizontalLine(contentWidth * 0.94f);

            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 9));


            for (int i = shakeData.shakeSequences.Count-1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));

                if (GUILayout.Button(shakeData.shakeSequences[i].name, skin.skin.button, GUILayout.Width(contentWidth / 2f)))
                {
                    shakeData.shakeSequences[i].editing = !shakeData.shakeSequences[i].editing;
                    return;
                }
                
                if (GUILayout.Button("", removeButton, GUILayout.Height(24), GUILayout.Width(24)))
                {
                    shakeData.shakeSequences.RemoveAt(i);
                    return;
                }
                if (GUILayout.Button("", duplicateButton, GUILayout.Height(24), GUILayout.Width(24)))
                {
                    CameraShakeData.ShakeSequence seq = new CameraShakeData.ShakeSequence();
                    shakeData.shakeSequences[i].CopyFromTo(ref seq);
                    shakeData.shakeSequences.Add(seq);
                    return;
                }
                if (Application.isPlaying)
                    CameraShakeButton(shakeData.shakeSequences[i].name);
                EditorGUILayout.EndHorizontal();

                if (shakeData.shakeSequences[i].editing)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Shake Name", "The name of this shake sequence. This name is what will be called via the API when you want to shake the camera with this sequence.");
                    GUILayout.Label("Shake Name ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].name = EditorGUILayout.TextField(shakeData.shakeSequences[i].name);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Shake Duration ", "The length of time this shake sequence will execute for.");
                    GUILayout.Label("Shake Duration ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].duration = EditorGUILayout.FloatField(shakeData.shakeSequences[i].duration);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Shake Intensity ", "The multiplier of the X and Y curves when shaking the camera.");
                    GUILayout.Label("Shake Intensity ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].intensity = EditorGUILayout.FloatField(shakeData.shakeSequences[i].intensity);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Intensity Decay", "The rate at which the intensity decreases. Higher values will make it appear as though the sequence ends early.");
                    GUILayout.Label("Intensity Decay ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].decay = EditorGUILayout.FloatField(shakeData.shakeSequences[i].decay);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Pos X Curve", "The curve that the camera's X position will loop through for the duration of the sequence.");
                    GUILayout.Label("Pos X Curve ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].curve_posX = EditorGUILayout.CurveField(shakeData.shakeSequences[i].curve_posX);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Pos Y Curve", "The curve that the camera's Y position will loop through for the duration of the sequence.");
                    GUILayout.Label("Pos Y Curve ", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
                    shakeData.shakeSequences[i].curve_posY = EditorGUILayout.CurveField(shakeData.shakeSequences[i].curve_posY);
                    EditorGUILayout.EndHorizontal();

                }
            }

            EditorGUILayout.EndScrollView();

            EditorUtility.SetDirty(shakeData);
        }

        PlayerControllerData playerControllerData;
        void UpdatePlayerControllerSettings()
        {
            if (!playerControllerData)
            {
                GUILayout.Label("Unable to retrieve player controller data.", skin.skin.label);
                return;
            }

            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 9));

            PlayerControllerData.MoveSettings _move = playerControllerData.moveSetting;
            PlayerControllerData.PhysSettings _phys = playerControllerData.physSetting;
            PlayerControllerData.InputSettings _input = playerControllerData.inputSetting;

            GUILayout.Label("Move Settings", skin.skin.customStyles[3]);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Forward Velocity", "The speed that the player moves on it's Z axis.");
            GUILayout.Label("Forward Velocity", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _move.forwardVel = EditorGUILayout.FloatField(_move.forwardVel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Turn Velocity", "The speed that the player turns left and right.");
            GUILayout.Label("Turn Velocity", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _move.rotateVel = EditorGUILayout.FloatField(_move.rotateVel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Jump Velocity", "The speed with which the player jumps.");
            GUILayout.Label("Jump Velocity", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _move.jumpVel = EditorGUILayout.FloatField(_move.jumpVel);
            EditorGUILayout.EndHorizontal();


            playerControllerData.moveSetting = _move;

            HorizontalLine(contentWidth * 0.94f);
            GUILayout.Label("Physics Settings", skin.skin.customStyles[3]);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Gravity", "The acceleration rate at which the player is pulled toward the ground when airborne.");
            GUILayout.Label("Gravity", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _phys.downAccel = EditorGUILayout.FloatField(_phys.downAccel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Run Angle Limit", "The maximum angle the player can run against. A good value for this is 140.");
            GUILayout.Label("Run Angle Limit", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _phys.runAngleLimit = EditorGUILayout.Slider(_phys.runAngleLimit, 100, 180);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Capsule Radius", "Used to determine if the player is grounded. This value should be slightly lower than the capsule collider radius that is on your player. This value will be different based on your player model.");
            GUILayout.Label("Capsule Radius", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _phys.capsuleRadius = EditorGUILayout.FloatField(_phys.capsuleRadius);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Ground Layer", "Objects on this layer will set the player to grounded if the player is on them.");
            GUILayout.Label("Ground Layer", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            var serializedObject = new SerializedObject(playerControllerData);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("physSetting.ground"), new GUIContent(""));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            playerControllerData.physSetting = _phys;

            HorizontalLine(contentWidth * 0.94f);
            GUILayout.Label("Input Settings", skin.skin.customStyles[3]);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Input Delay", "This is the delay from when a button is pressed to when a response is triggered.");
            GUILayout.Label("Input Delay", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _input.inputDelay = EditorGUILayout.Slider(_input.inputDelay, 0.01f, 0.2f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Forward Axis", "The string value from input settings that will move the player forward.");
            GUILayout.Label("Forward Axis", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _input.FORWARD_AXIS = EditorGUILayout.TextField(_input.FORWARD_AXIS);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Turn Axis", "The string value from input settings that will turn the player left and right.");
            GUILayout.Label("Turn Axis", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _input.TURN_AXIS = EditorGUILayout.TextField(_input.TURN_AXIS);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Jump Axis", "The string value from input settings that will allow the player to jump.");
            GUILayout.Label("Jump Axis", skin.skin.label, GUILayout.Width(contentWidth / 2.8f));
            _input.JUMP_AXIS = EditorGUILayout.TextField(_input.JUMP_AXIS);
            EditorGUILayout.EndHorizontal();

            playerControllerData.inputSetting = _input;

            EditorGUILayout.EndScrollView();
            EditorUtility.SetDirty(playerControllerData);
        }

        ObstructionHandlerData obstructionData;
        void UpdateObstructionSettings()
        {   
            if (!obstructionData)
            {
                GUILayout.Label("Unable to retrieve obstruction handler data.", skin.skin.label);
                return;
            }

            settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos, GUILayout.Height(Screen.height - Constants.MENU_TOPBAR_HEIGHT - Constants.CONTENT_BOTTOM_BAR_HEIGHT - Constants.CONTENT_PADDING * 9));

            ObstructionHandlerData.ObstructionSettings _obstruction = obstructionData.obstructionSet;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
            InfoButton("Use Obstruction Handler", "Enable if you want objects to fade out when they occlude the camera's target.");
            GUILayout.Label("Use Obstruction Handler", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
            _obstruction.active = EditorGUILayout.Toggle(_obstruction.active);
            EditorGUILayout.EndHorizontal();

            if (_obstruction.active)

            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Obstruction Layer", "Objects on this layer will fade out when the player is occluded by them. To work, these objects will need to use the Standard Shader (or an extension of it).");
                GUILayout.Label("Obstruction Layer", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                var serializedObject = new SerializedObject(obstructionData);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("obstructionSet.obstructionLayer"), new GUIContent(""));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
    
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Obstruction Fade Speed", "The speed at which objects fade when they occlude the player from the camera view.");
                GUILayout.Label("Obstruction Fade Speed", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                _obstruction.obstructionFadeSmooth = EditorGUILayout.Slider(_obstruction.obstructionFadeSmooth, 0.01f, 20);
                EditorGUILayout.EndHorizontal();
    
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Minimum Obstruction Alpha", "The alpha of the object's shader color property when fading out. Lower values will make the obstructions easier to see through.");
                GUILayout.Label("Minimum Obstruction Alpha", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                _obstruction.minObstructionAlpha = EditorGUILayout.Slider(_obstruction.minObstructionAlpha, 0.01f, 0.99f);
                EditorGUILayout.EndHorizontal();
    
                EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                InfoButton("Change Target Color", "Enable if you want the camera's target to change color when objects occlude him.");
                GUILayout.Label("Change Target Color", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                _obstruction.changeTargetColor = EditorGUILayout.Toggle(_obstruction.changeTargetColor);
                EditorGUILayout.EndHorizontal();

                if (_obstruction.changeTargetColor)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Target Obstructed Color", "The color of the target when objects occlude him.");
                    GUILayout.Label("Target Obstructed Color", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                    _obstruction.obstructedColor = EditorGUILayout.ColorField(_obstruction.obstructedColor);
                    EditorGUILayout.EndHorizontal();
        
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Target Color Intensity", "The intensity of the color property of the target's occluded color.");
                    GUILayout.Label("Target Color Intensity", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                    _obstruction.colorIntensity = EditorGUILayout.Slider(_obstruction.colorIntensity, 0.01f, 10);
                    EditorGUILayout.EndHorizontal();
            
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.94f));
                    InfoButton("Target Fade Speed", "The speed that the target's color changes when occluded.");
                    GUILayout.Label("Target Fade Speed", skin.skin.label, GUILayout.Width(contentWidth / 2.1f));
                    _obstruction.targetFadeSmooth = EditorGUILayout.Slider(_obstruction.targetFadeSmooth, 0.01f, 20);
                    EditorGUILayout.EndHorizontal();
                }
            }

            obstructionData.obstructionSet = _obstruction;

            EditorGUILayout.EndScrollView();
            EditorUtility.SetDirty(obstructionData);
        }

        void CameraShakeButton(string _shakeID)
        {
            if (cameraObject != null)
            {
                if (GUILayout.Button("Shake", skin.skin.button))
                {
                    cameraObject.ShakeCamera(_shakeID);
                }
            }
        }

        void CameraSwitchButton(CameraControl.CameraType toCamera)
        {
            if (cameraObject != null)
            {
                if (GUILayout.Button("Switch To This Cam", skin.skin.button))
                    cameraObject.SetCameraType(toCamera);
            }
        }

        void InfoButton(string _title, string _info)
        {
            if (GUILayout.Button("", infoButton, GUILayout.Width(18), GUILayout.Height(18)))
            {
                CameraSettingsInfoWindow.OpenWindow(_title, _info);
            }
        }

        void UpdateSubMenu()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(contentWidth * 0.98f));
            if (GUILayout.Button("", positionButton, GUILayout.Width(32), GUILayout.Height(32)))
            {
                activeSubPage = ActiveSubPage.POSITION;
                positionButton = InitButtonStyle(positionIconSelected);
                orbitButton = InitButtonStyle(orbitIcon);
                panButton = InitButtonStyle(panIcon);
                inputButton = InitButtonStyle(inputIcon);
                debugButton = InitButtonStyle(debugIcon);
            }
            if (GUILayout.Button("", orbitButton, GUILayout.Width(32), GUILayout.Height(32)))
            {
                activeSubPage = ActiveSubPage.ORBIT;
                positionButton = InitButtonStyle(positionIcon);
                orbitButton = InitButtonStyle(orbitIconSelected);
                panButton = InitButtonStyle(panIcon);
                inputButton = InitButtonStyle(inputIcon);
                debugButton = InitButtonStyle(debugIcon);
            }
            if (activePage == ActivePage.RTS)
            {
                if (GUILayout.Button("", panButton, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    activeSubPage = ActiveSubPage.PAN;
                    positionButton = InitButtonStyle(positionIcon);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIconSelected);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIcon);
                }
            }
            if (activePage != ActivePage.FPS)
            {
                if (GUILayout.Button("", inputButton, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    activeSubPage = ActiveSubPage.INPUT;
                    positionButton = InitButtonStyle(positionIcon);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIconSelected);
                    debugButton = InitButtonStyle(debugIcon);
                }
            }
            /*if (activePage == ActivePage.RTS)
            {
                if (GUILayout.Button("", mobileButton, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    activeSubPage = ActiveSubPage.MOBILE;
                    positionButton = InitButtonStyle(positionIcon);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    mobileButton = InitButtonStyle(mobileIconSelected);
                    debugButton = InitButtonStyle(debugIcon);
                    shakeButton = InitButtonStyle(shakeIcon);
                }
            }*/
            if (activePage == ActivePage.RPG)
            {
                if (GUILayout.Button("", debugButton, GUILayout.Width(32), GUILayout.Height(32)))
                {
                    activeSubPage = ActiveSubPage.DEBUG;
                    positionButton = InitButtonStyle(positionIcon);
                    orbitButton = InitButtonStyle(orbitIcon);
                    panButton = InitButtonStyle(panIcon);
                    inputButton = InitButtonStyle(inputIcon);
                    debugButton = InitButtonStyle(debugIconSelected);
                }
            }

            if (Application.isPlaying)
            {
                switch (activePage)
                {
                    case ActivePage.RPG: CameraSwitchButton(CameraControl.CameraType.RPG); break;
                    case ActivePage.FPS: CameraSwitchButton(CameraControl.CameraType.FPS); break;
                    case ActivePage.RTS: CameraSwitchButton(CameraControl.CameraType.RTS); break;
                    case ActivePage.TOP_DOWN: CameraSwitchButton(CameraControl.CameraType.TOP_DOWN); break;
                }
            }

            EditorGUILayout.EndHorizontal();

            HorizontalLine(contentWidth * 0.98f);
        }

        void DrawSettingsTitle()
        {
            string title = (activePage == ActivePage.RPG) ? "RPG Camera" :
                           (activePage == ActivePage.FPS) ? "FPS Camera" :
                           (activePage == ActivePage.RTS) ? "RTS Camera" :
                           (activePage == ActivePage.TOP_DOWN) ? "Top-Down Camera" : "Invalid Page. Re-open";
            string subTitle = (activeSubPage == ActiveSubPage.POSITION) ? "Position Settings" :
                              (activeSubPage == ActiveSubPage.ORBIT) ? "Orbit Settings" :
                              (activeSubPage == ActiveSubPage.INPUT) ? "Input Settings" :
                              (activeSubPage == ActiveSubPage.MOBILE) ? "Mobile Settings" :
                              (activeSubPage == ActiveSubPage.DEBUG) ? "Collision Settings" :
                              (activeSubPage == ActiveSubPage.PAN) ? "Pan Settings" : "Invalid Settings. Re-open";
            string message = "Have more ideas/requests for this page? Email me at contentdev@darrenoneale.com";
            GUIStyle img = InitButtonStyle((activeSubPage == ActiveSubPage.POSITION) ? positionIcon :
                                            (activeSubPage == ActiveSubPage.ORBIT) ? orbitIcon :
                                            (activeSubPage == ActiveSubPage.INPUT) ? inputIcon :
                                            (activeSubPage == ActiveSubPage.DEBUG) ? debugIcon :
                                            (activeSubPage == ActiveSubPage.PAN) ? panIcon : positionIcon);
            
            GUILayout.BeginArea(contentImageRect);
            GUILayout.Button("", img, GUILayout.Width(Constants.CONTENT_IMAGE_SIZE), GUILayout.Height(Constants.CONTENT_IMAGE_SIZE));
            GUILayout.EndArea();

            GUILayout.Space(Constants.CONTENT_IMAGE_SIZE + 5);
            GUILayout.Label(title, skin.skin.customStyles[3]);
            GUILayout.Label(subTitle, skin.skin.customStyles[2]);
            GUILayout.Label(message, skin.skin.customStyles[3]);
            GUILayout.Space(5);
            HorizontalLine(Screen.width);
            GUILayout.Space(5);
        }

        void DrawGeneralSettingsTitle()
        {
            string title =  "All Cameras";
            string subTitle = (activePage == ActivePage.SHAKE) ? "Shake Settings" :
                              (activePage == ActivePage.PLAYER_CONTROLLER) ? "Player Controller" :
                              (activePage == ActivePage.OBSTRUCTION) ? "Obstruction Handler" : "Invalid Settings. Re-open";
            string message = "Have more ideas/requests for this page? Email me at contentdev@darrenoneale.com";
            GUIStyle img = InitButtonStyle((activePage == ActivePage.SHAKE) ? generalIcon :
                                            (activePage == ActivePage.PLAYER_CONTROLLER) ? playerIcon :
                                            (activePage == ActivePage.OBSTRUCTION) ? obstructionIcon : generalIcon);

            Rect r = contentImageRect;
            r.y = 0;
            GUILayout.BeginArea(r);
            GUILayout.Button("", img, GUILayout.Width(Constants.CONTENT_IMAGE_SIZE), GUILayout.Height(Constants.CONTENT_IMAGE_SIZE));
            GUILayout.EndArea();

            GUILayout.Space(Constants.CONTENT_IMAGE_SIZE + 5);
            GUILayout.Label(title, skin.skin.customStyles[3]);
            GUILayout.Label(subTitle, skin.skin.customStyles[2]);
            GUILayout.Label(message, skin.skin.customStyles[3]);
            GUILayout.Space(5);
            HorizontalLine(Screen.width);
            GUILayout.Space(5);
        }

        void HorizontalLine(float length)
        {
            GUI.backgroundColor = Color.grey;
            GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.Width(length));
            GUI.backgroundColor = Color.white;
        }
        
        void RefreshDataLists()
        {
            rpgDatas = Resources.LoadAll(Constants.RPG_DATA_PATH);
            rpgDataNames = new string[rpgDatas.Length];
            for (int i = 0; i < rpgDatas.Length; i++)
                rpgDataNames[i] = rpgDatas[i].name;
            
            fpsDatas = Resources.LoadAll(Constants.FPS_DATA_PATH);
            fpsDataNames = new string[fpsDatas.Length];
            for (int i = 0; i < fpsDatas.Length; i++)
                fpsDataNames[i] = fpsDatas[i].name;

            rtsDatas = Resources.LoadAll(Constants.RTS_DATA_PATH);
            rtsDataNames = new string[rtsDatas.Length];
            for (int i = 0; i < rtsDatas.Length; i++)
                rtsDataNames[i] = rtsDatas[i].name;

            topDownDatas = Resources.LoadAll(Constants.TOPDOWN_DATA_PATH);
            topDownDataNames = new string[topDownDatas.Length];
            for (int i = 0; i < topDownDatas.Length; i++)
                topDownDataNames[i] = topDownDatas[i].name;

            shakeData = (CameraShakeData)Resources.Load(Constants.SHAKE_DATA_PATH);
            playerControllerData = (PlayerControllerData)Resources.Load(Constants.PLAYER_CONTROLLER_DATA_PATH);
            obstructionData = (ObstructionHandlerData)Resources.Load(Constants.OBSTRUCTION_HANDLER_DATA_PATH);

            if (rpgDatas.Length > 0)
                rpgData = (CameraData)rpgDatas[selectedRPGData];
            if (fpsDatas.Length > 0)
                fpsData = (CameraData)fpsDatas[selectedFPSData];
            if (rtsDatas.Length > 0)
                rtsData = (CameraData)rtsDatas[selectedRTSData];
            if (topDownDatas.Length > 0)
                topDownData = (CameraData)topDownDatas[selectedTopDownData];

            if (cameraObject)
            {
                if (rpgData)
                    cameraObject.GetComponent<CameraControl>().dataManager.rpgData = rpgData;
                if (fpsData)
                    cameraObject.GetComponent<CameraControl>().dataManager.fpsData = fpsData;
                if (rtsData)
                    cameraObject.GetComponent<CameraControl>().dataManager.rtsData = rtsData;
                if (topDownData)
                    cameraObject.GetComponent<CameraControl>().dataManager.topDownData = topDownData;
                if (shakeData)
                    cameraObject.GetComponent<CameraControl>().dataManager.shakeData = shakeData;
                if (playerControllerData)
                    cameraObject.GetComponent<CameraControl>().dataManager.playerControllerData = playerControllerData;
                if (obstructionData)
                    cameraObject.GetComponent<CameraControl>().dataManager.obstructionData = obstructionData;
            }
        }

        void InitStyles()
        {

            rpgIcon = (Texture2D)Resources.Load("rpg_icon", typeof(Texture2D));
            rpgIconSelected = (Texture2D)Resources.Load("rpg_iconSelected", typeof(Texture2D));
            rpgButton = InitButtonStyle(rpgIcon);

            fpsIcon = (Texture2D)Resources.Load("fps_icon", typeof(Texture2D));
            fpsIconSelected = (Texture2D)Resources.Load("fps_iconSelected", typeof(Texture2D));
            fpsButton = InitButtonStyle(fpsIcon);

            rtsIcon = (Texture2D)Resources.Load("rts_icon", typeof(Texture2D));
            rtsIconSelected = (Texture2D)Resources.Load("rts_iconSelected", typeof(Texture2D));
            rtsButton = InitButtonStyle(rtsIcon);

            topDownIcon = (Texture2D)Resources.Load("topDown_icon", typeof(Texture2D));
            topDownIconSelected = (Texture2D)Resources.Load("topDown_iconSelected", typeof(Texture2D));
            topDownButton = InitButtonStyle(topDownIcon);

            positionIcon = (Texture2D)Resources.Load("editor_positionIcon", typeof(Texture2D));
            positionIconSelected = (Texture2D)Resources.Load("editor_positionIconSelected", typeof(Texture2D));
            positionButton = InitButtonStyle(positionIcon);

            orbitIcon = (Texture2D)Resources.Load("editor_rotationIcon", typeof(Texture2D));
            orbitIconSelected = (Texture2D)Resources.Load("editor_rotationIconSelected", typeof(Texture2D));
            orbitButton = InitButtonStyle(orbitIcon);

            inputIcon = (Texture2D)Resources.Load("input_icon", typeof(Texture2D));
            inputIconSelected = (Texture2D)Resources.Load("input_iconSelected", typeof(Texture2D));
            inputButton = InitButtonStyle(inputIcon);

            debugIcon = (Texture2D)Resources.Load("collision_icon", typeof(Texture2D));
            debugIconSelected = (Texture2D)Resources.Load("collision_iconSelected", typeof(Texture2D));
            debugButton = InitButtonStyle(debugIcon);

            panIcon = (Texture2D)Resources.Load("pan_icon", typeof(Texture2D));
            panIconSelected = (Texture2D)Resources.Load("pan_iconSelected", typeof(Texture2D));
            panButton = InitButtonStyle(panIcon);

            generalIcon = (Texture2D)Resources.Load("shake_icon", typeof(Texture2D));
            generalIconSelected = (Texture2D)Resources.Load("shake_iconSelected", typeof(Texture2D));
            generalButton = InitButtonStyle(generalIcon);

            infoIcon = (Texture2D)Resources.Load("info_icon", typeof(Texture2D));
            infoButton = InitButtonStyle(infoIcon);

            playerIcon = (Texture2D)Resources.Load("player_icon", typeof(Texture2D));
            playerIconSelected = (Texture2D)Resources.Load("player_iconSelected", typeof(Texture2D));
            playerButton = InitButtonStyle(playerIcon);

            obstructionIcon = (Texture2D)Resources.Load("obstruction_icon", typeof(Texture2D));
            obstructionIconSelected = (Texture2D)Resources.Load("obstruction_iconSelected", typeof(Texture2D));
            obstructionButton = InitButtonStyle(obstructionIcon);

            addIcon = (Texture2D)Resources.Load("add_icon", typeof(Texture2D));
            addButton = InitButtonStyle(addIcon);

            removeIcon = (Texture2D)Resources.Load("remove_icon", typeof(Texture2D));
            removeButton = InitButtonStyle(removeIcon);

            duplicateIcon = (Texture2D)Resources.Load("duplicate_icon", typeof(Texture2D));
            duplicateButton = InitButtonStyle(duplicateIcon);

            skin = (GUISkinData)Resources.Load("Data/Style/EditorSkin_Dark", typeof(GUISkinData));
        }

        GUIStyle InitButtonStyle(Texture2D icon)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = icon;
            style.margin = new RectOffset(2, 2, 2, 2);
            style.active.background = icon;
            style.focused.background = icon;
            style.hover.background = icon;
            return style;
        }

        GUIStyle rpgButton, fpsButton, rtsButton, topDownButton;
        GUIStyle positionButton, orbitButton, inputButton, debugButton, panButton;
        GUIStyle generalButton, cameraButton;
        GUIStyle addButton, removeButton, duplicateButton;
        GUIStyle playerButton, obstructionButton;
        GUIStyle infoButton;
        Texture2D rpgIcon, fpsIcon, rtsIcon, topDownIcon;
        Texture2D rpgIconSelected, fpsIconSelected, rtsIconSelected, topDownIconSelected;
        Texture2D positionIcon, orbitIcon, inputIcon, debugIcon, panIcon;
        Texture2D positionIconSelected, orbitIconSelected, inputIconSelected, debugIconSelected, panIconSelected;
        Texture2D generalIcon;
        Texture2D generalIconSelected;
        Texture2D infoIcon;
        Texture2D addIcon, removeIcon, duplicateIcon;
        Texture2D playerIcon, playerIconSelected, obstructionIcon, obstructionIconSelected;
    }
}
