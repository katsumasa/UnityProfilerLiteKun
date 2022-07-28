using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.PlayerConnection;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.SceneManagement;
using System.Runtime.Remoting.Messaging;
using System.Linq;

#if UNITY_2020_1_OR_NEWER
using UnityEngine.Networking.PlayerConnection;
using ConnectionUtility = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUIUtility;
using ConnectionGUILayout = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUILayout;
#elif UNITY_2018_1_OR_NEWER
using UnityEngine.Experimental.Networking.PlayerConnection;
using ConnectionUtility = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUIUtility;
using ConnectionGUILayout = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUILayout;
#endif

namespace Utj.UnityProfilerLiteKun
{
    public class UnityProfilerLiteKunEditorWindow : EditorWindow
    {
        private static class Styles
        {
            public static readonly GUIContent TitleContent = new GUIContent("Profiler Lite", (Texture2D)EditorGUIUtility.Load("d_UnityEditor.ProfilerWindow@2x"));
            public static readonly GUIContent RecOnContent= new GUIContent((Texture2D)EditorGUIUtility.Load("d_Record On@2x"),"Start Recording");
            public static readonly GUIContent RecOffContent = new GUIContent((Texture2D)EditorGUIUtility.Load("d_Record Off@2x"),"Stop Recording");            
            public static readonly GUIContent SaveContent = new GUIContent((Texture2D)EditorGUIUtility.Load("d_SaveAs@2x"),"Save Profile Data as CSV");
            public static readonly GUIContent OpenContent = new GUIContent((Texture2D)EditorGUIUtility.Load("d_Profiler.Open@2x"),"Load Profile Data");
            public static readonly GUIContent StatsContent = new GUIContent("enabled Stats",(Texture2D)EditorGUIUtility.Load("d_SaveAs@2x"));
            public static readonly GUIContent statsContent = EditorGUIUtility.TrTextContent("Stats");
            public static readonly GUIContent clearData = EditorGUIUtility.TrTextContent("Clear", "Clear the captured data");
        }



        [SerializeField] IConnectionState mAttachProfilerState;
        IConnectionState attachProfilerState
        {
            get { return mAttachProfilerState; }
            set { mAttachProfilerState = value; }
        }

        bool mEnabledStats;
        bool mIsRecording;

        ProfileData mProfileData;
        ProfileData profileData
        {
            get { if (mProfileData == null) { mProfileData = new ProfileData(); } return mProfileData; }
            set { mProfileData = value; }
        }


        [SerializeField] List<ProfileData> mProfileDataList;
        List<ProfileData> profileDataList
        {
            get { if (mProfileDataList == null) { mProfileDataList = new List<ProfileData>(); } return mProfileDataList; }
            set { mProfileDataList = value; }
        }


        [SerializeField] int mFrameCountMax;
        int frameCountMax
        {
            get { return mFrameCountMax; }
            set { mFrameCountMax = value; }
        }


        [SerializeField] List<float>[] mGraphLists;

        [SerializeField] Vector2 mScrollPos;
        [SerializeField] int mSlider;


        static UnityProfilerLiteKunEditorWindow mEditorWindow;
        public static UnityProfilerLiteKunEditorWindow editorWindow
        {
            get { 
                if(mEditorWindow == null)
                {
                    mEditorWindow = (UnityProfilerLiteKunEditorWindow)EditorWindow.GetWindow(typeof(UnityProfilerLiteKunEditorWindow));
                    mEditorWindow.titleContent = Styles.TitleContent;
                    
                }
                return mEditorWindow;
            }
        }



        [MenuItem("Window/UTJ/UnityProfilerLiteKun")]
        public static void OpenWindow()
        {            
            editorWindow.Show();
        }


