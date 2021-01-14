using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;


namespace Utj.UnityProfilerLiteKun
{
    public class UnityProfilerLiteKunPlayer : MonoBehaviour
    {
        FrameTiming[] mFrameTimings;
        uint mFrameTimingNum;
        ProfileData mProfileData;

        long frameCount
        {
            get { return mProfileData.mFrameCount; }
            set { mProfileData.mFrameCount = value; }
        }

        float deltaTime
        {
            get { return mProfileData.mDeltaTime; }
            set { mProfileData.mDeltaTime = value; }
        }

        long renderingTime
        {
            get { return mProfileData.mRenderingTime; }
            set { mProfileData.mRenderingTime = value; }
        }

        long playerLoopTime
        {
            get { return mProfileData.mPlayerLoopTime; }
            set { mProfileData.mPlayerLoopTime = value; }
        }

        long scriptTime
        {
            get { return mProfileData.mScriptTime; }
            set { mProfileData.mScriptTime = value; }
        }

        long physicsTime
        {
            get { return mProfileData.mPhysicsTime; }
            set { mProfileData.mPhysicsTime = value; }
        }

        long animationTime
        {
            get { return mProfileData.mAnimationTime; }
            set { mProfileData.mAnimationTime = value; }
        }

        double cpuFrameTime
        {
            get { return mProfileData.mCpuFrameTime; }
            set { mProfileData.mCpuFrameTime = value; }
        }
        
        double gpuFrameTime
        {
            get { return mProfileData.mGpuFrameTime; }
            set { mProfileData.mGpuFrameTime = value; }
        }

        float widthScaleFactor
        {
            get { return mProfileData.mWidthScaleFactor; }
            set { mProfileData.mWidthScaleFactor = value; }
        }
        
        float heightScaleFactor
        {
            get { return mProfileData.mHeightScaleFactor; }
            set { mProfileData.mHeightScaleFactor = value; }
        }

        int widthResolution
        {
            get { return mProfileData.mWidthResolution; }
            set { mProfileData.mWidthResolution = value; }
        }
        
        int heightResolution
        {
            get { return mProfileData.mHeightResolution; }
            set { mProfileData.mHeightResolution = value; }
        }

        int refreshRate
        {
            get { return mProfileData.mRefreshRate; }
            set { mProfileData.mRefreshRate = value; }
        }

        long usedHeapSize
        {
            get { return mProfileData.mUsedHeapSize; }
            set { mProfileData.mUsedHeapSize = value; }
        }
        
        long monoHeapSize
        {
            get { return mProfileData.mMonoHeapSize; }
            set { mProfileData.mMonoHeapSize = value; }
        }
        
        long monoUsedSize
        {
            get { return mProfileData.mMonoUsedSize; }
            set { mProfileData.mMonoUsedSize = value; }
        }

        long tempAllocatorSize
        {
            get { return mProfileData.mTempAllocatorSize; }
            set { mProfileData.mTempAllocatorSize = value; }
        }

        long totalAllocatedMemorySize
        {
            get { return mProfileData.mTotalAllocatedMemorySize; }
            set { mProfileData.mTotalAllocatedMemorySize = value; }
        }

        long totalReservedMemorySize
        {
            get { return mProfileData.mTotalReservedMemorySize; }
            set { mProfileData.mTotalReservedMemorySize = value; }
        }

        long totalUnusedReservedMemorySize
        {
            get { return mProfileData.mTotalUnusedReservedMemorySize; }
            set { mProfileData.mTotalUnusedReservedMemorySize = value; }
        }

        long gfxDriverAllocatedMemory
        {
            get { return mProfileData.mGfxDriverAllocatedMemory; }
            set { mProfileData.mGfxDriverAllocatedMemory = value; }
        }

        const float K = 0.05f;
        
        float mPrevRealTime;
        float mAvgTime;

        


        int width
        {
            get
            {
                return (int)Mathf.Ceil(widthScaleFactor * widthResolution);
            }
        }


        int height
        {
            get
            {
                return (int)Mathf.Ceil(heightScaleFactor * heightResolution);
            }
        }

        string[] mPlayerLoopSamplerNames =
        {
            "PlayerLoop",
        };

        string[] mRenderingSamplerNames =
        {
            "PreLateUpdate.EndGraphicsJobsAfterScriptUpdate",
            "PreLateUpdate.ParticleSystemBeginUpdateAll",

            "PostLateUpdate.UpdateCustomRenderTextures",

            "PostLateUpdate.UpdateAllRenerers",
            "PostLateUpdate.UpdateAllSkinnedMeshes",
            "PostLateUpdate.FinishFrameRendering",
        };

        string[] mScriptsSamplerNames =
        {
            "Update.ScriptRunBehaviourUpdate",
            "PreLateUpdate.ScriptRunBehaviourLateUpdate",
            "FixedUpdate.ScriptRunBehaviourFixedUpdate",
            "Update.ScriptRunDelayedDynamicFrameRate",
        };

        string[] mPhysicsSamplerNames =
        {
            "FixedUpdate.PhysocsFixedUpdate",
        };

