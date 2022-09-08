using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public enum EAxialDirection
    {
        x = 0, y = 1, z = 2
    }

    [Serializable]
    public class CPlane
    {
        public Vector3Int GridResolution;
        public Vector3 GridOrigin;
        public Vector3 GridSpacing;

        public EAxialDirection NormalDirection;
        public int AxialCoord;
        public Vector2Int PlanarCoordMin, PlanarCoordMax;

        public Color VisualizeColor;
        private GameObject m_Cube;
        private static UnityEngine.Object m_CubePrefab;

        public CPlane() { }

        public CPlane
        (
            Vector3Int vGridRsoulution,
            Vector3 vGridOrigin,
            Vector3 vGridSpacing,
            EAxialDirection vNormalDirection,
            int vAxialCoord,
            Vector2Int vPlanarCoordMin,
            Vector2Int vPlanarCoordMax,
            Color vVisuzlizeColor
        )
        {
            GridResolution = vGridRsoulution;
            GridOrigin = vGridOrigin;
            GridSpacing = vGridSpacing;
            NormalDirection = vNormalDirection;
            AxialCoord = vAxialCoord;
            PlanarCoordMin = vPlanarCoordMin;
            PlanarCoordMax = vPlanarCoordMax;
            VisualizeColor = vVisuzlizeColor;

            limit();
        }

        ~CPlane()
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

        public void limit()
        {
            int AxialResolution = 0;
            Vector2Int PlanarResolution = new Vector2Int(0, 0);

            if (NormalDirection == EAxialDirection.x)
            {
                AxialResolution = GridResolution.x;
                PlanarResolution.x = GridResolution.y;
                PlanarResolution.y = GridResolution.z;
            }
            else if (NormalDirection == EAxialDirection.y)
            {
                AxialResolution = GridResolution.y;
                PlanarResolution.x = GridResolution.z;
                PlanarResolution.y = GridResolution.x;
            }
            else if (NormalDirection == EAxialDirection.z)
            {
                AxialResolution = GridResolution.z;
                PlanarResolution.x = GridResolution.x;
                PlanarResolution.y = GridResolution.y;
            }

            AxialCoord = Mathf.Max(0, Mathf.Min(AxialCoord, AxialResolution));
            PlanarCoordMax.x = Mathf.Max(0, Mathf.Min(PlanarCoordMax.x, PlanarResolution.x - 1));
            PlanarCoordMax.y = Mathf.Max(0, Mathf.Min(PlanarCoordMax.y, PlanarResolution.y - 1));
            PlanarCoordMin.x = Mathf.Max(0, Mathf.Min(PlanarCoordMin.x, PlanarResolution.x - 1));
            PlanarCoordMin.y = Mathf.Max(0, Mathf.Min(PlanarCoordMin.y, PlanarResolution.y - 1));

            if (PlanarCoordMin.x > PlanarCoordMax.x)
                PlanarCoordMin.x = PlanarCoordMax.x = 0;
            if (PlanarCoordMin.y > PlanarCoordMax.y)
                PlanarCoordMin.y = PlanarCoordMax.y = 0;
        }

        public void visualize()
        {
            if (m_Cube == null) 
            {
                __init();

                m_Cube = (GameObject)UnityEngine.Object.Instantiate(m_CubePrefab);
                m_Cube.GetComponent<MeshRenderer>().material.color = VisualizeColor;
            }

            Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);

            if (NormalDirection == EAxialDirection.x)
            {
                Vector2 PlanarMin = new Vector2(GridOrigin.y + GridSpacing.y * (0.5f + PlanarCoordMin.x), GridOrigin.z + GridSpacing.z * (0.5f + PlanarCoordMin.y));
                Vector2 PlanarMax = new Vector2(GridOrigin.y + GridSpacing.y * (0.5f + PlanarCoordMax.x), GridOrigin.z + GridSpacing.z * (0.5f + PlanarCoordMax.y));

                Position.x = GridOrigin.x + GridSpacing.x * (0.5f + AxialCoord);
                Position.y = (PlanarMax.x + PlanarMin.x) / 2;
                Position.z = (PlanarMax.y + PlanarMin.y) / 2;

                Scale.x = 0.01f;
                Scale.y = PlanarMax.x - PlanarMin.x;
                Scale.z = PlanarMax.y - PlanarMin.y;
            }
            else if (NormalDirection == EAxialDirection.y)
            {
                Vector2 PlanarMin = new Vector2(GridOrigin.z + GridSpacing.z * (0.5f + PlanarCoordMin.x), GridOrigin.x + GridSpacing.x * (0.5f + PlanarCoordMin.y));
                Vector2 PlanarMax = new Vector2(GridOrigin.z + GridSpacing.z * (0.5f + PlanarCoordMax.x), GridOrigin.x + GridSpacing.x * (0.5f + PlanarCoordMax.y));

                Position.x = (PlanarMax.y + PlanarMin.y) / 2;
                Position.y = GridOrigin.y + GridSpacing.y * (0.5f + AxialCoord);
                Position.z = (PlanarMax.x + PlanarMin.x) / 2;

                Scale.x = PlanarMax.y - PlanarMin.y;
                Scale.y = 0.01f;
                Scale.z = PlanarMax.x - PlanarMin.x;
            }
            else if (NormalDirection == EAxialDirection.z)
            {
                Vector2 PlanarMin = new Vector2(GridOrigin.x + GridSpacing.x * (0.5f + PlanarCoordMin.x), GridOrigin.y + GridSpacing.y * (0.5f + PlanarCoordMin.y));
                Vector2 PlanarMax = new Vector2(GridOrigin.x + GridSpacing.x * (0.5f + PlanarCoordMax.x), GridOrigin.y + GridSpacing.y * (0.5f + PlanarCoordMax.y));

                Position.x = (PlanarMax.x + PlanarMin.x) / 2;
                Position.y = (PlanarMax.y + PlanarMin.y) / 2;
                Position.z = GridOrigin.z + GridSpacing.z * (0.5f + AxialCoord);

                Scale.x = PlanarMax.x - PlanarMin.x;
                Scale.y = PlanarMax.y - PlanarMin.y;
                Scale.z = 0.01f;
            }

            m_Cube.SetActive(true);
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

    //public static class CStatisticsToolInvokers
    //{
    //    private static bool m_Initialized = false;
    //    private static ComputeShader m_StatisticsTool = null;

    //    private static int m_ComputeFluxFromGridKernel = -1;

    //    private static ComputeBuffer m_TempResult = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
    //    private static ComputeBuffer m_GroupCounter = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Raw);
    //    private static ComputeBuffer m_Zero = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Append);
    //    private static ComputeBuffer m_Flux = new ComputeBuffer(1, sizeof(float));

    //    public static void init()
    //    {
    //        if (m_Initialized) return;

    //        free();

    //        m_StatisticsTool = Resources.Load("Shaders/StatisticsTool") as ComputeShader;

    //        m_ComputeFluxFromGridKernel = m_StatisticsTool.FindKernel("computeFluxFromGrid");

    //        Shader.SetGlobalBuffer("STTempResult", m_TempResult);
    //        Shader.SetGlobalBuffer("STGroupCounter", m_GroupCounter);

    //        m_TempResult = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
    //        m_GroupCounter = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Raw);
    //        m_Zero = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Append);
    //        m_Flux = new ComputeBuffer(1, sizeof(float));

    //        m_Zero.SetCounterValue(0);

    //        m_Initialized = true;
    //    }

    //    public static void free()
    //    {
    //        m_TempResult.Release();
    //        m_GroupCounter.Release();
    //        m_Zero.Release();
    //        m_Flux.Release();

    //        m_Initialized = false;
    //    }

    //    private static void __resetGroupCounter()
    //    {
    //        ComputeBuffer.CopyCount(m_Zero, m_GroupCounter, 0);
    //    }

    //    public static float ComputeFluxFromGridInvoker
    //    (
    //        CFaceCenteredVectorField vVelField,
    //        CCellCenteredScalarField vFluidDomainField,
    //        CPlane vPlane
    //    )
    //    {
    //        if (!m_Initialized) init();

    //        int NormalDirection = (int)vPlane.NormalDirection;

    //        vPlane.GridResolution = vVelField.getResolution();
    //        vPlane.limit();

    //        Vector3Int Resolution = vVelField.getResolution();
    //        Vector3 Origin = new Vector3(0.0f, 0.0f, 0.0f);
    //        Vector3 Spacing=vVelField.getSpacing();
    //        ComputeBuffer VelFieldData = null;

    //        if(NormalDirection==0)
    //        {
    //            Origin = vVelField.getOrigin() - new Vector3(0.5f * Spacing.x, 0.0f, 0.0f);
    //            VelFieldData = vVelField.getGridDataX();
    //        }
    //        else if (NormalDirection == 1)
    //        {
    //            Origin = vVelField.getOrigin() - new Vector3(0.0f, 0.5f * Spacing.y, 0.0f);
    //            VelFieldData = vVelField.getGridDataY();
    //        }
    //        else if (NormalDirection == 2)
    //        {
    //            Origin = vVelField.getOrigin() - new Vector3(0.0f, 0.0f, 0.5f * Spacing.z);
    //            VelFieldData = vVelField.getGridDataZ();
    //        }

    //        int TotalThreadsNum = (vPlane.PlanarCoordMax.x - vPlane.PlanarCoordMin.x) * (vPlane.PlanarCoordMax.y - vPlane.PlanarCoordMin.y);
    //        int TotalGroupsNum, ThreadsNumPerGroup;
    //        CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadsNum, 1024, out ThreadsNumPerGroup, out TotalGroupsNum);

    //        __resetGroupCounter();
    //        m_StatisticsTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
    //        m_StatisticsTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
    //        m_StatisticsTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
    //        m_StatisticsTool.SetInt("vTotalGroupsNum", TotalGroupsNum);
    //        m_StatisticsTool.SetInt("vNormalDirection", NormalDirection);
    //        m_StatisticsTool.SetInt("vAxialCoord", vPlane.AxialCoord);
    //        m_StatisticsTool.SetInts("vPlanarCoordMin", vPlane.PlanarCoordMin.x, vPlane.PlanarCoordMin.y);
    //        m_StatisticsTool.SetInts("vPlanarCoordMax", vPlane.PlanarCoordMax.x, vPlane.PlanarCoordMax.y);
    //        m_StatisticsTool.SetBuffer(m_ComputeFluxFromGridKernel, "vFluidDomainFieldData", vFluidDomainField.getGridData());
    //        m_StatisticsTool.SetBuffer(m_ComputeFluxFromGridKernel, "vVelFieldData", VelFieldData);
    //        m_StatisticsTool.SetBuffer(m_ComputeFluxFromGridKernel, "voFlux", m_Flux);

    //        CGlobalMacroAndFunc.dispatchKernel(m_StatisticsTool, m_ComputeFluxFromGridKernel, TotalThreadsNum);

    //        float[] FluxArray = new float[1];
    //        m_Flux.GetData(FluxArray);
    //        float Result = FluxArray[0];

    //        return Result;
    //    }
    //}
}