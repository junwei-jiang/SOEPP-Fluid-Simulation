using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

public class CFluidParticlesVisualizer : MonoBehaviour
{
    public CFluidParticlesVisualizer(CGridFluidSolver vVisualizedFluid, float vParticlesRadius = 0.025f)
    {
        init(vVisualizedFluid, vParticlesRadius);
    }

    public void init(CGridFluidSolver vVisualizedFluid, float vParticlesRadius = 0.025f)
    {
        m_GridFluidSolver = vVisualizedFluid;

        m_Resolution = m_GridFluidSolver.getResolution();
        m_Origin = m_GridFluidSolver.getOrigin();
        m_Spacing = m_GridFluidSolver.getSpacing();

        m_FluidParticlesVisualizerMaterial = Resources.Load("Materials/FluidParticlesVisualizer") as Material;

        m_FluidParticlesVisualizerMaterial.SetFloat("_ParticlesRadius", vParticlesRadius);

        m_FluidParticlesVisualizerMaterial.SetBuffer("ParticlesPos", m_GridFluidSolver.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos());
        m_FluidParticlesVisualizerMaterial.SetBuffer("ParticlesVel", m_GridFluidSolver.getParticlesAdvectionSolver().getEulerParticles().getParticlesVel());
    }

    public void setVisualizerFlag(bool vFlag)
    {
        m_IsVisualized = vFlag;
    }

    public void visualize()
    {
        if (m_IsVisualized)
        {
            //TODO:是否需要每帧更新
            m_FluidParticlesVisualizerMaterial.SetBuffer("ParticlesPos", m_GridFluidSolver.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos());
            m_FluidParticlesVisualizerMaterial.SetBuffer("ParticlesVel", m_GridFluidSolver.getParticlesAdvectionSolver().getEulerParticles().getParticlesVel());

            m_FluidParticlesVisualizerMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Quads, (int)m_GridFluidSolver.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles() * 4);
        }
    }

    private bool m_IsVisualized = true;

    private Vector3Int m_Resolution;
    private Vector3 m_Origin;
    private Vector3 m_Spacing;

    private Material m_FluidParticlesVisualizerMaterial;

    private CGridFluidSolver m_GridFluidSolver; 
}