        string[] mAnimationSamplerNames = {
            "PreLateUpdate.DirectorUpdateAnimationBegin",
            "PreLateUpdate.DirectorUpdateAnimationEnd",
        };

        UnityEngine.Profiling.Recorder[] mPlayerLoopRecorders;
        UnityEngine.Profiling.Recorder[] mRenderingRecorders;
        UnityEngine.Profiling.Recorder[] mScriptsSamplerRecorders;
        UnityEngine.Profiling.Recorder[] mPhysicsSamplerRecorders;
        UnityEngine.Profiling.Recorder[] mAnimationSamplerRecorders;

        

        GUIStyle mLabelStyle;
        GUIStyle mTextStyle;


        bool mIsRecording;
        bool mIsEnableStats;


        // Start is called before the first frame update
        void Start()
        {

            mFrameTimings = new FrameTiming[3];
            mFrameTimingNum = 0;
            mProfileData = new ProfileData();


            frameCount = 0;
            mPrevRealTime = Time.realtimeSinceStartup;
            deltaTime = 0;

            RecorerdInit(in mPlayerLoopSamplerNames, out mPlayerLoopRecorders);
            RecorerdInit(in mRenderingSamplerNames, out mRenderingRecorders);
            RecorerdInit(in mScriptsSamplerNames, out mScriptsSamplerRecorders);
            RecorerdInit(in mPhysicsSamplerNames, out mPhysicsSamplerRecorders);
            RecorerdInit(in mAnimationSamplerNames, out mAnimationSamplerRecorders);


            mLabelStyle = new GUIStyle();
            mLabelStyle.fontSize = 16;
            mLabelStyle.fontStyle = FontStyle.Bold;
            mLabelStyle.normal.textColor = Color.white;

            mTextStyle = new GUIStyle();
            mTextStyle.fontSize = 16;
            mTextStyle.fontStyle = FontStyle.Normal;
            mTextStyle.normal.textColor = Color.white;
        }


        private void OnEnable()
        {
            PlayerConnection.instance.Register(UnityProfilerLiteKun.kMsgSendEditorToPlayer, OnMessageEvent);
        }

        private void OnDisable()
        {
            PlayerConnection.instance.Unregister(UnityProfilerLiteKun.kMsgSendEditorToPlayer, OnMessageEvent);
        }


        private void OnMessageEvent(MessageEventArgs args)
        {
            //UnityChoseKun.Log("OnMessageEvent ");

            var json = System.Text.Encoding.ASCII.GetString(args.data);
            var message = JsonUtility.FromJson<MessageData>(json);

            //UnityChoseKun.Log(message.mMessageID);
            switch (message.mMessageID)
            {
                case MessageData.MessageID.STATS_OFF:
                    {
                        mIsEnableStats = false;
                    }
                    break;

                case MessageData.MessageID.STATS_ON:
                    {
                        mIsEnableStats = true;
                    }
                    break;
                case MessageData.MessageID.RECORDING_ON:
                    {
                        mIsRecording = true;
                    }
                    break;
                case MessageData.MessageID.RECORDING_OFF:
                    {
                        mIsRecording = false;
                    }
                    break;
            }
        }


        void SendProfileData()
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            try
            {
                bf.Serialize(ms, mProfileData);
                //var json = JsonUtility.ToJson(mProfileData);
                //var bytes = System.Text.Encoding.ASCII.GetBytes(json);
                PlayerConnection.instance.Send(UnityProfilerLiteKun.kMsgSendPlayerToEditor, ms.ToArray());
            }
            finally
            {
                ms.Close();
            }
        }


        void RecorerdInit(in string[] samplerNames,out UnityEngine.Profiling.Recorder[] recorders)
        {
            recorders = new UnityEngine.Profiling.Recorder[samplerNames.Length];
            for(var i = 0; i < recorders.Length; i++)
            {
                recorders[i] = UnityEngine.Profiling.Recorder.Get(samplerNames[i]);
            }            
        }


        long GetRecordersTime(UnityEngine.Profiling.Recorder[] recorders)
        {
            long t = 0;
            foreach(var recorder in recorders)
            {
               t +=  recorder.elapsedNanoseconds;
            }
            return t;
        }

