using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;

namespace EulerFluidEngine
{
	public class CBoundarys
    {
		#region Constructor&Resize
		public CBoundarys() { }
		public CBoundarys(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			initBoundarys(vResolution, vOrigin, vSpacing);
		}

		~CBoundarys()
        {
			m_DynamicBoundarysTrans.Clear();
			m_DynamicBoundarysRot.Clear();
			m_DynamicBoundarysScale.Clear();

			m_DynamicBoundarysTransforms.Clear();
			m_DynamicOriginalSDFData.Clear();
			m_DynamicBoundarysCurrentSDF.Clear();

			if (m_Initialized)
            {
				m_DynamicBoundarysVel.Release();
				m_StaticBoundarysSDF.free();
				m_AdditionalStaticBoundarySDF.free();

				m_TotalBoundarySDF.free();
				m_TotalBoundaryVel.free();
			}
		}

		public void free()
        {
			m_DynamicBoundarysTrans.Clear();
			m_DynamicBoundarysRot.Clear();
			m_DynamicBoundarysScale.Clear();

			m_DynamicBoundarysTransforms.Clear();
			m_DynamicOriginalSDFData.Clear();
			m_DynamicBoundarysCurrentSDF.Clear();

			if (m_Initialized)
			{
				m_DynamicBoundarysVel.Release();
				m_StaticBoundarysSDF.free();
				m_AdditionalStaticBoundarySDF.free();

				m_TotalBoundarySDF.free();
				m_TotalBoundaryVel.free();
			}
		}

		public void initBoundarys(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
		{
			m_BoundarysResolution = vResolution;
			m_BoundarysOrigin = vOrigin;
			m_BoundarysSpacing = vSpacing;

			m_DynamicBoundarysNum = 0;

			if(m_Initialized)
            {
				m_DynamicBoundarysTrans.Clear();
				m_DynamicBoundarysRot.Clear();
				m_DynamicBoundarysScale.Clear();
				m_DynamicBoundarysVel.Release();
				m_DynamicBoundarysVel = new ComputeBuffer(3 * 20, sizeof(float));
	
				m_DynamicBoundarysTransforms.Clear();
				m_DynamicOriginalSDFData.Clear();
				m_DynamicBoundarysCurrentSDF.Clear();

				m_StaticBoundarysSDF.resize(vResolution, vOrigin, vSpacing);
				m_AdditionalStaticBoundarySDF.resize(vResolution, vOrigin, vSpacing);

				m_TotalBoundarySDF.resize(vResolution, vOrigin, vSpacing);
				m_TotalBoundaryVel.resize(vResolution, vOrigin, vSpacing);
			}
            else
            {
				m_DynamicBoundarysTrans = new List<Vector3>();
				m_DynamicBoundarysRot = new List<Vector3>();
				m_DynamicBoundarysScale = new List<Vector3>();
				m_DynamicBoundarysVel = new ComputeBuffer(3 * 20, sizeof(float));

				m_DynamicBoundarysTransforms = new List<Transform>();
				m_DynamicOriginalSDFData = new List<SDFData>();
				m_DynamicBoundarysCurrentSDF = new List<CCellCenteredScalarField>();

				m_StaticBoundarysSDF = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
				m_AdditionalStaticBoundarySDF = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);

				m_TotalBoundarySDF = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
				m_TotalBoundaryVel = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);

				m_Initialized = true;
			}
		}
		#endregion

		#region TestFunc
		void setTotalBoundarysVel(CFaceCenteredVectorField vBoundarysVelField)
        {
			m_TotalBoundaryVel = vBoundarysVelField;
		}
		#endregion

		#region Get&Set
		public int getNumOfDynamicBoundarys()
        {
			return m_DynamicBoundarysNum;
        }
		public ComputeBuffer getDynamicBoundarysVel()
        {
			return m_DynamicBoundarysVel;
        }
		public List<Vector3> getDynamicBoundarysTranslation()
        {
			return m_DynamicBoundarysTrans;
        }
		public List<Vector3> getDynamicBoundarysRotation()
        {
			return m_DynamicBoundarysRot;
        }
		public List<Vector3> getDynamicBoundarysScale()
        {
			return m_DynamicBoundarysScale;
        }
		public SDFData getDynamicOriginalSDFData(int vIndex = 0)
        {
			return m_DynamicOriginalSDFData[vIndex];
		}
		public CCellCenteredScalarField getDynamicBoundaryCurrentSDF(int vIndex = 0)
        {
			return m_DynamicBoundarysCurrentSDF[vIndex];
		}
		public CCellCenteredScalarField getTotalBoundarysSDF()
        {
			return m_TotalBoundarySDF;
		}
		public CFaceCenteredVectorField getTotalBoundarysVel()
        {
			return m_TotalBoundaryVel;
		}
		#endregion

