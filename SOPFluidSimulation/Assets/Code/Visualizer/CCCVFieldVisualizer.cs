using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

public class CCCVFieldVisualizer : MonoBehaviour
{
    public CCCVFieldVisualizer(CCellCenteredVectorField vVisualizedCCVField)
    {
        init(vVisualizedCCVField);
    }

    public void init(CCellCenteredVectorField vVisualizedCCVField)
    {
        m_CCVField = vVisualizedCCVField;

        m_Resolution = m_CCVField.getResolution();
        m_Origin = m_CCVField.getOrigin();
        m_Spacing = m_CCVField.getSpacing();

        if(m_LengthField != null)
        {
            m_LengthField.resize(m_Resolution, m_Origin, m_Spacing);
        }
        else
        {
            m_LengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
        }

        m_CCVFieldVisulaizerMaterial = Resources.Load("Materials/FieldVisualizer") as Material;

        m_CCVFieldVisulaizerMaterial.SetVector("Resolution_CCVField", new Vector4(m_Resolution.x, m_Resolution.y, m_Resolution.z, 0));
        m_CCVFieldVisulaizerMaterial.SetVector("Origin_CCVField", new Vector4(m_Origin.x, m_Origin.y, m_Origin.z, 0));
        m_CCVFieldVisulaizerMaterial.SetVector("Spacing_CCVField", new Vector4(m_Spacing.x, m_Spacing.y, m_Spacing.z, 0));

        m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataX_CCVField", m_CCVField.getGridDataX());
        m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataY_CCVField", m_CCVField.getGridDataY());
        m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataZ_CCVField", m_CCVField.getGridDataZ());
    }

    public void setVisualizerFlag(bool vFlag)
    {
        m_IsVisualized = vFlag;
    }

    public void visualize()
    {
        if (m_IsVisualized)
        {
            m_CCVField.length(m_LengthField);
            float MaxLengthValue = CMathTool.getAbsMaxValue(m_LengthField.getGridData());
            m_CCVFieldVisulaizerMaterial.SetFloat("MaxLengthValue_CCVField", MaxLengthValue);

            //TODO:是否需要每帧更新
            m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataX_CCVField", m_CCVField.getGridDataX());
            m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataY_CCVField", m_CCVField.getGridDataY());
            m_CCVFieldVisulaizerMaterial.SetBuffer("vVectorFieldDataZ_CCVField", m_CCVField.getGridDataZ());

            m_CCVFieldVisulaizerMaterial.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Lines, m_Resolution.x * m_Resolution.y * m_Resolution.z * 6);
        }
    }

    private bool m_IsVisualized = true;

    private CCellCenteredVectorField m_CCVField;
    private CCellCenteredScalarField m_LengthField;
    private Vector3Int m_Resolution;
    private Vector3 m_Origin;
    private Vector3 m_Spacing;

    private Material m_CCVFieldVisulaizerMaterial;
}