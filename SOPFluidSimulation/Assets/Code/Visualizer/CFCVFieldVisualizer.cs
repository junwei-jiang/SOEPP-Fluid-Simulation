using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

public class CFCVFieldVisualizer : MonoBehaviour
{
    public CFCVFieldVisualizer(CFaceCenteredVectorField vVisualizedFCVField)
    {
        init(vVisualizedFCVField);
    }

    public void init(CFaceCenteredVectorField vVisualizedFCVField)
    {
        m_FCVField = vVisualizedFCVField;

        m_Resolution = m_FCVField.getResolution();
        m_Origin = m_FCVField.getOrigin();
        m_Spacing = m_FCVField.getSpacing();

        if (m_LengthField != null)
        {
            m_LengthField.resize(m_Resolution, m_Origin, m_Spacing);
        }
        else
        {
            m_LengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
        }

        m_FCVFieldVisulaizerMaterial = Resources.Load("Materials/FieldVisualizer") as Material;

        m_FCVFieldVisulaizerMaterial.SetVector("Resolution_FCVField", new Vector4(m_Resolution.x, m_Resolution.y, m_Resolution.z, 0));
        m_FCVFieldVisulaizerMaterial.SetVector("Origin_FCVField", new Vector4(m_Origin.x, m_Origin.y, m_Origin.z, 0));
        m_FCVFieldVisulaizerMaterial.SetVector("Spacing_FCVField", new Vector4(m_Spacing.x, m_Spacing.y, m_Spacing.z, 0));

        m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataX_FCVField", m_FCVField.getGridDataX());
        m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataY_FCVField", m_FCVField.getGridDataY());
        m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataZ_FCVField", m_FCVField.getGridDataZ());
    }

    public void setVisualizerFlag(bool vFlag)
    {
        m_IsVisualized = vFlag;
    }

    //只有Buffer的值发生改变，Buffer还是那个Buffer
    public void visualize()
    {
        if (m_IsVisualized)
        {
            m_FCVField.length(m_LengthField);
            float MaxLengthValue = CMathTool.getAbsMaxValue(m_LengthField.getGridData());
            m_FCVFieldVisulaizerMaterial.SetFloat("MaxLengthValue_FCVField", MaxLengthValue);

            m_FCVFieldVisulaizerMaterial.SetPass(2);
            Graphics.DrawProceduralNow(MeshTopology.Lines, m_Resolution.x * m_Resolution.y * m_Resolution.z * 6);
        }
    }

    //全部重新初始化并可视化
    public void visualize(CFaceCenteredVectorField vVisualizedFCVField)
    {
        if (m_IsVisualized)
        {
            m_FCVField = vVisualizedFCVField;

            m_Resolution = m_FCVField.getResolution();
            m_Origin = m_FCVField.getOrigin();
            m_Spacing = m_FCVField.getSpacing();

            if (m_LengthField != null)
            {
                m_LengthField.resize(m_Resolution, m_Origin, m_Spacing);
            }
            else
            {
                m_LengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            }

            m_FCVFieldVisulaizerMaterial = Resources.Load("Materials/FieldVisualizer") as Material;

            m_FCVFieldVisulaizerMaterial.SetVector("Resolution_FCVField", new Vector4(m_Resolution.x, m_Resolution.y, m_Resolution.z, 0));
            m_FCVFieldVisulaizerMaterial.SetVector("Origin_FCVField", new Vector4(m_Origin.x, m_Origin.y, m_Origin.z, 0));
            m_FCVFieldVisulaizerMaterial.SetVector("Spacing_FCVField", new Vector4(m_Spacing.x, m_Spacing.y, m_Spacing.z, 0));

            //TODO:是否需要每帧更新
            m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataX_FCVField", m_FCVField.getGridDataX());
            m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataY_FCVField", m_FCVField.getGridDataY());
            m_FCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataZ_FCVField", m_FCVField.getGridDataZ());

            m_FCVField.length(m_LengthField);
            float MaxLengthValue = CMathTool.getAbsMaxValue(m_LengthField.getGridData());
            m_FCVFieldVisulaizerMaterial.SetFloat("MaxLengthValue_FCVField", MaxLengthValue);

            m_FCVFieldVisulaizerMaterial.SetPass(2);
            Graphics.DrawProceduralNow(MeshTopology.Lines, m_Resolution.x * m_Resolution.y * m_Resolution.z * 6);
        }

    }

    private bool m_IsVisualized = true;

    private CFaceCenteredVectorField m_FCVField;
    private CCellCenteredScalarField m_LengthField;
    private Vector3Int m_Resolution;
    private Vector3 m_Origin;
    private Vector3 m_Spacing;

    private Material m_FCVFieldVisulaizerMaterial;
}
