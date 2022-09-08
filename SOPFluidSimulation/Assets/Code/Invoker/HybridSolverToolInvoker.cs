using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public static class CHybridSolverToolInvoker
    {
        public static void mixFieldWithDensityInvoker
        (
	        CFaceCenteredVectorField vVectorFieldA,
	        CFaceCenteredVectorField voVectorFieldB,
	        CCellCenteredScalarField vWeightFieldA,
	        CCellCenteredScalarField vWeightFieldB
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = vVectorFieldA.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);

            CGlobalMacroAndFunc._ASSERTE(Resolution == voVectorFieldB.getResolution(), "混合速度时输入场的维度不同!");
            CGlobalMacroAndFunc._ASSERTE(Resolution == vWeightFieldB.getResolution(), "混合速度时输入场的维度不同!");
            CGlobalMacroAndFunc._ASSERTE(Resolution == vWeightFieldA.getResolution(), "混合速度时输入场的维度不同!");

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            int ThreadsPerBlockX, BlocksPerGridX;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumX, out ThreadsPerBlockX, out BlocksPerGridX);
            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            int ThreadsPerBlockY, BlocksPerGridY;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumY, out ThreadsPerBlockY, out BlocksPerGridY);
            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
            int ThreadsPerBlockZ, BlocksPerGridZ;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumZ, out ThreadsPerBlockZ, out BlocksPerGridZ);

            m_HybridSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_HybridSolverTool.SetBuffer(m_MixFieldXWithDensityKernel, "vWeightFieldA", vWeightFieldA.getGridData());
            m_HybridSolverTool.SetBuffer(m_MixFieldXWithDensityKernel, "vWeightFieldB", vWeightFieldB.getGridData());
            m_HybridSolverTool.SetBuffer(m_MixFieldYWithDensityKernel, "vWeightFieldA", vWeightFieldA.getGridData());
            m_HybridSolverTool.SetBuffer(m_MixFieldYWithDensityKernel, "vWeightFieldB", vWeightFieldB.getGridData());
            m_HybridSolverTool.SetBuffer(m_MixFieldZWithDensityKernel, "vWeightFieldA", vWeightFieldA.getGridData());
            m_HybridSolverTool.SetBuffer(m_MixFieldZWithDensityKernel, "vWeightFieldB", vWeightFieldB.getGridData());

            m_HybridSolverTool.SetBuffer(m_MixFieldXWithDensityKernel, "vVectorFieldAX", vVectorFieldA.getGridDataX());
            m_HybridSolverTool.SetBuffer(m_MixFieldYWithDensityKernel, "vVectorFieldAY", vVectorFieldA.getGridDataY());
            m_HybridSolverTool.SetBuffer(m_MixFieldZWithDensityKernel, "vVectorFieldAZ", vVectorFieldA.getGridDataZ());

            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNumX);
            m_HybridSolverTool.SetBuffer(m_MixFieldXWithDensityKernel, "vioVectorFieldBX", voVectorFieldB.getGridDataX());
            m_HybridSolverTool.Dispatch(m_MixFieldXWithDensityKernel, BlocksPerGridX, 1, 1);

            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNumY);
            m_HybridSolverTool.SetBuffer(m_MixFieldYWithDensityKernel, "vioVectorFieldBY", voVectorFieldB.getGridDataY());
            m_HybridSolverTool.Dispatch(m_MixFieldYWithDensityKernel, BlocksPerGridY, 1, 1);

            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNumZ);
            m_HybridSolverTool.SetBuffer(m_MixFieldZWithDensityKernel, "vioVectorFieldBZ", voVectorFieldB.getGridDataZ());
            m_HybridSolverTool.Dispatch(m_MixFieldZWithDensityKernel, BlocksPerGridZ, 1, 1);
        }

        public static void buildFluidOutsideSDFInvoker
        (
	        CCellCenteredScalarField vFluidDensityField,
	        CCellCenteredScalarField vSolidDomainField,
	        CCellCenteredScalarField voFluidOutsideSDFField,
	        int vExtrapolationDistance
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = vSolidDomainField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == vFluidDensityField.getResolution());
            CGlobalMacroAndFunc._ASSERTE(Resolution == voFluidOutsideSDFField.getResolution());

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            m_HybridSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_HybridSolverTool.SetBuffer(m_BuildFluidOutsideSDFKernel, "vSolidDomainFieldData", vSolidDomainField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidOutsideSDFKernel, "vFluidDensityFieldData", vFluidDensityField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidOutsideSDFKernel, "vioFluidOutsideSDFFieldData", voFluidOutsideSDFField.getGridData());

            for (int i = 0; i < vExtrapolationDistance; i++)
            {
                m_HybridSolverTool.SetInt("CurrentDis", i);

                m_HybridSolverTool.Dispatch(m_BuildFluidOutsideSDFKernel, BlocksPerGrid, 1, 1);
            }

            m_HybridSolverTool.SetBuffer(m_BuildFluidSDFPostProcessKernel, "vioFluidSDFFieldData", voFluidOutsideSDFField.getGridData());
            m_HybridSolverTool.Dispatch(m_BuildFluidSDFPostProcessKernel, BlocksPerGrid, 1, 1);
        }

        public static void buildFluidInsideSDFInvoker
		(
	        CCellCenteredScalarField vFluidDensityField,
	        CCellCenteredScalarField vSolidDomainField,
	        CCellCenteredScalarField voFluidInsideSDFField,
	        int vExtrapolationDistance
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = vSolidDomainField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == vFluidDensityField.getResolution());
            CGlobalMacroAndFunc._ASSERTE(Resolution == voFluidInsideSDFField.getResolution());

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            m_HybridSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_HybridSolverTool.SetBuffer(m_BuildFluidInsideSDFKernel, "vSolidDomainFieldData", vSolidDomainField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidInsideSDFKernel, "vFluidDensityFieldData", vFluidDensityField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidInsideSDFKernel, "vioFluidInsideSDFFieldData", voFluidInsideSDFField.getGridData());

            for (int i = 0; i < vExtrapolationDistance; i++)
            {
                m_HybridSolverTool.SetInt("CurrentDis", i);

                m_HybridSolverTool.Dispatch(m_BuildFluidInsideSDFKernel, BlocksPerGrid, 1, 1);
            }

            m_HybridSolverTool.SetBuffer(m_BuildFluidSDFPostProcessKernel, "vioFluidSDFFieldData", voFluidInsideSDFField.getGridData());
            m_HybridSolverTool.Dispatch(m_BuildFluidSDFPostProcessKernel, BlocksPerGrid, 1, 1);
        }

        public static void buildMixedFluidOutsideSDFInvoker
		(
	        CCellCenteredScalarField vGridFluidDensityField,
	        CCellCenteredScalarField vMixedFluidDensityField,
	        CCellCenteredScalarField vGridFluidOutsideSDFField,
	        CCellCenteredScalarField voMixedFluidOutsideSDFField
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = vGridFluidDensityField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == vMixedFluidDensityField.getResolution(), "����ܶȳ�ʱ���볡ά�Ȳ�ƥ�䣡");
            CGlobalMacroAndFunc._ASSERTE(Resolution == vGridFluidOutsideSDFField.getResolution(), "����ܶȳ�ʱ���볡ά�Ȳ�ƥ�䣡");
            CGlobalMacroAndFunc._ASSERTE(Resolution == voMixedFluidOutsideSDFField.getResolution(), "����ܶȳ�ʱ���볡ά�Ȳ�ƥ�䣡");

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            m_HybridSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_HybridSolverTool.SetBuffer(m_BuildMixedFluidOutsideSDFKernel, "vGridFluidDensityFieldData", vGridFluidDensityField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildMixedFluidOutsideSDFKernel, "vMixedFluidDensityFieldData", vMixedFluidDensityField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildMixedFluidOutsideSDFKernel, "vGridFluidOutsideSDFFieldData", vGridFluidOutsideSDFField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildMixedFluidOutsideSDFKernel, "vioMixedFluidOutsideSDFFieldData", voMixedFluidOutsideSDFField.getGridData());

            m_HybridSolverTool.Dispatch(m_BuildMixedFluidOutsideSDFKernel, BlocksPerGrid, 1, 1);
        }

        public static void buildFluidDensityInvoker
        (
            CCellCenteredScalarField vFluidDomainField, 
	        CCellCenteredScalarField vSolidDomainField,
	        CCellCenteredScalarField voFluidDensityField
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = voFluidDensityField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == vFluidDomainField.getResolution());
            CGlobalMacroAndFunc._ASSERTE(Resolution == vSolidDomainField.getResolution());

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            m_HybridSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_HybridSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_HybridSolverTool.SetBuffer(m_BuildFluidDensityKernel, "vSolidDomainFieldData", vSolidDomainField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidDensityKernel, "vFluidDomainFieldData", vFluidDomainField.getGridData());
            m_HybridSolverTool.SetBuffer(m_BuildFluidDensityKernel, "vioFluidDensityFieldData", voFluidDensityField.getGridData());

            m_HybridSolverTool.Dispatch(m_BuildFluidDensityKernel, BlocksPerGrid, 1, 1);
        }

        private static bool m_Initiated = false;
        private static ComputeShader m_HybridSolverTool = null;
        private static int m_MixFieldXWithDensityKernel = -1;
        private static int m_MixFieldYWithDensityKernel = -1;
        private static int m_MixFieldZWithDensityKernel = -1;
        private static int m_BuildFluidDensityKernel = -1;
        private static int m_BuildFluidOutsideSDFKernel = -1;
        private static int m_BuildFluidInsideSDFKernel = -1;
        private static int m_BuildFluidSDFPostProcessKernel = -1;
        private static int m_BuildMixedFluidOutsideSDFKernel = -1;

        public static void init()
        {
            if (m_Initiated) return;
            m_Initiated = true;
            m_HybridSolverTool = Resources.Load("Shaders/HybridSolverTool") as ComputeShader;

            m_MixFieldXWithDensityKernel = m_HybridSolverTool.FindKernel("mixFieldXWithDensity");
            m_MixFieldYWithDensityKernel = m_HybridSolverTool.FindKernel("mixFieldYWithDensity");
            m_MixFieldZWithDensityKernel = m_HybridSolverTool.FindKernel("mixFieldZWithDensity");
            m_BuildFluidDensityKernel = m_HybridSolverTool.FindKernel("buildFluidDensity");
            m_BuildFluidOutsideSDFKernel = m_HybridSolverTool.FindKernel("buildFluidOutsideSDF");
            m_BuildFluidInsideSDFKernel = m_HybridSolverTool.FindKernel("buildFluidInsideSDF");
            m_BuildFluidSDFPostProcessKernel = m_HybridSolverTool.FindKernel("buildFluidSDFPostProcess");
            m_BuildMixedFluidOutsideSDFKernel = m_HybridSolverTool.FindKernel("buildMixedFluidOutsideSDF");
        }
    }
}