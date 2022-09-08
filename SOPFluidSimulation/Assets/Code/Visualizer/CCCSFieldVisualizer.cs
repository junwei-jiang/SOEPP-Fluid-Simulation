using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

public class CCCSFieldVisualizer : MonoBehaviour
{
    public CCCSFieldVisualizer(CCellCenteredScalarField vVisualizedCCSField)
    {
        init(vVisualizedCCSField);
    }

    public void init(CCellCenteredScalarField vVisualizedCCSField)
    {
        m_CCSField = vVisualizedCCSField;

        m_Resolution = m_CCSField.getResolution();
        m_Origin = m_CCSField.getOrigin();
        m_Spacing = m_CCSField.getSpacing();

        m_CCSFieldVisulaizerMaterial = Resources.Load("Materials/FieldVisualizer") as Material;

        m_CCSFieldVisulaizerMaterial.SetVector("Resolution_CCSField", new Vector4(m_Resolution.x, m_Resolution.y, m_Resolution.z, 0));
        m_CCSFieldVisulaizerMaterial.SetVector("Origin_CCSField", new Vector4(m_Origin.x, m_Origin.y, m_Origin.z, 0));
        m_CCSFieldVisulaizerMaterial.SetVector("Spacing_CCSField", new Vector4(m_Spacing.x, m_Spacing.y, m_Spacing.z, 0));

        m_CCSFieldVisulaizerMaterial.SetBuffer("vScalarFieldData_CCSField", m_CCSField.getGridData());
    }

    public void setVisualizerFlag(bool vFlag)
    {
        m_IsVisualized = vFlag;
    }

    public void visualize()
    {
        if (m_IsVisualized)
        {
            float MaxValue = CMathTool.getAbsMaxValue(m_CCSField.getGridData());
            m_CCSFieldVisulaizerMaterial.SetFloat("MaxValue_CCSField", MaxValue);

            //TODO:是否需要每帧更新
            m_CCSFieldVisulaizerMaterial.SetBuffer("vScalarFieldData_CCSField", m_CCSField.getGridData());

            m_CCSFieldVisulaizerMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, m_Resolution.x * m_Resolution.y * m_Resolution.z * 36);
        }
    }

    private bool m_IsVisualized = true;

    private CCellCenteredScalarField m_CCSField;
    private Vector3Int m_Resolution;
    private Vector3 m_Origin;
    private Vector3 m_Spacing;

    private Material m_CCSFieldVisulaizerMaterial;
}
