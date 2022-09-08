using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CCellCenteredVectorField
    {
		#region Constructor&Resize
		public CCellCenteredVectorField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			resize(vResolution, vOrigin, vSpacing);
		}
		public CCellCenteredVectorField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float[] vInitialValueX, float[] vInitialValueY, float[] vInitialValueZ)
		{
			resize(vResolution, vOrigin, vSpacing, vInitialValueX, vInitialValueY, vInitialValueZ);
		}
		public CCellCenteredVectorField(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer vInitialComputeBufferX, ComputeBuffer vInitialComputeBufferY, ComputeBuffer vInitialComputeBufferZ)
		{
			resize(vResolution, vOrigin, vSpacing, vInitialComputeBufferX, vInitialComputeBufferY, vInitialComputeBufferZ);
		}
		public CCellCenteredVectorField(CCellCenteredVectorField vOther)
		{
			resize(vOther);
		}
		~CCellCenteredVectorField()
		{
			m_GridDataX.Release();
			m_GridDataY.Release();
			m_GridDataZ.Release();
		}
		public void free()
        {
			m_GridDataX.Release();
			m_GridDataY.Release();
			m_GridDataZ.Release();
		}
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeDataX();
			__resizeDataY();
			__resizeDataZ();
		}
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float[] vInitialValueX, float[] vInitialValueY, float[] vInitialValueZ)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeDataX(vInitialValueX);
			__resizeDataY(vInitialValueY);
			__resizeDataZ(vInitialValueZ);
		}
		public void resize(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer vInitialComputeBufferX, ComputeBuffer vInitialComputeBufferY, ComputeBuffer vInitialComputeBufferZ)
		{
			setGridParameters(vResolution, vOrigin, vSpacing);
			__resizeDataX(vInitialComputeBufferX);
			__resizeDataY(vInitialComputeBufferY);
			__resizeDataZ(vInitialComputeBufferZ);
		}
		public void resize(CCellCenteredVectorField vOther)
		{
			setGridParameters(vOther.getResolution(), vOther.getOrigin(), vOther.getSpacing());
			__resizeDataX(vOther.getGridDataX());
			__resizeDataY(vOther.getGridDataY());
			__resizeDataZ(vOther.getGridDataZ());
		}
		private void __resizeDataX()
		{
			float[] InitialValueX = new float[m_GridResolution.x * m_GridResolution.y * m_GridResolution.z];

			if (m_GridDataX == null)
			{
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataX.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataX.Release();
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataX.SetData(InitialValueX);
		}
		private void __resizeDataY()
		{
			float[] InitialValueY = new float[m_GridResolution.x * m_GridResolution.y * m_GridResolution.z];

			if (m_GridDataY == null)
			{
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataY.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataY.Release();
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataY.SetData(InitialValueY);
		}
		private void __resizeDataZ()
		{
			float[] InitialValueZ = new float[m_GridResolution.x * m_GridResolution.y * m_GridResolution.z];

			if (m_GridDataZ == null)
			{
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataZ.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataZ.Release();
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataZ.SetData(InitialValueZ);
		}
		private void __resizeDataX(float[] vInitialValueX)
		{
			if (vInitialValueX.Length != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的数组与场的维度不同！");
				return;
			}
			else if (m_GridDataX == null)
			{
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataX.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataX.Release();
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataX.SetData(vInitialValueX);
		}
		private void __resizeDataY(float[] vInitialValueY)
		{
			if (vInitialValueY.Length != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的数组与场的维度不同！");
				return;
			}
			else if (m_GridDataY == null)
			{
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataY.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataY.Release();
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataY.SetData(vInitialValueY);
		}
		private void __resizeDataZ(float[] vInitialValueZ)
		{
			if (vInitialValueZ.Length != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的数组与场的维度不同！");
				return;
			}
			else if (m_GridDataZ == null)
			{
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataZ.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataZ.Release();
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			m_GridDataZ.SetData(vInitialValueZ);
		}
		private void __resizeDataX(ComputeBuffer vOtherComputeBufferX)
		{
			if (vOtherComputeBufferX.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的Buffer与场的维度不同！");
				return;
			}
			else if (m_GridDataX == null)
			{
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataX.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataX.Release();
				m_GridDataX = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			CMathTool.copy(vOtherComputeBufferX, m_GridDataX);
		}
		private void __resizeDataY(ComputeBuffer vOtherComputeBufferY)
		{
			if (vOtherComputeBufferY.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的Buffer与场的维度不同！");
				return;
			}
			else if (m_GridDataY == null)
			{
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataY.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataY.Release();
				m_GridDataY = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			CMathTool.copy(vOtherComputeBufferY, m_GridDataY);
		}
		private void __resizeDataZ(ComputeBuffer vOtherComputeBufferZ)
		{
			if (vOtherComputeBufferZ.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				Debug.LogError("初始化CCV场的Buffer与场的维度不同！");
				return;
			}
			else if (m_GridDataZ == null)
			{
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else if (m_GridDataZ.count != m_GridResolution.x * m_GridResolution.y * m_GridResolution.z)
			{
				m_GridDataZ.Release();
				m_GridDataZ = new ComputeBuffer(m_GridResolution.x * m_GridResolution.y * m_GridResolution.z, sizeof(float));
			}
			else
			{

			}
			CMathTool.copy(vOtherComputeBufferZ, m_GridDataZ);
		}
		#endregion

		#region Get&Set
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
		public ComputeBuffer getGridDataX()
		{
			return m_GridDataX;
		}
		public ComputeBuffer getGridDataY()
		{
			return m_GridDataY;
		}
		public ComputeBuffer getGridDataZ()
		{
			return m_GridDataZ;
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
		public void sampleField(CCellCenteredVectorField vSampledAbsPosVectorField, CCellCenteredVectorField voDstVectorField, ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR)
		{
			CFieldMathToolInvoker.sampleCCVFieldWithPosFieldInvoker(this, voDstVectorField, vSampledAbsPosVectorField, vSamplingAlgorithm);
		}
        public void sampleField(ComputeBuffer vSampledAbsPos, ComputeBuffer voDstData, ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR)
        {
			CFieldMathToolInvoker.sampleCCVFieldWithPosBufferInvoker(this, voDstData, vSampledAbsPos, vSamplingAlgorithm);
		}
        public void divergence(CCellCenteredScalarField voDivergenceField)
        {
			CFieldMathToolInvoker.divergenceInvoker(this, voDivergenceField);
		}
		public void curl(CCellCenteredVectorField voCurlField)
        {
			CFieldMathToolInvoker.curlInvoker(this, voCurlField);
        }
		#endregion

		#region MathOperator
		public void transfer2FCVField(CFaceCenteredVectorField voFCVField)
		{
			voFCVField.resize(getResolution(), getOrigin(), getSpacing());
			CFieldMathToolInvoker.transferCCVField2FCVFieldInvoker(this, voFCVField);
		}

		public void length(CCellCenteredScalarField voLengthField)
        {
			CFieldMathToolInvoker.lengthInvoker(this, voLengthField);
        }

		public void scale(float vScalarValue)
        {
			CMathTool.scale(m_GridDataX, vScalarValue);
			CMathTool.scale(m_GridDataY, vScalarValue);
			CMathTool.scale(m_GridDataZ, vScalarValue);
		}

		public void plus(float vScalarValue)
        {
			CMathTool.plus(m_GridDataX, vScalarValue);
			CMathTool.plus(m_GridDataY, vScalarValue);
			CMathTool.plus(m_GridDataZ, vScalarValue);
		}

		public void plus(Vector3 vVectorValue)
		{
			CMathTool.plus(m_GridDataX, vVectorValue.x);
			CMathTool.plus(m_GridDataY, vVectorValue.y);
			CMathTool.plus(m_GridDataZ, vVectorValue.z);
		}

		public void plusAlphaX(CCellCenteredVectorField vVectorField, float vScalarValue)
        {
			CGlobalMacroAndFunc._ASSERTE(getResolution() == vVectorField.getResolution());

			if (Mathf.Abs(vScalarValue) < Mathf.Epsilon) return;

			CMathTool.plusAlphaX(m_GridDataX, vVectorField.getGridDataX(), vScalarValue);
			CMathTool.plusAlphaX(m_GridDataY, vVectorField.getGridDataY(), vScalarValue);
			CMathTool.plusAlphaX(m_GridDataZ, vVectorField.getGridDataZ(), vScalarValue);
		}
		#endregion

		private Vector3Int m_GridResolution;
		private Vector3 m_GridOrigin;
		private Vector3 m_GridSpacing;

		private ComputeBuffer m_GridDataX;
		private ComputeBuffer m_GridDataY;
		private ComputeBuffer m_GridDataZ;
	}
}