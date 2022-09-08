using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CZalesakDisk
    {
        public CZalesakDisk(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            resizeZalesakDisk(vResolution, vOrigin, vSpacing);
        }

        public void resizeZalesakDisk(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            m_Resolution = new Vector3Int(vResolution.x, vResolution.y, 1);
            m_Origin = vOrigin;
            m_Spacing = vSpacing;

            m_R = 0.1f * m_Resolution.x * m_Spacing.x;
            m_CenterX = m_Origin.x + 0.5f * m_Resolution.x * m_Spacing.x;
            m_CenterY = m_Origin.y + 0.65f * m_Resolution.x * m_Spacing.x;
            m_Width = 0.04f * m_Resolution.x * m_Spacing.x;
            m_Height = 0.2f * m_Resolution.x * m_Spacing.x;
            m_RecX = m_Origin.x + 0.5f * m_Resolution.x * m_Spacing.x;
            m_RecY = m_Origin.y + 0.6f * m_Resolution.x * m_Spacing.x;

            if (m_RhoField == null)
            {
                m_RhoField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            }
            else
            {
                m_RhoField.resize(m_Resolution, m_Origin, m_Spacing);
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

        public void initZalesakDiskVelField()
        {
            __generateRhoField();

            __generateInitVelField();
        }

        private void __generateRhoField()
        {
            CZalesakDiskInvoer.generateRhoFieldInvoker(m_R, m_CenterX, m_CenterY, m_Width, m_Height, m_RecX, m_RecY, m_RhoField);
        }

        private void __generateInitVelField()
        {
            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 0, 0);
            CZalesakDiskInvoer.generateInitVelFieldXInvoker(ArgumentResolution, m_Origin, m_Spacing, m_InitVelField.getGridDataX());

            ArgumentResolution = m_Resolution + new Vector3Int(0, 1, 0);
            CZalesakDiskInvoer.generateInitVelFieldYInvoker(ArgumentResolution, m_Origin, m_Spacing, m_InitVelField.getGridDataY());
        }

        private Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;

        private float m_R;
        private float m_CenterX;
        private float m_CenterY;
        private float m_Width;
        private float m_Height;
        private float m_RecX;
        private float m_RecY;

        private CCellCenteredScalarField m_RhoField;
        private CFaceCenteredVectorField m_InitVelField;
    }
}