        private void GUILayoutConnect()
        {
            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent("Connect To");
            var contentSize = EditorStyles.label.CalcSize(content);
            EditorGUILayout.LabelField(content,GUILayout.MaxWidth(contentSize.x));
            if (attachProfilerState != null)
            {
#if UNITY_2020_1_OR_NEWER
                ConnectionGUILayout.ConnectionTargetSelectionDropdown(attachProfilerState, EditorStyles.toolbarDropDown);
#else
                ConnectionGUILayout.AttachToPlayerDropdown(attachProfilerState, EditorStyles.toolbarDropDown);
#endif
            }
            EditorGUI.BeginChangeCheck();
            contentSize = EditorStyles.label.CalcSize(Styles.RecOnContent);
            mIsRecording = GUILayout.Toggle(mIsRecording, mIsRecording ? Styles.RecOnContent : Styles.RecOffContent, EditorStyles.toolbarButton,GUILayout.MaxWidth(contentSize.x + 10));
            if (EditorGUI.EndChangeCheck())
            {                
                if (mIsRecording) {
                    SendMessage( MessageData.MessageID.RECORDING_ON);
                }
                else
                {
                    SendMessage(MessageData.MessageID.RECORDING_OFF);
                } 
            }

            content = new GUIContent(string.Format("Frame:{0,6}/", profileDataList.Count));
            contentSize = EditorStyles.label.CalcSize(content);            
            EditorGUILayout.LabelField(content, GUILayout.MaxWidth(contentSize.x+10));

            content = new GUIContent(string.Format("Frame:{0}",frameCountMax));
            contentSize = EditorStyles.label.CalcSize(content);
            var tmp = EditorGUILayout.DelayedIntField(frameCountMax,GUILayout.MaxWidth(contentSize.x-32));
            if(tmp > 0)
            {
                frameCountMax = tmp;
            }
            //
            // Clear
            //
            contentSize = EditorStyles.label.CalcSize(Styles.clearData);
            if (GUILayout.Button(Styles.clearData, EditorStyles.toolbarButton,GUILayout.MaxWidth(contentSize.x+10)))
            {
                profileDataList.Clear();
                mSlider = 0;
            }

            //
            // Load CSV
            //
            contentSize = EditorStyles.label.CalcSize(Styles.OpenContent);
            if (GUILayout.Button(Styles.OpenContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(contentSize.x + 10)))
            {
                var path = EditorUtility.OpenFilePanel("Open Profile Data","","csv");
                if (!string.IsNullOrEmpty(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        if (mProfileDataList == null)
                        {
                            mProfileDataList = new List<ProfileData>();
                        }
                        else
                        {
                            mProfileDataList.Clear();
                        }
                        // headerを読み捨てる
                        sr.ReadLine();
                        // Bodyの読み込む
                        while(!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var item = new ProfileData();
                            item.SetCsvBody(line);
                            mProfileDataList.Add(item);

                        }
                        sr.Close();
                        
                    }
                }
            }
            //
            // Save File
            //
            contentSize = EditorStyles.label.CalcSize(Styles.SaveContent);
            if(GUILayout.Button(Styles.SaveContent, EditorStyles.toolbarButton, GUILayout.MaxWidth(contentSize.x + 10)))
            {
                var path = EditorUtility.SaveFilePanel(
                    "Save Profile Data as CSV",
                    "",
                    "",
                    "csv");
                if(path.Length != 0)
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        
                        sw.WriteLine(ProfileData.GetCSVHeader());
                        foreach(var d in profileDataList)
                        {
                            sw.WriteLine(d.GetCSVBody());
                        }
                        sw.Close();
                    }
                }                    
            }

            EditorGUI.BeginChangeCheck();
            contentSize = EditorStyles.label.CalcSize(Styles.statsContent);
            mEnabledStats = GUILayout.Toggle(mEnabledStats, Styles.statsContent, EditorStyles.toolbarButton,GUILayout.MaxWidth(contentSize.x + 10));
            if (EditorGUI.EndChangeCheck())
            {
                SendMessage(mEnabledStats ? MessageData.MessageID.STATS_ON : MessageData.MessageID.STATS_OFF);
            }
            EditorGUILayout.EndHorizontal();

