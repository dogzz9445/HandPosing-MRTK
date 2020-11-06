﻿using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PoseAuthoring.PoseVolumes.Editor
{
    [CustomEditor(typeof(CylinderSurface))]
    [CanEditMultipleObjects]
    public class CylinderEditor : UnityEditor.Editor
    {
        private static readonly Color NONINTERACTABLE_COLOR = new Color(0f, 1f, 1f, 0.1f);
        private static readonly Color INTERACTABLE_COLOR = new Color(0f, 1f, 1f, 0.5f);
        private const float DRAWSURFACE_RESOLUTION = 5f;

        private ArcHandle topArc = new ArcHandle();
        private Vector3[] surfaceEdges;

        private void OnEnable()
        {
            topArc.SetColorWithRadiusHandle(INTERACTABLE_COLOR, 0f);
        }

        public void OnSceneGUI()
        {
            CylinderSurface surface = (target as CylinderSurface);

            DrawEndsCaps(surface);
            DrawArcEditor(surface);
            if (Event.current.type == EventType.Repaint)
            {
                DrawCylinderVolume(surface);
            }
        }

        private void DrawEndsCaps(CylinderSurface cylinder)
        {
            EditorGUI.BeginChangeCheck();

            Quaternion handleRotation = (cylinder.relativeTo ?? cylinder.transform).rotation;

            Vector3 startPosition = Handles.PositionHandle(cylinder.StartPoint, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cylinder, "Change Start Cylinder Position");
                cylinder.StartPoint = startPosition;
            }
            EditorGUI.BeginChangeCheck();
            Vector3 endPosition = Handles.PositionHandle(cylinder.EndPoint, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cylinder, "Change Start Cylinder Position");
                cylinder.EndPoint = endPosition;
            }
        }

        private void DrawCylinderVolume(CylinderSurface cylinder)
        {
            Vector3 start = cylinder.StartPoint;
            Vector3 end = cylinder.EndPoint;
            float radious = cylinder.Radious;

            Handles.color = INTERACTABLE_COLOR;
            Handles.DrawWireArc(end,
            cylinder.Direction,
            cylinder.StartAngleDir,
            cylinder.Angle,
            radious);

            Handles.DrawLine(start,end);
            Handles.DrawLine(start, start + cylinder.StartAngleDir * radious);
            Handles.DrawLine(start, start + cylinder.EndAngleDir * radious);
            Handles.DrawLine(end,end + cylinder.StartAngleDir * radious);
            Handles.DrawLine(end, end + cylinder.EndAngleDir * radious);

            int edgePoints = Mathf.CeilToInt((2 * cylinder.Angle) / DRAWSURFACE_RESOLUTION) + 3;
            if(surfaceEdges == null 
                || surfaceEdges.Length != edgePoints)
            {
                surfaceEdges = new Vector3[edgePoints];
            }

            Handles.color = NONINTERACTABLE_COLOR;
            int i = 0;
            for(float angle = 0f; angle < cylinder.Angle; angle += DRAWSURFACE_RESOLUTION)
            {
                Vector3 direction = Quaternion.AngleAxis(angle, cylinder.Direction) * cylinder.StartAngleDir;
                surfaceEdges[i++] = start + direction * radious;
                surfaceEdges[i++] = end + direction * radious;
            }
            surfaceEdges[i++] = start + cylinder.EndAngleDir * radious;
            surfaceEdges[i++] = end + cylinder.EndAngleDir * radious;
            Handles.DrawPolyLine(surfaceEdges);

        }

        private void DrawArcEditor(CylinderSurface cylinder)
        {
            float radious = cylinder.Radious;
            topArc.angle = cylinder.Angle;
            topArc.radius = radious;
            Matrix4x4 handleMatrix = Matrix4x4.TRS(
                cylinder.StartPoint,
                Quaternion.LookRotation(cylinder.StartAngleDir, cylinder.Direction),
                Vector3.one
            );
            using (new Handles.DrawingScope(handleMatrix))
            {
                EditorGUI.BeginChangeCheck();

                Handles.color = Color.white;
                topArc.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cylinder, "Change Cylinder Properties");
                    cylinder.Angle = topArc.angle;
                    radious = topArc.radius;
                }
            }
        }
    }
}