        // Update is called once per frame
        void Update()
        {

            frameCount++;
            if (frameCount <= 2)
            {
                return;
            }
            FrameTimingManager.CaptureFrameTimings();
            mFrameTimingNum = FrameTimingManager.GetLatestTimings(2, mFrameTimings);
            if (mFrameTimingNum > 1)
            {
                cpuFrameTime = mFrameTimings[0].cpuFrameTime;
                gpuFrameTime = mFrameTimings[0].gpuFrameTime;
                widthScaleFactor = mFrameTimings[0].widthScale;
                heightScaleFactor = mFrameTimings[0].heightScale;
            }

            // [指数関数で近似する](https://techblog.kayac.com/approximate-average-fps)
            var currentTime = Time.realtimeSinceStartup;
            deltaTime = currentTime - mPrevRealTime;
            mAvgTime *= 1f - K;
            mAvgTime += deltaTime * K;
            mPrevRealTime = currentTime;

            playerLoopTime = GetRecordersTime(mPlayerLoopRecorders);

            renderingTime = GetRecordersTime(mRenderingRecorders);
            scriptTime = GetRecordersTime(mScriptsSamplerRecorders);
            physicsTime = GetRecordersTime(mPhysicsSamplerRecorders);
            animationTime = GetRecordersTime(mAnimationSamplerRecorders);

            
            widthResolution = Screen.currentResolution.width;
            heightResolution = Screen.currentResolution.height;
            refreshRate = Screen.currentResolution.refreshRate;
            
            usedHeapSize = UnityEngine.Profiling.Profiler.usedHeapSizeLong;
            monoHeapSize = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
            monoUsedSize = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            tempAllocatorSize = UnityEngine.Profiling.Profiler.GetTempAllocatorSize();            
            totalReservedMemorySize = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
            totalAllocatedMemorySize = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            totalUnusedReservedMemorySize = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
            gfxDriverAllocatedMemory = UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver();

            if (mIsRecording)
            {
                SendProfileData();
            }
        }
               

        private void OnGUI()
        {
            if (mIsEnableStats == false)
            {
                return;
            }

            float w = 400, h = 324;

            GUILayout.BeginArea(new Rect(Screen.safeArea.width - w - 30, 27, w, h), "Statistics", GUI.skin.window);
#if true
            GUI.Label(new Rect(  8, 20, 390, 20), Format("Frame:{0,7}\n", Time.frameCount),mLabelStyle);
                        
            GUI.Label(new Rect(  8, 40,180, 20),"Graphics:",mLabelStyle);
            GUI.Label(new Rect(200, 40,180, 20), Format("{0,3:F1} FPS ({1,3:F1}ms)\n", 1.0f / mAvgTime, mAvgTime * 1000.0f), mTextStyle);
            GUI.Label(new Rect( 16, 60,190, 20), Format("CPU Time: {0,3:F1}[ms]\n", cpuFrameTime),mTextStyle);
            GUI.Label(new Rect(200, 60,190, 20), Format("GPU Time: {0,3:F1}[ms]\n", gpuFrameTime),mTextStyle);
            GUI.Label(new Rect( 16, 80,390, 20), Format("Screen: {0} x {1} ({2} x {3}) - {4} [Hz]", widthScaleFactor*widthResolution, heightResolution*heightScaleFactor, widthResolution, heightResolution, refreshRate),mTextStyle);
           

            GUI.Label(new Rect(  8,130,390, 20), "CPU Usage:", mLabelStyle);
            GUI.Label(new Rect( 16,150,180, 20), Format("Rendering: {0,3:F2}ms\n", renderingTime / 1000000.0f),mTextStyle);
            GUI.Label(new Rect(200,150,180, 20), Format("Scripts:   {0,3:F2}ms\n", scriptTime / 1000000.0f),mTextStyle);

            GUI.Label(new Rect( 16,170,180, 20), Format("Physics:   {0,3:F2}ms\n", physicsTime / 1000000.0f), mTextStyle);
            GUI.Label(new Rect(200,170,180, 20), Format("Animation: {0,3:F2}ms\n", animationTime / 1000000.0f), mTextStyle);

            GUI.Label(new Rect(  8,220,390, 20), "Memory:", mLabelStyle);            
            StringBuilder sb = new StringBuilder();
            sb.Append("Used:");            
            sb.Append("\n  Unity:");
            sb.Append(FormatBytes(totalAllocatedMemorySize));
            sb.Append(" Mono:");
            sb.Append(FormatBytes(monoUsedSize));
            sb.Append(" GfxDriver:");
            sb.Append(FormatBytes(gfxDriverAllocatedMemory));

            sb.Append("\nReserved:");            
            sb.Append("\n  Unity:");
            sb.Append(FormatBytes(totalReservedMemorySize));
            sb.Append(" Mono:");
            sb.Append(FormatBytes(monoHeapSize));

            GUI.Label(new Rect( 16,240,390, 40),sb.ToString(), mTextStyle);
# else

            GUILayout.Label("maxUsedMemory:"+ FormatBytes(UnityEngine.Profiling.Profiler.maxUsedMemory));
            GUILayout.Label("usedHeapSizeLong:" + FormatBytes(UnityEngine.Profiling.Profiler.usedHeapSizeLong));
            GUILayout.Label("GetAllocatedMemoryForGraphicsDriver:" + FormatBytes(UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver()));
            GUILayout.Label("GetMonoHeapSizeLong:" + FormatBytes(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong()));
            GUILayout.Label("GetMonoUsedSizeLong:" + FormatBytes(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()));
            GUILayout.Label("GetTempAllocatorSize:" + FormatBytes(UnityEngine.Profiling.Profiler.GetTempAllocatorSize()));
            GUILayout.Label("GetTotalAllocatedMemoryLong:" + FormatBytes(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()));
            GUILayout.Label("GetTotalReservedMemoryLong:" + FormatBytes(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong()));
            GUILayout.Label("GetTotalUnusedReservedMemoryLong:" + FormatBytes(UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong()));
#endif

            GUILayout.EndArea();
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

    }
}