		#region CoreMethods
		public void addDynamicBoundary
		(
			SDFData vNewBoundarySDFData,
			Vector3 vVelocity,
			Vector3 vTranslation,
			Vector3 vRotation,
			Vector3 vScale
		)
		{
			if (vNewBoundarySDFData == null)
            {
				Debug.LogError("SDF数据为空！");
				return;
            }

			if (m_DynamicBoundarysNum >= m_MaxDynamicBoundarysNum)
			{
				Debug.LogError("动态边界数量超出限制！");
				return;
			}

			float[] TempBounadrysVel = new float[3];

			TempBounadrysVel[0] = vVelocity.x;
			TempBounadrysVel[1] = vVelocity.y;
			TempBounadrysVel[2] = vVelocity.z;

			m_DynamicBoundarysVel.SetData(TempBounadrysVel, 0, 3 * m_DynamicBoundarysNum, 3);
			m_DynamicBoundarysTrans.Add(vTranslation);
			m_DynamicBoundarysRot.Add(vRotation);
			m_DynamicBoundarysScale.Add(vScale);

			CCellCenteredScalarField TempBoundarySDFField = new CCellCenteredScalarField(m_BoundarysResolution, m_BoundarysOrigin, m_BoundarysSpacing);

			CBoundaryToolInvoker.sampleTexture3DWithTransformInvoker
			(
				vNewBoundarySDFData.sdfTexture, 
				vNewBoundarySDFData.bounds.min, 
				vNewBoundarySDFData.bounds.max, 
				m_DynamicBoundarysTrans[m_DynamicBoundarysNum],
				m_DynamicBoundarysRot[m_DynamicBoundarysNum],
				m_DynamicBoundarysScale[m_DynamicBoundarysNum],
				TempBoundarySDFField
			);

			m_DynamicOriginalSDFData.Add(vNewBoundarySDFData);
			m_DynamicBoundarysCurrentSDF.Add(TempBoundarySDFField);

			m_DynamicBoundarysNum++;
		}

		public void addStaticBoundary
		(
			SDFData vNewBoundarySDFData,
			Vector3 vTranslation,
			Vector3 vRotation,
			Vector3 vScale
		)
        {
			if (vNewBoundarySDFData == null)
			{
				Debug.LogError("SDF数据为空！");
				return;
			}

			CBoundaryToolInvoker.sampleTexture3DWithTransformInvoker
			(
				vNewBoundarySDFData.sdfTexture,
				vNewBoundarySDFData.bounds.min,
				vNewBoundarySDFData.bounds.max,
				vTranslation,
				vRotation,
				vScale,
				m_AdditionalStaticBoundarySDF
			);

			m_StaticBoundarysSDF = CFieldMathToolInvoker.unionSDFInvoker(m_StaticBoundarysSDF, m_AdditionalStaticBoundarySDF);
		}

