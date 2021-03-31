using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utj.UnityProfilerLiteKun
{
    public class UnityProfilerLiteKun
    {
        public static readonly System.Guid kMsgSendEditorToPlayer = new System.Guid("77827383de564256850e77b403bcf3f8");
        public static readonly System.Guid kMsgSendProfileDataPlayerToEditor = new System.Guid("dcc882dc20f344759ece75ca6f67fc25");
        public static readonly System.Guid kMsgSendMessagePlayerToEditor = new System.Guid("2c0754b17b324067aa7d8e8e27c74db5");
        
    }

    [Serializable]
    public class MessageData
    {
        public enum MessageID
        {
            STATS_ON,
            STATS_OFF,
            RECORDING_ON,
            RECORDING_OFF,
        }
        [SerializeField] public MessageID mMessageID;       
    }


    [Serializable]
    public class ProfileData
    {
        [SerializeField] public long mFrameCount;
        [SerializeField] public float mDeltaTime;
        [SerializeField] public long mPlayerLoopTime;
        [SerializeField] public long mRenderingTime;
        [SerializeField] public long mScriptTime;
        [SerializeField] public long mPhysicsTime;
        [SerializeField] public long mAnimationTime;
        [SerializeField] public double mCpuFrameTime;
        [SerializeField] public double mGpuFrameTime;
        [SerializeField] public float mWidthScaleFactor;
        [SerializeField] public float mHeightScaleFactor;
        [SerializeField] public int mWidthResolution;
        [SerializeField] public int mHeightResolution;
        [SerializeField] public int mRefreshRate;
        [SerializeField] public long mUsedHeapSize;
        [SerializeField] public long mMonoHeapSize;
        [SerializeField] public long mMonoUsedSize;
        [SerializeField] public long mTempAllocatorSize;
        [SerializeField] public long mTotalAllocatedMemorySize;
        [SerializeField] public long mTotalReservedMemorySize;
        [SerializeField] public long mTotalUnusedReservedMemorySize;
        [SerializeField] public long mGfxDriverAllocatedMemory;


        

        public float GetDeltaTime()
        {
            return mDeltaTime;
        }


        public float GetPlayerLoopTime()
        {
            return (float)mPlayerLoopTime / 1000f / 1000f;
        }


        public float GetRenderingTime()
        {
            return (float)mScriptTime / 1000f / 1000f;
        }


        public static string GetCSVHeader()
        {
            return "frameCount,deltaTime,playerLoopTime,renderingTime,scriptTime,physicsTime,animationTime,cpuFrameTime,gpuFrameTime,widthScaleFactor,heightScaleFactor,widthResolutio,heightResolution,usedHeapSize,monoHeapSize,monoUsedSize,tempAllocatorSize,totalAllocatedMemorySize,totalReservedMemorySize,totalUnusedReservedMemorySize,gfxDriverAllocatedMemory";
        }

        public void SetCsvBody(string body)
        {
            string[] arr = body.Split(',');
            mFrameCount                     = System.Convert.ToInt64(arr[0]);
            mDeltaTime                      = System.Convert.ToSingle(arr[1]);
            mPlayerLoopTime                 = System.Convert.ToInt64(arr[2]);
            mRenderingTime                  = System.Convert.ToInt64(arr[3]);
            mScriptTime                     = System.Convert.ToInt64(arr[4]);
            mPhysicsTime                    = System.Convert.ToInt64(arr[5]);
            mAnimationTime                  = System.Convert.ToInt64(arr[6]);
            mCpuFrameTime                   = System.Convert.ToDouble(arr[7]);
            mGpuFrameTime                   = System.Convert.ToDouble(arr[8]);
            mWidthScaleFactor               = System.Convert.ToSingle(arr[9]);
            mHeightScaleFactor              = System.Convert.ToSingle(arr[10]);
            mWidthResolution                = System.Convert.ToInt32(arr[11]);
            mHeightResolution               = System.Convert.ToInt32(arr[12]);
            mUsedHeapSize                   = System.Convert.ToInt64(arr[13]);
            mMonoHeapSize                   = System.Convert.ToInt64(arr[14]);
            mMonoUsedSize                   = System.Convert.ToInt64(arr[15]);
            mTempAllocatorSize              = System.Convert.ToInt64(arr[16]);
            mTotalAllocatedMemorySize       = System.Convert.ToInt64(arr[17]);
            mTotalReservedMemorySize        = System.Convert.ToInt64(arr[18]);
            mTotalUnusedReservedMemorySize  = System.Convert.ToInt64(arr[19]);
            mGfxDriverAllocatedMemory       = System.Convert.ToInt64(arr[20]);
        }


        public string GetCSVBody()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                mFrameCount,
                mDeltaTime,
                mPlayerLoopTime,        
                mRenderingTime,                
                mScriptTime,
                mPhysicsTime,
                mAnimationTime,                
                mCpuFrameTime,
                mGpuFrameTime,                
                mWidthScaleFactor,
                mHeightScaleFactor,
                mWidthResolution,
                mHeightResolution,                
                mUsedHeapSize,
                mMonoHeapSize,
                mMonoUsedSize,
                mTempAllocatorSize,
                mTotalAllocatedMemorySize,
                mTotalReservedMemorySize,
                mTotalUnusedReservedMemorySize,
                mGfxDriverAllocatedMemory
                );
        }
    }
}
