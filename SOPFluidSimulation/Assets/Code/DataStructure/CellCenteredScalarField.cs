using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
	public class CCellCenteredScalarField
	{
		#region Constructor&Resize
		public CCellCenteredScalarField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			resize(vResolution, vOrigin, vSpacing);
		}
		public CCellCenteredScalarField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float[] vInitialValue)
		{
			resize(vResolution, vOrigin, vSpacing, vInitialValue);
		}
		public CCellCenteredScalarField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer vInitialComputeBuffer)
		{
			resize(vResolution, vOrigin, vSpacing, vInitialComputeBuffer);
		}
		public CCellCenteredScalarField(CCellCenteredScalarField vOther)
		{
			resize(vOther);
		}
		~CCellCenteredScalarField()
		{
			m_GridData.Release();
		}
		public void free()
        {
			m_GridData.Release();
        }
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeData();
		}
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float[] vInitialValue)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeData(vInitialValue);
		}
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer vInitialComputeBuffer)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeData(vInitialComputeBuffer);
		}
		public void resize(CCellCenteredScalarField vOther)
		{
			setGridParameters(vOther.getResolution(), vOther.getOrigin(), vOther.getSpacing());
			__resizeData(vOther.getGridData());
		}
		private void __resizeData()
		{
			float[] InitialValue = new float[m_GridResolution.x * m_GridResolution.y * m_GridResolution.z];

			if (m_GridData == null)
			{
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridData.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridData.Release();
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridData.SetData(InitialValue);
		}
		private void __resizeData(float[] vInitialValue)
		{
			if (vInitialValue.Length != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCS场的数组与场的维度不同！");
				return;
			}
			else if (m_GridData == null)
			{
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridData.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridData.Release();
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridData.SetData(vInitialValue);
		}
		private void __resizeData(ComputeBuffer vOtherComputeBuffer)
		{
			if (vOtherComputeBuffer.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCS场的Buffer与场的维度不同！");
				return;
			}
			else if (m_GridData == null)
			{
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridData.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridData.Release();
				m_GridData = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			CMathTool.copy(vOtherComputeBuffer, m_GridData);
		}
		#endregion

		#region Set&Get
		public Vector3Int getResolution()
		{
			return m_GridResolution;
		}
		public Vector3 getOrigin()
		{
			return m_GridOrigin;
		}
		public Vector3 getSpacing()
		{
			return m_GridSpacing;
		}
		public ComputeBuffer getGridData()
		{
			return m_GridData;
		}
		public void setGridParameters(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			m_GridResolution = vResolution;
			m_GridOrigin = vOrigin;
			m_GridSpacing = vSpacing;
		}
		public void setResolution(Vector3Int vResolution)
		{
			m_GridResolution = vResolution;
		}
		public void setOrigin(Vector3 vOrigin)
		{
			m_GridOrigin = vOrigin;
		}
		public void setSpacing(Vector3 vSpacing)
		{
			m_GridSpacing = vSpacing;
		}
		#endregion

		#region CoreMethods
		public void sampleField(CCellCenteredVectorField vSampledAbsPosVectorField, CCellCenteredScalarField voDstScalarField, ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR)
        {
			CFieldMathToolInvoker.sampleCCSFieldWithPosFieldInvoker(this, voDstScalarField, vSampledAbsPosVectorField, vSamplingAlgorithm);
        }
        public void sampleField(ComputeBuffer vSampledAbsPos, ComputeBuffer voDstData, ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR)
        {
			CFieldMathToolInvoker.sampleCCSFieldWithPosBufferInvoker(this, voDstData, vSampledAbsPos, vSamplingAlgorithm);
		}
        public void gradient(CCellCenteredVectorField voGradientField)
        {
			CFieldMathToolInvoker.gradientInvoker(this, voGradientField);
		}
		public void laplacian(CCellCenteredScalarField voLaplacianField)
        {
			CFieldMathToolInvoker.laplacianInvoker(this, voLaplacianField);
		}
		#endregion

		#region MathOperator
		public void scale(float vScalarValue)
        {
			CMathTool.scale(m_GridData, vScalarValue);
        }
		public void plus(float vScalarValue)
		{
			CMathTool.plus(m_GridData, vScalarValue);
		}
		public void plusAlphaX(CCellCenteredScalarField vScalarField, float vScalarValue)
        {
			CMathTool.plusAlphaX(m_GridData, vScalarField.getGridData(), vScalarValue);
        }
		#endregion

		private Vector3Int m_GridResolution;
		private Vector3 m_GridOrigin;
		private Vector3 m_GridSpacing; 

		private ComputeBuffer m_GridData;
	}
}
