using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    [Serializable]
    public class CEmitter
    {
        public SBoundingBox BoundingBox;
        public EAxialDirection Direction = EAxialDirection.x;
        public float Velocity = 0.0f;
        public int ParticlesNumPerGrid = 8;
        public bool IsActive = true;

        public Color VisualizeColor = new Color(1, 0, 0, 0.5f);
        private GameObject m_Cube;
        private static UnityEngine.Object m_CubePrefab;

        public CEmitter() { }

        public CEmitter(SBoundingBox vBoundingBox, EAxialDirection vDirection, float vVelocity, int vParticlesNumPerGrid)
        {
            BoundingBox = vBoundingBox;
            Direction = vDirection;
            Velocity = vVelocity;
            ParticlesNumPerGrid = vParticlesNumPerGrid;
        }

        ~CEmitter()
        {
            GameObject.Destroy(m_Cube);
        }

        private static void __init()
        {
            if (m_CubePrefab == null)
            {
                m_CubePrefab = Resources.Load("Prefabs/VisualizedGrid");
            }
        }

        public SBoundingBox getEmitBox(float vDeltaT)
        {
            float DistancePerFrame = Mathf.Abs(Velocity) * vDeltaT;
            SBoundingBox Result = BoundingBox;

            if (Direction == EAxialDirection.x)
            {
                if (Velocity >= 0)
                {
                    Result.Min.x = Result.Max.x - DistancePerFrame;
                }
                else
                {
                    Result.Max.x = Result.Min.x + DistancePerFrame;
                }
            }
            else if (Direction == EAxialDirection.y)
            {
                if (Velocity >= 0)
                {
                    Result.Min.y = Result.Max.y - DistancePerFrame;
                }
                else
                {
                    Result.Max.y = Result.Min.y + DistancePerFrame;
                }
            }
            else if (Direction == EAxialDirection.z)
            {
                if (Velocity >= 0)
                {
                    Result.Min.z = Result.Max.z - DistancePerFrame;
                }
                else
                {
                    Result.Max.z = Result.Min.z + DistancePerFrame;
                }
            }

            return Result;
        }

        public Vector3 getVelocityVector()
        {
            Vector3 Result = Vector3.zero;

            if (Direction == EAxialDirection.x) 
            {
                Result.x = Velocity;
            }
            else if (Direction == EAxialDirection.y)
            {
                Result.y = Velocity;
            }
            else if (Direction == EAxialDirection.z)
            {
                Result.z = Velocity;
            }

            return Result;
        }

        public void visualize()
        {
            if (m_Cube == null)
            {
                __init();

                m_Cube = (GameObject)UnityEngine.Object.Instantiate(m_CubePrefab);
                m_Cube.GetComponent<MeshRenderer>().material.color = VisualizeColor;
            }

            Vector3 Position = (BoundingBox.Min + BoundingBox.Max) / 2;
            Vector3 Scale = BoundingBox.Max - BoundingBox.Min;

            m_Cube.transform.position = Position;
            m_Cube.transform.localScale = Scale;
        }

        public void disVisualize()
        {
            if (m_Cube != null)
            {
                m_Cube.SetActive(false);
            }
        }
    }
}