using System;
using UnityEngine;

namespace EulerFluidEngine
{
    [Serializable]
    public struct SBoundingBox
    {
        public Vector3 Min, Max;

        public SBoundingBox(Vector3 vMin, Vector3 vMax)
        {
            Min = vMin;
            Max = vMax;
        }
    }

    public enum ESamplingAlgorithm
    {
        LINEAR,
        CATMULLROM,
        MONOCATMULLROM,
        CUBICBRIDSON,
        CLAMPCUBICBRIDSON
    }

    public enum EPGTransferAlgorithm
    {
	    LINEAR,
	    QUADRATIC,
	    CUBIC
    }

    public enum EAdvectionAccuracy
    {
        RK1,
        RK2,
        RK3
    }

    public enum EAdvectionAlgorithm
    {
        SemiLagrangian,
        MacCormack,
        Iteration,
        PIC,
        FLIP,
        MixPICAndFLIP,
        Other_Invalid
    }

    public enum EPressureAlgorithm
    {
        FirstOrder,
        FirstOrder_Smoke,
        SecondOrder,
        SecondOrder_Smoke,
        Reflection,
        Reflection_Smoke,
        SecondOrderReflection,
        SecondOrderReflection_Smoke,
        BDF2,
        BDF2_Smoke,
        BDF2AndSecondOrder,
        CrankNicolson,
        MaterialAcceleration,
        Test
    }

    public enum ESmokeScene
    {
        NULL,
        TwoDSmoke,
        ThreeDSmoke,
        VortexCollision,
        VortexLeapFrogging
    }

    public delegate Vector3 EmitteVelFunc(Vector3 vPos);
    public delegate Vector3 EmitterVelFunc(int vCurSimulationFrame);

    public static class CGlobalMacroAndFunc
    {
        public const float M_PI = 3.1415926f;

        public const int FLT_STRIDE = sizeof(float);
        public const int INT_STRIDE = sizeof(int);

        public const int NUM_OF_THREADS_PER_BLOCK = 512;
        public const int MAX_NUM_OF_THREADS_PER_BLOCK = 1024;
        public const int MAX_NUM_OF_BLOCKS = 65535;
        public const int MAX_NUM_OF_THREADS = MAX_NUM_OF_BLOCKS * MAX_NUM_OF_THREADS_PER_BLOCK;
        public const int MAX_GRIDS_NUM = 200 * 400 * 400;
        public const int MAX_PARTICLE_NUM = 256 * 256 * 256 * 8 / 2;
        public const int MAX_PARTICLE_DATA_NUM = 256 * 256 * 256 * 8 * 3 / 2;

        public const float GRID_SOLVER_EPSILON_FOR_TEST = 1e-4f;
        public const float GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL = 1.0f;
        public const float FLUID_SURFACE_DENSITY = 0.2f;
        public const float FLUID_DOMAIN_VALUE = 100;
        public const float ATMOICCOFF = 1e+6f;
        public const float MINATOMICCOFF = 1e-6f;

        public static void init()
        {
            CMathTool.init();
            CMatrixFreePCGToolInvokers.init();

            CBoundaryToolInvoker.init();
            CFieldMathToolInvoker.init();
            CEulerParticlesInvokers.init();
            CEulerSolverToolInvoker.init();

            CHybridSolverToolInvoker.init();
        }

        public static void free()
        {
            CMathTool.free();
            CMatrixFreePCGToolInvokers.free();

            CEulerParticlesInvokers.free();
        }

        public static void fetchPropBlockGridSize1D(int vTotalThreadNum, out int voNumOfThreadsPerBlock, out int voNumOfBlocksPerGrid)
        {
            if (vTotalThreadNum == 0)
            {
                Debug.LogError("申请的线程数为零！");
                voNumOfThreadsPerBlock = 0;
                voNumOfBlocksPerGrid = 0;
                return;
            }

            voNumOfThreadsPerBlock = NUM_OF_THREADS_PER_BLOCK;

            if (vTotalThreadNum <= NUM_OF_THREADS_PER_BLOCK)
            {
                voNumOfBlocksPerGrid = 1;
            }
            else
            {
                if (vTotalThreadNum % NUM_OF_THREADS_PER_BLOCK == 0)
                {
                    voNumOfBlocksPerGrid = vTotalThreadNum / NUM_OF_THREADS_PER_BLOCK;
                }
                else
                {
                    voNumOfBlocksPerGrid = vTotalThreadNum / NUM_OF_THREADS_PER_BLOCK + 1;
                }
            }
        }

        public static void fetchPropBlockGridSize1D(int vTotalThreadNum, int vNumOfThreadsPerBlock, out int voNumOfThreadsPerBlock, out int voNumOfBlocksPerGrid)
        {
            if (vTotalThreadNum == 0)
            {
                Debug.LogError("申请的线程数为零！");
                voNumOfThreadsPerBlock = 0;
                voNumOfBlocksPerGrid = 0;
                return;
            }

            voNumOfThreadsPerBlock = vNumOfThreadsPerBlock;

            if (vTotalThreadNum <= vNumOfThreadsPerBlock)
            {
                voNumOfBlocksPerGrid = 1;
            }
            else
            {
                if (vTotalThreadNum % vNumOfThreadsPerBlock == 0)
                {
                    voNumOfBlocksPerGrid = vTotalThreadNum / vNumOfThreadsPerBlock;
                }
                else
                {
                    voNumOfBlocksPerGrid = vTotalThreadNum / vNumOfThreadsPerBlock + 1;
                }
            }
        }

        public static void dispatchKernel(ComputeShader vComputeShader, int vKernel, int vTotalThreadNum)
        {
            vComputeShader.SetInt("TotalThreadNum", vTotalThreadNum);

            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;
            fetchPropBlockGridSize1D(vTotalThreadNum, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

            vComputeShader.Dispatch(vKernel, NumOfBlocksPerGrid, 1, 1);
        }

        public static void dispatchKernel(ComputeShader vComputeShader, int vKernel, int vTotalThreadNum, int vNumOfThreadsPerBlock)
        {
            vComputeShader.SetInt("TotalThreadNum", vTotalThreadNum);

            int NumOfThreadsPerBlock = 0, NumOfBlocksPerGrid = 0;
            fetchPropBlockGridSize1D(vTotalThreadNum, vNumOfThreadsPerBlock, out NumOfThreadsPerBlock, out NumOfBlocksPerGrid);

            vComputeShader.Dispatch(vKernel, NumOfBlocksPerGrid, 1, 1);
        }

        public static void _ASSERTE(bool vFlag, string vErrorLog = "")
        {
            if (!vFlag)
            {
                Debug.LogError(vErrorLog);
            }
        }
    }
}
