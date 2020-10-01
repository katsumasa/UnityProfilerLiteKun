using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utj.UnityProfilerLiteKun
{
    public class UnityProfilerLiteKun
    {
        public static readonly System.Guid kMsgSendEditorToPlayer = new System.Guid("77827383de564256850e77b403bcf3f8");
        public static readonly System.Guid kMsgSendPlayerToEditor = new System.Guid("dcc882dc20f344759ece75ca6f67fc25");
    }

    
    public class MessageData
    {
        public enum MessageID
        {
            STATS_ON,
            STATS_OFF,
            RECORDING_ON,
            RECORDING_OFF,
        }


        public MessageID mMessageID;
    }


    [Serializable]
    public class ProfileData
    {
        public long mFrameCount;

        public float mDeltaTime;

        public long mPlayerLoopTime;

        public long mRenderingTime;
        public long mScriptTime;
        public long mPhysicsTime;
        public long mAnimationTime;

        public double mCpuFrameTime;
        public double mGpuFrameTime;

        public float mWidthScaleFactor;
        public float mHeightScaleFactor;
        public int mWidthResolution;
        public int mHeightResolution;

        public long mUsedHeapSize;
        public long mMonoHeapSize;
        public long mMonoUsedSize;
        public long mTempAllocatorSize;
        public long mTotalAllocatedMemorySize;
        public long mTotalReservedMemorySize;
        public long mTotalUnusedReservedMemorySize;

        public long mGfxDriverAllocatedMemory;


        

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
