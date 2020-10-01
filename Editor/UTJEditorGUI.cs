using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Globalization;
using System;
using Unity.Collections;

namespace Utj
{
    public class EditorGUILayoutUTJ{
        static public void GraphFieldFloat(GUIContent content, List<float> list, Color color)
        {

            if (content != null)
            {
                EditorGUILayout.LabelField(content);
            }
            var area = GUILayoutUtility.GetRect(Mathf.Min(EditorGUIUtility.currentViewWidth, 300f), 50.0f);            
            EditorGUI.DrawRect(area, UnityEngine.Color.gray);

            

            if (list.Count != 0)
            {
                var maxValue = list.Max();
                var avgValue = list.Average();
                var scale = area.height / maxValue * 0.90f; // 最大値の高さが描画範囲の80%位に
                
                for (var i = 0; i < list.Count; i++)
                {
                    var w = 1.0f;
                    var h = list[list.Count - (i + 1)] * scale;
                    var x = area.x + area.width - (i + 1) * w;
                    var y = area.y + area.height;
                    var rect = new Rect(x, y, w, -h);
                    EditorGUI.DrawRect(rect, color);
                }
                
                // 最大値の補助線
                {
                    var x = area.x;
                    var y = area.y + area.height - maxValue * scale;
                    var w = area.width;
                    var h = 1.0f;
                    EditorGUI.DrawRect(
                        new Rect(x,y,w,h),
                        Color.white
                        
                        );
                    var label = new GUIContent(Format("{0,3:F1}ms",maxValue));
                    var contentSize = EditorStyles.label.CalcSize(label);
                    EditorGUI.DrawRect(new Rect(x, y-contentSize.y/2, contentSize.x, contentSize.y), Color.black);
                    EditorGUI.LabelField(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), label);
                }

                // 平均値の補助線
                {
                    var x = area.x;
                    var y = area.y + area.height - avgValue * scale;
                    var w = area.width;
                    var h = 1.0f;
                    EditorGUI.DrawRect(
                        new Rect(x, y, w, h),
                        Color.white

                        );
                    var label = new GUIContent(Format("{0,3:F1}ms", avgValue));
                    var contentSize = EditorStyles.label.CalcSize(label);
                    EditorGUI.DrawRect(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), Color.black);
                    EditorGUI.LabelField(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), label);
                }
            }
            
            
        }

        static public void GrapFieldMemory(
            GUIContent content,
            List<long> reservedList,Color reservedColor,
            List<long> aloocateList,Color allocateColor
            )
        {

            if (content != null)
            {
                EditorGUILayout.LabelField(content);
            }
            var area = GUILayoutUtility.GetRect(Mathf.Min(EditorGUIUtility.currentViewWidth, 300f), 50.0f);
            EditorGUI.DrawRect(area, UnityEngine.Color.gray);

            float scale = 1.0f;
            long maxValue = 0;
            long avgValue = 0;


            

            if (reservedList != null && reservedList.Count != 0)
            {                
                scale = area.height / reservedList.Max() * 0.90f;
                for(var i = 0; i < reservedList.Count; i++)
                {
                    var w = 1.0f;
                    var h = reservedList[reservedList.Count - (i + 1)] * scale;
                    var x = area.x + area.width - (i + 1) * w;
                    var y = area.y + area.height;
                    var rect = new Rect(x, y, w, -h);
                    EditorGUI.DrawRect(rect, reservedColor);
                }
            }

            if(aloocateList != null && aloocateList.Count != 0)
            {
               

                maxValue = Math.Max(maxValue, aloocateList.Max());
                if (reservedList == null || reservedList.Count == 0)
                {
                    scale = area.height / maxValue * 0.90f;
                }
                avgValue = (long)aloocateList.Average();
                for (var i = 0; i < aloocateList.Count; i++)
                {
                    var w = 1.0f;
                    var h = aloocateList[aloocateList.Count - (i + 1)] * scale;
                    var x = area.x + area.width - (i + 1) * w;
                    var y = area.y + area.height;
                    var rect = new Rect(x, y, w, -h);
                    EditorGUI.DrawRect(rect, allocateColor);
                }
            }
            // 最大値の補助線
            {
                var x = area.x;
                var y = area.y + area.height - maxValue * scale;
                var w = area.width;
                var h = 1.0f;
                EditorGUI.DrawRect(
                    new Rect(x, y, w, h),
                    Color.white

                    );
                var label = new GUIContent(FormatBytes( maxValue));
                var contentSize = EditorStyles.label.CalcSize(label);
                EditorGUI.DrawRect(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), Color.black);
                EditorGUI.LabelField(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), label);
            }

            // 平均値の補助線
            {
                var x = area.x;
                var y = area.y + area.height - avgValue * scale;
                var w = area.width;
                var h = 1.0f;
                EditorGUI.DrawRect(
                    new Rect(x, y, w, h),
                    Color.white

                    );
                var label = new GUIContent(FormatBytes(avgValue));
                var contentSize = EditorStyles.label.CalcSize(label);
                EditorGUI.DrawRect(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), Color.black);
                EditorGUI.LabelField(new Rect(x, y - contentSize.y / 2, contentSize.x, contentSize.y), label);
            }            
        }



        public static string Format(string fmt, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);

        }


        private static string FormatBytes(long num)
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
