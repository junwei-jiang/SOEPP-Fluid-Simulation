using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
	public struct SFdmLinearSystem
	{
		public Vector3Int Resolution;
		public Vector3 Spacing;
		public Vector3 Origin;
		public ComputeBuffer FdmMatrixA;
		public ComputeBuffer FdmVectorx;
		public ComputeBuffer FdmVectorb;
	};

	public enum RedBlack
	{
		Red = 0,
		Black = 1
    };

	public class CPressureSolver
    {
		#region Constructor&GetSet
		public CPressureSolver(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
			resizePressureSolver(vResolution, vOrigin, vSpacing);
		}

		~CPressureSolver()
        {
			free();
		}

		public void free()
        {
			if (m_FdmLinearSystem.FdmMatrixA != null)
			{
				m_FdmLinearSystem.FdmMatrixA.Release();
			}
			if (m_FdmLinearSystem.FdmVectorx != null)
			{
				m_FdmLinearSystem.FdmVectorx.Release();
			}
			if (m_FdmLinearSystem.FdmVectorb != null)
			{
				m_FdmLinearSystem.FdmVectorb.Release();
			}
			if (m_PressureField != null)
			{
				m_PressureField.free();
			}
			if (m_KineticEnergyField != null)
			{
				m_KineticEnergyField.free();
			}
			if (m_TempCCVelField != null)
			{
				m_TempCCVelField.free();
			}
			if (m_PressureFieldPrevious != null)
			{
				m_PressureFieldPrevious.free();
			}
			if (m_PressureFieldTemp != null)
			{
				m_PressureFieldTemp.free(); 
			}
			if (m_TempPressureGradientField != null)
			{
				m_TempPressureGradientField.free();
			}
			if (m_TempTempPressureGradientField != null)
			{
				m_TempTempPressureGradientField.free();
			}
			if (m_PressureGradientField != null)
			{
				m_PressureGradientField.free();
			}
			if (m_PressureGradientFieldX != null)
			{
				m_PressureGradientFieldX.free();
			}
			if (m_PressureGradientFieldY != null)
			{
				m_PressureGradientFieldY.free();
			}
			if (m_PressureGradientFieldZ != null)
			{
				m_PressureGradientFieldZ.free();
			}
		}

        public ComputeBuffer getFdmMatrixA()
        {
			return m_FdmLinearSystem.FdmMatrixA;
		}
		public ComputeBuffer getVectorx()
        {
			return m_FdmLinearSystem.FdmVectorx;
		}
		public ComputeBuffer getVectorb()
        {
			return m_FdmLinearSystem.FdmVectorb;
		}
		public ComputeBuffer getMarkers()
        {
			return m_MarkersField.getGridData();
        }
		public CCellCenteredScalarField getMarkersField()
        {
			return m_MarkersField;
        }
		public CMatrixFreePCG getPCGSolver()
        {
			return m_MartixFreePCGSolver;
        }

		public CCellCenteredScalarField getPressureField()
        {
			return m_PressureField;
        }

		public CCellCenteredVectorField getTempPressureGradientField()
        {
			return m_TempPressureGradientField;
        }

		public CCellCenteredVectorField getPressureGradientField()
		{
			return m_PressureGradientField;
		}

		public void resizePressureSolver(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
			m_FdmLinearSystem.Resolution = vResolution;
			m_FdmLinearSystem.Spacing = vSpacing;
			m_FdmLinearSystem.Origin = vOrigin;

			float[] VectorInitialValueA = new float[4 * vResolution.x * vResolution.y * vResolution.z];
			float[] VectorInitialValueB = new float[vResolution.x * vResolution.y * vResolution.z];

			if(m_FdmLinearSystem.FdmMatrixA != null)
            {
				m_FdmLinearSystem.FdmMatrixA.Release();
			}
			if (m_FdmLinearSystem.FdmVectorx != null)
			{
				m_FdmLinearSystem.FdmVectorx.Release();
			}
			if (m_FdmLinearSystem.FdmVectorb != null)
            {
				m_FdmLinearSystem.FdmVectorb.Release();
			}

			m_FdmLinearSystem.FdmMatrixA = new ComputeBuffer(4 * vResolution.x * vResolution.y * vResolution.z, sizeof(float));
			m_FdmLinearSystem.FdmVectorx = new ComputeBuffer(vResolution.x * vResolution.y * vResolution.z, sizeof(float));
			m_FdmLinearSystem.FdmVectorb = new ComputeBuffer(vResolution.x * vResolution.y * vResolution.z, sizeof(float));
			m_FdmLinearSystem.FdmMatrixA.SetData(VectorInitialValueA);
			m_FdmLinearSystem.FdmVectorx.SetData(VectorInitialValueB);
			m_FdmLinearSystem.FdmVectorb.SetData(VectorInitialValueB);

			if (m_MarkersField == null)
			{
				m_MarkersField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_MarkersField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_VelDivergenceField == null)
            {
				m_VelDivergenceField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
            else
            {
				m_VelDivergenceField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_PressureField == null)
			{
				m_PressureField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_PressureField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_TempCCVelField == null)
			{
				m_TempCCVelField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_TempCCVelField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_KineticEnergyField == null)
			{
				m_KineticEnergyField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_KineticEnergyField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_PressureFieldPrevious == null)
			{
				m_PressureFieldPrevious = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_PressureFieldPrevious.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_PressureFieldTemp == null)
			{
				m_PressureFieldTemp = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_PressureFieldTemp.resize(vResolution, vOrigin, vSpacing);
			}
			
			if (m_TempPressureGradientField == null)
			{
				m_TempPressureGradientField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_TempPressureGradientField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_TempTempPressureGradientField == null)
			{
				m_TempTempPressureGradientField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_TempTempPressureGradientField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_PressureGradientField == null)
			{
				m_PressureGradientField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
			}
			else
			{
				m_PressureGradientField.resize(vResolution, vOrigin, vSpacing);
			}

			if (m_PressureGradientFieldX == null)
			{
				m_PressureGradientFieldX = new CCellCenteredVectorField(vResolution + new Vector3Int(1, 0, 0), vOrigin, vSpacing);
			}
			else
			{
				m_PressureGradientFieldX.resize(vResolution + new Vector3Int(1, 0, 0), vOrigin, vSpacing);
			}

			if (m_PressureGradientFieldY == null)
			{
				m_PressureGradientFieldY = new CCellCenteredVectorField(vResolution + new Vector3Int(0, 1, 0), vOrigin, vSpacing);
			}
			else
			{
				m_PressureGradientFieldY.resize(vResolution + new Vector3Int(0, 1, 0), vOrigin, vSpacing);
			}

			if (m_PressureGradientFieldZ == null)
			{
				m_PressureGradientFieldZ = new CCellCenteredVectorField(vResolution + new Vector3Int(0, 0, 1), vOrigin, vSpacing);
			}
			else
			{
				m_PressureGradientFieldZ.resize(vResolution + new Vector3Int(0, 0, 1), vOrigin, vSpacing);
			}

			m_MartixFreePCGSolver = new CMatrixFreePCG();
			m_MartixFreePCGSolver.init(vResolution.x * vResolution.y * vResolution.z, PressureAxCGProd, PressureMinvxCGProd);
			m_MartixFreePCGSolver.setIterationNum(vResolution.x * vResolution.y * vResolution.z);
		}

		#endregion

		#region Public Core Methods
		public void executeHelmholtzHodgDecomposition
		(
			CFaceCenteredVectorField vioFluidVelField,
			float vDeltaT,
			CFaceCenteredVectorField vSolidVelField,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField
		)
		{
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			//__addFluid(vFluidSDFField);//
			//__removeFluid(vFluidSDFField);
			__buildMarkers(vSolidSDFField, vFluidSDFField);
			//__setFluidVel(vioFluidVelField, 1.0f * CGlobalMacroAndFunc.M_PI);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			//m_MartixFreePCGSolver.solveCG(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient(vDeltaT, vioFluidVelField, vSolidVelField);
			__updatePressure();
			//__updatePressureGradient();
			//__updatePressureGradient();
			__correctPressureBoundaryCondition(RedBlack.Red);
			__correctPressureBoundaryCondition(RedBlack.Black);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
		}
		public void executeHelmholtzHodgDecomposition_MA
		(
			CFaceCenteredVectorField vioFluidVelField,
			float vDeltaT,
			CFaceCenteredVectorField vSolidVelField,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField
		)
		{
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			//__addFluid(vFluidSDFField);//
			//__removeFluid(vFluidSDFField);
			__buildMarkers(vSolidSDFField, vFluidSDFField);
			//__setFluidVel(vioFluidVelField, 1.0f * CGlobalMacroAndFunc.M_PI);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			//m_MartixFreePCGSolver.solveCG(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient(2.0f * vDeltaT / 3.0f, vioFluidVelField, vSolidVelField);
			__updatePressure();
			//__updatePressureGradient();
			//__updatePressureGradient();
			__correctPressureBoundaryCondition(RedBlack.Red);
			__correctPressureBoundaryCondition(RedBlack.Black);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
		}
		public void executeHelmholtzHodgDecomposition_CN
		(
			CFaceCenteredVectorField vioFluidVelField,
			float vDeltaT,
			CFaceCenteredVectorField vSolidVelField,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField
		)
		{
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			//__addFluid(vFluidSDFField);//
			//__removeFluid(vFluidSDFField);
			__buildMarkers(vSolidSDFField, vFluidSDFField);
			//__setFluidVel(vioFluidVelField, 1.0f * CGlobalMacroAndFunc.M_PI);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			//m_PressureFieldPrevious.resize(m_MarkersField.getResolution(), m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient_CN(vDeltaT, vioFluidVelField, vSolidVelField);
			m_PressureFieldPrevious.resize(m_MarkersField.getResolution(), m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient(vDeltaT, vioFluidVelField, vSolidVelField);
			m_PressureFieldTemp.resize(m_MarkersField.getResolution(), m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			m_PressureFieldPrevious.plusAlphaX(m_PressureFieldTemp, 1.0f);
			__updatePressure();
			__correctPressureBoundaryCondition(RedBlack.Red);
			__correctPressureBoundaryCondition(RedBlack.Black);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			m_FirstFrameFlag = false;
		}

		public void executeHelmholtzHodgDecomposition_CN2
		(
			CFaceCenteredVectorField vioFluidVelField,
			float vDeltaT,
			CFaceCenteredVectorField vSolidVelField,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField
		)
		{
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);

			__buildMarkers(vSolidSDFField, vFluidSDFField);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient(vDeltaT, vioFluidVelField, vSolidVelField);
			__updatePressure();
			__updatePressure();
			__correctPressureBoundaryCondition(RedBlack.Red);
			__correctPressureBoundaryCondition(RedBlack.Black);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);

			//if(m_FirstFrameFlag)
			//         {
			//	executeHelmholtzHodgDecomposition(vioFluidVelField, vDeltaT, vSolidVelField, vSolidSDFField, vFluidSDFField);
			//	m_PressureFieldPrevious.resize(m_PressureFieldPrevious.getResolution(), m_PressureFieldPrevious.getOrigin(), m_PressureFieldPrevious.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			//	//压力场没有外扩，下一帧对流之后可能有地方的压力是无效的。
			//	m_FirstFrameFlag = false;
			//}
			//         else
			//         {
			//	__buildMarkers(vSolidSDFField, vFluidSDFField);
			//	__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			//	__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			//	m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			//	//__updatePressure();
			//	//__correctPressureBoundaryCondition(RedBlack.Red);
			//	//__correctPressureBoundaryCondition(RedBlack.Black);

			//	m_PressureFieldTemp.resize(m_PressureFieldPrevious.getResolution(), m_PressureFieldPrevious.getOrigin(), m_PressureFieldPrevious.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			//	m_PressureFieldTemp.scale(0.5f);
			//	m_PressureFieldTemp.plusAlphaX(m_PressureFieldPrevious, 0.5f);
			//	Vector3 Scale = new Vector3(vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.x), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.y), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.z));
			//	CEulerSolverToolInvoker.applyPotentialGradientInvoker(m_PressureField.getResolution(), Scale, m_MarkersField, m_PressureFieldTemp.getGridData(), vioFluidVelField, vSolidVelField);

			//	//__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			//	//m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			//	__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			//	m_PressureFieldPrevious.resize(m_PressureFieldPrevious.getResolution(), m_PressureFieldPrevious.getOrigin(), m_PressureFieldPrevious.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			//}
		}

		public void executeHelmholtzHodgDecomposition
		(
			CFaceCenteredVectorField vioFluidVelField,
			float vDeltaT,
			CFaceCenteredVectorField vSolidVelField,
			CCellCenteredScalarField vSolidSDFField,
			ComputeBuffer vParticlesPos,
			CEmitter vEmitter = null
		)
		{
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getResolution() == m_FdmLinearSystem.Resolution);
			CGlobalMacroAndFunc._ASSERTE(vioFluidVelField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getSpacing() == m_FdmLinearSystem.Spacing);
			CGlobalMacroAndFunc._ASSERTE(vSolidVelField.getSpacing() == m_FdmLinearSystem.Spacing);

			if (vEmitter != null && vEmitter.IsActive) 
			{
				__buildMarkers(vSolidSDFField, vParticlesPos, vEmitter.BoundingBox);
				vSolidVelField.setDataWithBoundingBox(vEmitter.BoundingBox, vEmitter.getVelocityVector());
			}
			else
			{
				__buildMarkers(vSolidSDFField, vParticlesPos);
			}
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
			__buildSystem(vDeltaT, vioFluidVelField, vSolidVelField);
			m_PressureFieldPrevious.resize(m_MarkersField.getResolution(), m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			//m_MartixFreePCGSolver.solveCG(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
			__applyPotentialGradient(vDeltaT, vioFluidVelField, vSolidVelField);
			__updatePressure();
			__correctPressureBoundaryCondition(RedBlack.Red);
			__correctPressureBoundaryCondition(RedBlack.Black);
			__correctVelBoundaryCondition(vioFluidVelField, vSolidVelField);
		}

		public float calculateKineticEnergy(CFaceCenteredVectorField vioFluidVelField, Vector3 vXYZMin, Vector3 vXYZMax)
		{
			Vector3Int Resolution = m_FdmLinearSystem.Resolution;
			Vector3 Spacing = m_FdmLinearSystem.Spacing;
			Vector3 Origin = m_FdmLinearSystem.Origin;

			vioFluidVelField.transfer2CCVField(m_TempCCVelField);
			CEulerSolverToolInvoker.calculateKineticEnergyField(Resolution, Origin, Spacing, vXYZMin, vXYZMax, m_MarkersField, m_TempCCVelField, m_KineticEnergyField);
			m_KineticEnergy = CMathTool.sum(m_KineticEnergyField.getGridData(), m_KineticEnergyField.getGridData().count);
			return m_KineticEnergy;
		}
		#endregion

		#region Private Core Methods
		private void __buildMarkers(CCellCenteredScalarField vSolidSDFField, CCellCenteredScalarField vFluidSDFField)
		{
			CEulerSolverToolInvoker.buildFluidMarkersInvoker(vSolidSDFField, vFluidSDFField, m_MarkersField);
		}

		private void __buildMarkers(CCellCenteredScalarField vSolidSDFField, ComputeBuffer vParticlesPos)
		{
			CEulerParticlesInvokers.buildFluidMarkersInvoker(vSolidSDFField, vParticlesPos, m_MarkersField);
		}

		private void __buildMarkers(CCellCenteredScalarField vSolidSDFField, ComputeBuffer vParticlesPos, SBoundingBox vEmitBox)
        {
			CEulerParticlesInvokers.buildFluidMarkersWithEmitBoxInvoker(vSolidSDFField, vEmitBox, vParticlesPos, m_MarkersField);
		}

		private void __buildSystem(float vDeltaT, CFaceCenteredVectorField vInputVelField, CFaceCenteredVectorField vSolidVelField)
        {
            __buildMatrix(vDeltaT);
            __buildVector(vInputVelField, vSolidVelField);
            float[] VectorInitialValue = new float[vInputVelField.getResolution().x * vInputVelField.getResolution().y * vInputVelField.getResolution().z];
            m_FdmLinearSystem.FdmVectorx.SetData(VectorInitialValue);
        }

        private void __buildMatrix(float vDeltaT)
        {
			Vector3 Scale = new Vector3(vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.x * m_FdmLinearSystem.Spacing.x), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.y * m_FdmLinearSystem.Spacing.y), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.z * m_FdmLinearSystem.Spacing.z));
			CEulerSolverToolInvoker.buildPressureFdmMatrixAInvoker(m_FdmLinearSystem.Resolution, Scale, m_MarkersField, m_FdmLinearSystem.FdmMatrixA);
		}

		private void __buildVector(CFaceCenteredVectorField vInputVelField, CFaceCenteredVectorField vSolidVelField)
        {
            vInputVelField.divergence(m_VelDivergenceField);

			CEulerSolverToolInvoker.buildPressureVectorbInvoker(vInputVelField, m_VelDivergenceField, m_MarkersField, vSolidVelField, m_FdmLinearSystem.FdmVectorb);
        }

        public void PressureAxCGProd(ComputeBuffer vX, ComputeBuffer voResult, SFdmLinearSystem vFdmLinearSystem)
		{
			Vector3Int Resolution = vFdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.fdmMatrixVectorMulInvoker(Resolution, vFdmLinearSystem.FdmMatrixA, vX, voResult);
		}

		private void __updatePressure()
        {
			CEulerSolverToolInvoker.updatePressureInvoker(m_MarkersField, m_FdmLinearSystem.FdmVectorx, m_PressureField);
		}

		private void __updatePressureGradient()
		{
			m_PressureFieldTemp.resize(m_MarkersField.getResolution(), m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
			m_PressureFieldTemp.gradient(m_TempTempPressureGradientField);
			CEulerSolverToolInvoker.updatePressureGradientInvoker(m_MarkersField, m_TempTempPressureGradientField, m_PressureGradientField);
			//m_PressureGradientField.plusAlphaX(m_TempTempPressureGradientField, 1.0f);
		}

		private void __correctPressureBoundaryCondition(RedBlack vRBMarker)
		{
			Vector3Int Resolution = m_FdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.correctPressureBoundaryCondition(Resolution, m_MarkersField, m_PressureField, vRBMarker);
		}

        private void __correctVelBoundaryCondition(CFaceCenteredVectorField vioFluidVelField, CFaceCenteredVectorField vSolidVelField)
        {
            Vector3Int Resolution = m_FdmLinearSystem.Resolution;

            CEulerSolverToolInvoker.correctVelBoundaryCondition(Resolution, m_MarkersField, vioFluidVelField, vSolidVelField);
        }

		private void __addFluid(CCellCenteredScalarField vioFluidSDFField)
		{
			Vector3Int Resolution = m_FdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.addFluidInvoker(Resolution, vioFluidSDFField);
		}

		private void __removeFluid(CCellCenteredScalarField vioFluidSDFField)
		{
			Vector3Int Resolution = m_FdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.removeFluidInvoker(Resolution, vioFluidSDFField);
		}
		

		private void __setFluidVel(CFaceCenteredVectorField vioFluidVelField, float vInletFluidVel)
        {
			Vector3Int Resolution = m_FdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.setFluidVelInvoker(Resolution, vioFluidVelField, vInletFluidVel);
		}

		private void __applyPotentialGradient(float vDeltaT, CFaceCenteredVectorField vioFluidVelField, CFaceCenteredVectorField vSolidVelField)
		{
			Vector3 Scale = new Vector3(vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.x), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.y), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.z));

			Vector3Int Resolution = m_FdmLinearSystem.Resolution;

			CEulerSolverToolInvoker.applyPotentialGradientInvoker(Resolution, Scale, m_MarkersField, m_FdmLinearSystem.FdmVectorx, vioFluidVelField, vSolidVelField);
		}

		private void __applyPotentialGradient_CN(float vDeltaT, CFaceCenteredVectorField vioFluidVelField, CFaceCenteredVectorField vSolidVelField)
		{
			Vector3 Scale = new Vector3(vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.x), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.y), vDeltaT / (m_Density * m_FdmLinearSystem.Spacing.z));

			Vector3Int Resolution = m_FdmLinearSystem.Resolution;
			if(!m_FirstFrameFlag)
            {
				m_PressureFieldTemp.resize(Resolution, m_MarkersField.getOrigin(), m_MarkersField.getSpacing(), m_FdmLinearSystem.FdmVectorx);
				m_PressureFieldTemp.scale(0.5f);
				m_PressureFieldTemp.plusAlphaX(m_PressureFieldPrevious, 0.5f);
				CEulerSolverToolInvoker.applyPotentialGradientInvoker(Resolution, Scale, m_MarkersField, m_PressureFieldTemp.getGridData(), vioFluidVelField, vSolidVelField);
			}
            else
            {
				CEulerSolverToolInvoker.applyPotentialGradientInvoker(Resolution, Scale, m_MarkersField, m_FdmLinearSystem.FdmVectorx, vioFluidVelField, vSolidVelField);
			}
		}

		//遗弃
		public void PressureMinvxCGProd(ComputeBuffer vX, ComputeBuffer voResult, SFdmLinearSystem vFdmLinearSystem)
		{
			CMathTool.copy(vX, voResult);
		}

		#endregion

		#region SecondOrderPressure
		public void applyPressureGradient
		(
			float vDeltaT,
			CCellCenteredVectorField vMidPointPosFieldX,
			CCellCenteredVectorField vMidPointPosFieldY, 
			CCellCenteredVectorField vMidPointPosFieldZ,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField,
			CFaceCenteredVectorField vSolidVelField, 
			CFaceCenteredVectorField vioFluidVelField,
			ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
		)
		{
			Vector3 Scale = new Vector3(vDeltaT / m_Density, vDeltaT / m_Density, vDeltaT / m_Density);

			Vector3Int Resolution = vioFluidVelField.getResolution();

			m_PressureField.gradient(m_TempPressureGradientField);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldX, m_PressureGradientFieldX, vSamplingAlgorithm);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldY, m_PressureGradientFieldY, vSamplingAlgorithm);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldZ, m_PressureGradientFieldZ, vSamplingAlgorithm);

			CEulerSolverToolInvoker.buildFluidMarkersInvoker(vSolidSDFField, vFluidSDFField, m_MarkersField);

			CEulerSolverToolInvoker.applyPressureGradientInvoker(Resolution, Scale, m_MarkersField, m_PressureGradientFieldX, m_PressureGradientFieldY, m_PressureGradientFieldZ, vioFluidVelField, vSolidVelField);
		}

		public void applyPressureGradient
		(
			float vDeltaT,
			CCellCenteredVectorField vMidPointPosFieldX,
			CCellCenteredVectorField vMidPointPosFieldY,
			CCellCenteredVectorField vMidPointPosFieldZ,
			CCellCenteredScalarField vSolidSDFField,
			ComputeBuffer vParticlesPos,
			CFaceCenteredVectorField vSolidVelField,
			CFaceCenteredVectorField vioFluidVelField,
			ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
		)
		{
			Vector3 Scale = new Vector3(vDeltaT / m_Density, vDeltaT / m_Density, vDeltaT / m_Density);

			Vector3Int Resolution = vioFluidVelField.getResolution();

			m_PressureField.gradient(m_TempPressureGradientField);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldX, m_PressureGradientFieldX, vSamplingAlgorithm);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldY, m_PressureGradientFieldY, vSamplingAlgorithm);
			m_TempPressureGradientField.sampleField(vMidPointPosFieldZ, m_PressureGradientFieldZ, vSamplingAlgorithm);

			CEulerParticlesInvokers.buildFluidMarkersInvoker(vSolidSDFField, vParticlesPos, m_MarkersField);

			CEulerSolverToolInvoker.applyPressureGradientInvoker(Resolution, Scale, m_MarkersField, m_PressureGradientFieldX, m_PressureGradientFieldY, m_PressureGradientFieldZ, vioFluidVelField, vSolidVelField);
		}

		public void applyPressureGradientTest
		(
			float vDeltaT,
			CCellCenteredVectorField vMidPointPosFieldX,
			CCellCenteredVectorField vMidPointPosFieldY,
			CCellCenteredVectorField vMidPointPosFieldZ,
			CCellCenteredScalarField vSolidSDFField,
			CCellCenteredScalarField vFluidSDFField,
			CFaceCenteredVectorField vSolidVelField,
			CFaceCenteredVectorField vioFluidVelField,
			ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
		)
		{
			Vector3 Scale = new Vector3(vDeltaT / m_Density, vDeltaT / m_Density, vDeltaT / m_Density);

			Vector3Int Resolution = vioFluidVelField.getResolution();

			m_PressureGradientField.sampleField(vMidPointPosFieldX, m_PressureGradientFieldX, vSamplingAlgorithm);
			m_PressureGradientField.sampleField(vMidPointPosFieldY, m_PressureGradientFieldY, vSamplingAlgorithm);
			m_PressureGradientField.sampleField(vMidPointPosFieldZ, m_PressureGradientFieldZ, vSamplingAlgorithm);

			CEulerSolverToolInvoker.buildFluidMarkersInvoker(vSolidSDFField, vFluidSDFField, m_MarkersField);

			CEulerSolverToolInvoker.applyPressureGradientInvoker(Resolution, Scale, m_MarkersField, m_PressureGradientFieldX, m_PressureGradientFieldY, m_PressureGradientFieldZ, vioFluidVelField, vSolidVelField);
		}

		#endregion

		private float m_Density = 1.0f;
		private float m_KineticEnergy = 0.0f;
		private SFdmLinearSystem m_FdmLinearSystem;
		CMatrixFreePCG m_MartixFreePCGSolver = null;
		private bool m_FirstFrameFlag = true;

		private CCellCenteredScalarField m_MarkersField;
		private CCellCenteredScalarField m_VelDivergenceField;

		private CCellCenteredScalarField m_PressureField;
		private CCellCenteredScalarField m_PressureFieldTemp;
		private CCellCenteredScalarField m_PressureFieldPrevious;
		private CCellCenteredVectorField m_TempPressureGradientField;
		private CCellCenteredVectorField m_TempTempPressureGradientField;
		private CCellCenteredVectorField m_PressureGradientField;
		private CCellCenteredVectorField m_PressureGradientFieldX;
		private CCellCenteredVectorField m_PressureGradientFieldY;
		private CCellCenteredVectorField m_PressureGradientFieldZ;
		private CCellCenteredVectorField m_TempCCVelField;
		private CCellCenteredScalarField m_KineticEnergyField;
	}
}
