using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CVortexBox
    {
        public CVortexBox(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            resizeVortexBox(vResolution, vOrigin, vSpacing);
        }

        public void resizeVortexBox(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            m_Resolution = new Vector3Int(vResolution.x, vResolution.y, 1);
            m_Origin = vOrigin;
            m_Spacing = vSpacing;

            m_R = 0.15f * m_Resolution.x * m_Spacing.x;
            m_CenterX = 0.5f * m_Resolution.x * m_Spacing.x;
            m_CenterY = 0.75f * m_Resolution.x * m_Spacing.x;
            m_Normalize = 0.0f;

            if (m_RhoField == null)
            {
                m_RhoField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            }
            else
            {
                m_RhoField.resize(m_Resolution, m_Origin, m_Spacing);
            }

            if (m_MagField == null)
            {
                m_MagField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            }
            else
            {
                m_MagField.resize(m_Resolution, m_Origin, m_Spacing);
            }

            if (m_InitVelField == null)
            {
                m_InitVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            }
            else
            {
                m_InitVelField.resize(m_Resolution, m_Origin, m_Spacing);
            }
        }

        public CCellCenteredScalarField getRhoField()
        {
            return m_RhoField;
        }

        public CFaceCenteredVectorField getVelField()
        {
            return m_InitVelField;
        }

        public void initVortexBoxVelField()
        {
            __generateRhoAndMagField();

            m_Normalize = CMathTool.getMaxValue(m_MagField.getGridData());

            __generateInitVelField();
        }

        private void __generateRhoAndMagField()
        {
            CVortexBoxInvoer.generateRhoAndMagFieldInvoker(m_R, m_CenterX, m_CenterY, m_RhoField, m_MagField);
        }

        private void __generateInitVelField()
        {
            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 0, 0);
            CVortexBoxInvoer.generateInitVelFieldXInvoker(ArgumentResolution, m_Origin, m_Spacing, m_Normalize, m_InitVelField.getGridDataX());

            ArgumentResolution = m_Resolution + new Vector3Int(0, 1, 0);
            CVortexBoxInvoer.generateInitVelFieldYInvoker(ArgumentResolution, m_Origin, m_Spacing, m_Normalize, m_InitVelField.getGridDataY());
        }

        private Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;

        private float m_R;
        private float m_CenterX;
        private float m_CenterY;
        private float m_Normalize;

        private CCellCenteredScalarField m_RhoField;
        private CCellCenteredScalarField m_MagField;
        private CFaceCenteredVectorField m_InitVelField;
    }
}