            var playerCount = EditorConnection.instance.ConnectedPlayers.Count;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} players connected.", playerCount));
            int i = 0;
            foreach (var p in EditorConnection.instance.ConnectedPlayers)
            {
                builder.AppendLine(string.Format("[{0}] - {1} {2}", i++, p.name, p.playerId));
            }
            EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
        }


        private void Update()
        {
            
        }


        void OnSceneGUI()
        {
            
        }

        private void OnGUI()
        {
            var protList = new List<float>();
            var ofst = 0;
            var count = 0;
            int w = (int)EditorGUIUtility.currentViewWidth;
#if true
            if (mSlider > w)
            {
                ofst = mSlider - w;
                count = w;
            }
            else
#endif
            {
                ofst = 0;
                count = mSlider;
            }
            var current = count + ofst;


            GUILayoutConnect();
            {
                long frameCount = 0;
                if(profileDataList.Count > current)
                {
                    frameCount = profileDataList[current].mFrameCount;
                }
                EditorGUILayout.LabelField(Format("Frame Count:{0,7}", frameCount));
            }

            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);            
            // FrameRate
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add(profileDataList[i + ofst].mDeltaTime * 1000.0f);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 1.0f;                
                var content = new GUIContent(Format("Frame Rate {1,3:F1}ms ({0,3:F1} FPS)", 1.0f / v * 1000.0f, v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, Color.yellow);
            }            
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            // Time.deltaTime
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add(profileDataList[i + ofst].mDeltaTime2 * 1000.0f);
                }
                var content = new GUIContent(Format("Time.deltaTime {1,3:F1}ms ({0,3:F1} FPS)", 1.0f / profileData.mDeltaTime2, profileData.mDeltaTime2 * 1000.0f));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, Color.yellow);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // CPU Time
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mCpuFrameTime);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;                
                var content = new GUIContent(Format("CPU Time {0,3:F1}ms", v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, new Color32(124,97,158,255));
            }
            EditorGUILayout.Space();            
            EditorGUILayout.Space();

            // GPU Time
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mGpuFrameTime);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;
                var content = new GUIContent(Format("GPU Time {0,3:F1}ms", (float)v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, new Color32(75,172,198,255));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("CPU Usage:");
            // Rendering Time
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mRenderingTime / 1000000.0f);                                        
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;
                var content = new GUIContent(Format("Rendering {0,3:F1}ms", v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, new Color32(127,154,72,255));
            }            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Script Time
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mScriptTime/ 1000000.0f);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;
                var content = new GUIContent(Format("Script {0,3:F1}ms", v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, new Color32(51,153,255,255));
            }
            EditorGUILayout.Space();            
            EditorGUILayout.Space();

            // Physics
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mPhysicsTime/ 1000000.0f);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;
                var content = new GUIContent(Format("Physics{0,3:F1}ms",v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, Color.magenta);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();           

            // Animation
            {
                protList.Clear();
                for (var i = 0; i < count; i++)
                {
                    protList.Add((float)profileDataList[i + ofst].mAnimationTime/ 1000000.0f);
                }
                float v = protList.Count > 0 ? protList[protList.Count - 1] : 0;
                var content = new GUIContent(Format("Animation{0,3:F1}ms", v));
                EditorGUILayoutUTJ.GraphFieldFloat(content, protList, Color.cyan);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Memory:");
            var allocateList = new List<long>();
            var reservedList = new List<long>();            
            // Total
            {
                allocateList.Clear();
                reservedList.Clear();

                for (var i = 0; i < count; i++)
                {
                    allocateList.Add(profileDataList[i + ofst].mTotalAllocatedMemorySize);
                    reservedList.Add(profileDataList[i + ofst].mTotalReservedMemorySize);                 
                }
                var allocateSize = allocateList.Count > 0 ? allocateList[allocateList.Count - 1] : 0;
                var reservedSize = reservedList.Count > 0 ? reservedList[reservedList.Count - 1] : 0;
                var content = new GUIContent("Unity:" + FormatBytes(allocateSize) + "/" + FormatBytes(reservedSize));
                EditorGUILayoutUTJ.GrapFieldMemory(content, reservedList, new Color32(217,170,170,255), allocateList, new Color32(192,80,72,255));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();            

            // Unity
            {
                allocateList.Clear();
                reservedList.Clear();

                for (var i = 0; i < count; i++)
                {
                    allocateList.Add(profileDataList[i + ofst].mMonoUsedSize);
                    reservedList.Add(profileDataList[i + ofst].mMonoHeapSize);
                }
                var allocateSize = allocateList.Count > 0 ? allocateList[allocateList.Count - 1] : 0;
                var reservedSize = reservedList.Count > 0 ? reservedList[reservedList.Count - 1] : 0;
                var content = new GUIContent("Mono:" + FormatBytes(allocateSize) + "/" + FormatBytes(reservedSize));
                EditorGUILayoutUTJ.GrapFieldMemory(content, reservedList, new Color32(170,186,215,255), allocateList, new Color32(79,129,189,255));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();            

            // GfxDriver
            {
                allocateList.Clear();
                reservedList.Clear();

                for (var i = 0; i < count; i++)
                {                    
                    allocateList.Add(profileDataList[i + ofst].mGfxDriverAllocatedMemory);
                }
                var allocateSize = allocateList.Count > 0 ? allocateList[allocateList.Count - 1] : 0;
                var content = new GUIContent("GfxDriver:" + FormatBytes(allocateSize));
                EditorGUILayoutUTJ.GrapFieldMemory(content, reservedList, new Color32(155, 187, 89, 255), allocateList, new Color32(155, 187, 89, 255));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();

            mSlider = EditorGUILayout.IntSlider(mSlider,0, mProfileDataList.Count - 1);
        }

        private void OnEnable()
        {
            if (attachProfilerState == null)
            {
#if UNITY_2020_1_OR_NEWER
                attachProfilerState = ConnectionUtility.GetConnectionState(this);
#else
                attachProfilerState = ConnectionUtility.GetAttachToPlayerState(this);
#endif
            }
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Initialize();
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Register(UnityProfilerLiteKun.kMsgSendProfileDataPlayerToEditor, OnProfileDataEvent);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Register(UnityProfilerLiteKun.kMsgSendMessagePlayerToEditor, OnMessageEvent);
            if (frameCountMax <= 0)
            {
                frameCountMax = 999999;
            }
        }


        private void OnDisable()
        {
            if (attachProfilerState != null)
            {
                attachProfilerState.Dispose();
            }
            attachProfilerState = null;
        }


        private void OnDestroy()
        {
            EditorConnection.instance.Unregister(UnityProfilerLiteKun.kMsgSendProfileDataPlayerToEditor, OnProfileDataEvent);
            EditorConnection.instance.Unregister(UnityProfilerLiteKun.kMsgSendMessagePlayerToEditor, OnMessageEvent);
            EditorConnection.instance.DisconnectAll();
        }


        private void OnMessageEvent(UnityEngine.Networking.PlayerConnection.MessageEventArgs args)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream(args.data);
            var messageData = (MessageData)bf.Deserialize(ms);
            switch (messageData.mMessageID)
            {
                case MessageData.MessageID.RECORDING_ON:
                    mIsRecording = true;
                    break;

                case MessageData.MessageID.RECORDING_OFF:
                    mIsRecording = false;
                    break;

            }
        }

        private void OnProfileDataEvent(UnityEngine.Networking.PlayerConnection.MessageEventArgs args)
        {
            //Debug.Log("OnMessageEvent");
            //var json = System.Text.Encoding.ASCII.GetString(args.data);
            //var message = JsonUtility.FromJson<UnityChoseKunMessageData>(json);
            if (mIsRecording)
            {
                var bf = new BinaryFormatter();
                var ms = new MemoryStream(args.data);
                mProfileData = (ProfileData)bf.Deserialize(ms);
                ms.Close();
                profileDataList.Add(mProfileData);
                mSlider = profileDataList.Count;
                Repaint();
            } 
            else
            {
                SendMessage(MessageData.MessageID.RECORDING_OFF);
            }            
        }


        private void SendMessage(MessageData.MessageID messageID)
        {
            //Debug.Log("SendMessage");
            var messageData = new MessageData();
            messageData.mMessageID = messageID;

            var json = JsonUtility.ToJson(messageData);
            var bytes = System.Text.Encoding.ASCII.GetBytes(json);
            EditorConnection.instance.Send(UnityProfilerLiteKun.kMsgSendEditorToPlayer, bytes);            
            
        }


        public static string Format(string fmt, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
        }


        private static string FormatNumber(long num)
        {
            if (num < 1000)
                return num.ToString(CultureInfo.InvariantCulture.NumberFormat);
            if (num < 1000000)
                return (num * 0.001).ToString("f1", CultureInfo.InvariantCulture.NumberFormat) + "K";
            return (num * 0.000001).ToString("f1", CultureInfo.InvariantCulture.NumberFormat) + "M";
        }


        private string FormatBytes(long num)
        {
            if (num < 0)
                return "Unknown";

            // TODO vs2015: 2010 doesn't have C99 inttypes.h where PRId64 format specifier
            //              is located but vs2015 has
            if (num < 512)
                return num.ToString(CultureInfo.InvariantCulture.NumberFormat) + "B";

            if (num < 512 * 1024)
                return (num / 1024f).ToString("f1", CultureInfo.InvariantCulture.NumberFormat) + "KB";


            num /= 1024;
            if (num < 512 * 1024)
                return (num / 1024f).ToString("f1", CultureInfo.InvariantCulture.NumberFormat) + "MB";

            num /= 1024;
            return (num / 1024f).ToString("f2", CultureInfo.InvariantCulture.NumberFormat) + "GB";
        }


        void DrawGraphFloat(Rect view,List<float> list,Color color)
        {            
            if(list.Count == 0)
            {
                return;
            }
            float maxValue = 0;

            for(var i = 0; i < list.Count; i++)
            {
                if (list[i] > maxValue)
                
                {
                    maxValue = list[i];
                }
            }
            float scale = view.height / maxValue;            
            for(var i = 0; i < list.Count; i++)
            {
                var w = 1;
                var x = view.x + i * w;
                var y = view.y + view.height;                
                var h = list[i] * scale;
                var rect = new Rect(x, y, w, -h);
                EditorGUI.DrawRect(rect, color);
            }         
        }

    }
}