		public void addBoundarys(List<GameObject> vBoundaryObjects, Vector3[] vOriginalVelocities, bool vIsDynamic = true)
        {
			for (int i = 0; i < vBoundaryObjects.Count; i++)
			{
				if (vIsDynamic && m_DynamicBoundarysNum >= m_MaxDynamicBoundarysNum)
				{
					Debug.LogError("动态边界数量超出限制！");
					return;
				}

				SDFBaker Baker = vBoundaryObjects[i].GetComponent<SDFBaker>();
				if (Baker == null)
				{
					Debug.LogError("物体" + vBoundaryObjects[i].name + "没有SDFBaker组件！");
					return;
				}
				SDFData Data = Baker.sdfData;
				if (Data == null)
				{
					Debug.LogError("物体" + vBoundaryObjects[i].name + "没有SDF数据！");
					return;
				}
				Data.sdfTexture.wrapMode = TextureWrapMode.Clamp;
				m_DynamicBoundarysTransforms.Add(vBoundaryObjects[i].transform);
				if (vIsDynamic)
				{
					if ((vOriginalVelocities != null && i < vOriginalVelocities.Length))
						addDynamicBoundary(Data, vOriginalVelocities[i], vBoundaryObjects[i].transform.position, Vector3.zero, new Vector3(1, 1, 1));
					else
						addDynamicBoundary(Data, Vector3.zero, vBoundaryObjects[i].transform.position, Vector3.zero, new Vector3(1, 1, 1));
				}
				else
				{
					addStaticBoundary(Data, vBoundaryObjects[i].transform.position, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
				}
			}

			__updateTotalBoundarySDF();
		}

		public void updateBoundarys(float vDeltaT)
		{
			if (__moveDynamicBoundarys(vDeltaT)) 
			{ 
				__resamplingDynamicBoundarysSDF();
				__updateTotalBoundarySDF();
			}
			__updateTotalBoundaryVel();
		}

		private bool __moveDynamicBoundarys(float vDeltaT)
		{
			bool BoundaryMoved = false;
			Vector3 CurTranslation;
			float[] TempVelocity = new float[m_DynamicBoundarysVel.count];

			for (int i = 0; i < m_DynamicBoundarysNum; i++)
            {
				CurTranslation = m_DynamicBoundarysTransforms[i].position;

				if (CurTranslation != m_DynamicBoundarysTrans[i]) 
				{
					BoundaryMoved = true;

					TempVelocity[3 * i] = (CurTranslation.x - m_DynamicBoundarysTrans[i].x) / vDeltaT;
					TempVelocity[3 * i + 1] = (CurTranslation.y - m_DynamicBoundarysTrans[i].y) / vDeltaT;
					TempVelocity[3 * i + 2] = (CurTranslation.z - m_DynamicBoundarysTrans[i].z) / vDeltaT;

					m_DynamicBoundarysTrans[i] = CurTranslation;
				}
			}

			if (BoundaryMoved)
				m_DynamicBoundarysVel.SetData(TempVelocity);

			return BoundaryMoved;
		}

		private void __resamplingDynamicBoundarysSDF()
		{
			for (int i = 0; i < m_DynamicBoundarysNum; i++)
			{
				SDFData Data = m_DynamicOriginalSDFData[i];
				CBoundaryToolInvoker.sampleTexture3DWithTransformInvoker
				(
					Data.sdfTexture,
					Data.bounds.min,
					Data.bounds.max,
					m_DynamicBoundarysTrans[i],
					m_DynamicBoundarysRot[i],
					m_DynamicBoundarysScale[i],
					m_DynamicBoundarysCurrentSDF[i]
				);
			}
		}

		private void __updateTotalBoundarySDF()
		{
			m_DynamicBoundarysCurrentSDF.Add(m_StaticBoundarysSDF);
			CBoundaryToolInvoker.buildTotalBoundarysSDFInvoker(m_DynamicBoundarysCurrentSDF, m_TotalBoundarySDF);
			m_DynamicBoundarysCurrentSDF.RemoveAt(m_DynamicBoundarysCurrentSDF.Count - 1);
		}
		
		private void __updateTotalBoundaryVel()
		{
			CBoundaryToolInvoker.buildSolidsVelFieldInvoker(m_DynamicBoundarysVel, m_TotalBoundarySDF, m_TotalBoundaryVel);
		}
		#endregion

		private Vector3Int m_BoundarysResolution;
		private Vector3 m_BoundarysOrigin;
		private Vector3 m_BoundarysSpacing;

		private bool m_Initialized = false;

		private int m_DynamicBoundarysNum = 0;
		private int m_MaxDynamicBoundarysNum = 20;

		private List<Vector3> m_DynamicBoundarysTrans;
		private List<Vector3> m_DynamicBoundarysRot;
		private List<Vector3> m_DynamicBoundarysScale;
		private ComputeBuffer m_DynamicBoundarysVel;

		private List<Transform> m_DynamicBoundarysTransforms;
		private List<SDFData> m_DynamicOriginalSDFData;

		private List<CCellCenteredScalarField> m_DynamicBoundarysCurrentSDF;
		private CCellCenteredScalarField m_StaticBoundarysSDF;
		private CCellCenteredScalarField m_AdditionalStaticBoundarySDF;

		private CCellCenteredScalarField m_TotalBoundarySDF;
		private CFaceCenteredVectorField m_TotalBoundaryVel;
	}
}