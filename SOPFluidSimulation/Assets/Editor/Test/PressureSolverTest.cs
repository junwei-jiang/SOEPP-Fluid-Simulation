using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class PressureSolverTest
{
    [Test]
    public void PressureSolverTest_BuildMarkers()
    {
        #region BuildMarkers1
        Vector3Int Resolution = new Vector3Int(3, 3, 3);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] ParticlesPosValue = new float[3];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        CEulerParticles EulerParticles = new CEulerParticles(1, 1, ParticlesPosValue);

        float[] SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[13] = 0.5f;
        SolidSDFFieldValue[16] = 0.5f;
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);
        CCellCenteredScalarField CCSMarkersField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);

        int[] MarkersFieldValueResult = new int[Resolution.x * Resolution.y * Resolution.z];
        CCSMarkersField.getGridData().GetData(MarkersFieldValueResult);

        for (int z = 0; z < 27; z++)
        {
            if (z == 13)
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else if(z == 16)
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 2) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        #endregion

        #region BuildMarkers2
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(10, 20, 30);

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        EulerParticles.resize(8, 8, ParticlesPosValue);

        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;
        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        CCSSolidSDFField.resize(Resolution, Origin, Spacing, SolidSDFFieldValue);
        CCSMarkersField.resize(Resolution, Origin, Spacing);

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);

        MarkersFieldValueResult = new int[Resolution.x * Resolution.y * Resolution.z];
        CCSMarkersField.getGridData().GetData(MarkersFieldValueResult);

        for (int z = 0; z < 64; z++)
        {
            if (z == 29 || z == 30 || z == 45 || z == 46)
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else if (z == 21 || z == 22 || z == 25 || z == 26 || z == 37 || z == 38 || z == 41 || z == 42)
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(MarkersFieldValueResult[z] - 2) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        #endregion
    }

    [Test]
    public void PressureSolverTest_BuildMatrixA()
    {
        #region BuildMatrixA1
        Vector3Int Resolution = new Vector3Int(3, 3, 3);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] ParticlesPosValue = new float[3];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        CEulerParticles EulerParticles = new CEulerParticles(1, 1, ParticlesPosValue);

        float[] SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[13] = 0.5f;
        SolidSDFFieldValue[16] = 0.5f;
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);
        CCellCenteredScalarField CCSMarkersField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        float[] FdmMatrixAValueResult = new float[4 * Resolution.x * Resolution.y * Resolution.z];
        ComputeBuffer FdmMatrixADataResult = new ComputeBuffer(FdmMatrixAValueResult.Length, sizeof(float));

        Vector3 Scale = new Vector3(1 / (1 * Spacing.x * Spacing.x), 1 / (1 * Spacing.y * Spacing.y), 1 / (1 * Spacing.z * Spacing.z));

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);
        CEulerSolverToolInvoker.buildPressureFdmMatrixAInvoker(Resolution, Scale, CCSMarkersField, FdmMatrixADataResult);

        FdmMatrixADataResult.GetData(FdmMatrixAValueResult);

        float InvY = 0.0025f;
        for (int z = 0; z < 27; z++)
        {
            if (z == 13)
            {
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z] - InvY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 1] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 2] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 3] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        #endregion

        #region BuildMatrixA2
        Resolution = new Vector3Int(4, 4, 4);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(10, 20, 30);

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        EulerParticles.resize(8, 8, ParticlesPosValue);

        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;
        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;
        CCSSolidSDFField.resize(Resolution, Origin, Spacing, SolidSDFFieldValue);
        CCSMarkersField.resize(Resolution, Origin, Spacing);

        FdmMatrixAValueResult = new float[4 * Resolution.x * Resolution.y * Resolution.z];
        FdmMatrixADataResult.Release();
        FdmMatrixADataResult = new ComputeBuffer(FdmMatrixAValueResult.Length, sizeof(float));

        Scale = new Vector3(2 / (1 * Spacing.x * Spacing.x), 2 / (1 * Spacing.y * Spacing.y), 2 / (1 * Spacing.z * Spacing.z));

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);
        CEulerSolverToolInvoker.buildPressureFdmMatrixAInvoker(Resolution, Scale, CCSMarkersField, FdmMatrixADataResult);

        FdmMatrixADataResult.GetData(FdmMatrixAValueResult);

        float InvX = 2 * 0.01f;
        InvY = 2 * 0.0025f;
        float InvZ = 2 * 0.00111111f;

        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 21] - InvX - InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 21 + 1] + InvX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 21 + 2] + InvY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 21 + 3] + InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 22] - InvX - InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 22 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 22 + 2] + InvY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 22 + 3] + InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 25] - InvX - 2 * InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 25 + 1] + InvX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 25 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 25 + 3] + InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 26] - InvX - 2 * InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 26 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 26 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 26 + 3] + InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 37] - InvX - InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 37 + 1] + InvX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 37 + 2] + InvY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 37 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 38] - InvX - InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 38 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 38 + 2] + InvY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 38 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 41] - InvX - 2 * InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 41 + 1] + InvX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 41 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 41 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 42] - InvX - 2 * InvY - InvZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 42 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 42 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * 42 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < 64; z++)
        {
            if (z != 29 && z != 30 && z != 45 && z != 46 && z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z] - 1) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(FdmMatrixAValueResult[4 * z + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        #endregion
    }

    [Test]
    public void PressureSolverTest_BuildVectorb()
    {
        #region BuildVectorb1
        Vector3Int Resolution = new Vector3Int(3, 3, 3);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] ParticlesPosValue = new float[3];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        CEulerParticles EulerParticles = new CEulerParticles(1, 1, ParticlesPosValue);

        float[] SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[13] = 0.5f;
        SolidSDFFieldValue[16] = 0.5f;
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        float[] FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x + 1;
                }
            }
        }
        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }
        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }
        CFaceCenteredVectorField FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        CFaceCenteredVectorField FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCellCenteredScalarField CCSFluidVelDivergenceField = new CCellCenteredScalarField(Resolution, Origin, Spacing);
        CCellCenteredScalarField CCSMarkersField = new CCellCenteredScalarField(Resolution, Origin, Spacing);

        float[] VectorbValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
        ComputeBuffer VectorbDataResult = new ComputeBuffer(VectorbValueResult.Length, sizeof(float));

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);
        FCVFluidVelField.divergence(CCSFluidVelDivergenceField);
        CEulerSolverToolInvoker.buildPressureVectorbInvoker(FCVFluidVelField, CCSFluidVelDivergenceField, CCSMarkersField, FCVSolidVelField, VectorbDataResult);

        VectorbDataResult.GetData(VectorbValueResult);

        for (int z = 0; z < 27; z++)
        {
            if (z != 13)
            {
                Assert.IsTrue(Mathf.Abs(VectorbValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(VectorbValueResult[z] + 0.2f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        #endregion

        #region BuildVectorb2
        Resolution = new Vector3Int(4, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(10, 20, 30);

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        EulerParticles.resize(8, 8, ParticlesPosValue);

        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }
        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;
        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;
        CCSSolidSDFField.resize(Resolution, Origin, Spacing, SolidSDFFieldValue);
        CCSMarkersField.resize(Resolution, Origin, Spacing);
        CCSFluidVelDivergenceField.resize(Resolution, Origin, Spacing);

        FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 1;
                }
            }
        }
        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }
        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }
        FCVFluidVelField.resize(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        FCVSolidVelField.resize(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);

        VectorbValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
        VectorbDataResult.Release();
        VectorbDataResult = new ComputeBuffer(VectorbValueResult.Length, sizeof(float));

        CEulerParticlesInvokers.buildFluidMarkersInvoker(CCSSolidSDFField, EulerParticles.getParticlesPos(), CCSMarkersField);
        FCVFluidVelField.divergence(CCSFluidVelDivergenceField);
        CEulerSolverToolInvoker.buildPressureVectorbInvoker(FCVFluidVelField, CCSFluidVelDivergenceField, CCSMarkersField, FCVSolidVelField, VectorbDataResult);

        VectorbDataResult.GetData(VectorbValueResult);

        for (int z = 0; z < 64; z++)
        {
            if (z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(VectorbValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        Assert.IsTrue(Mathf.Abs(VectorbValueResult[21] + 0.26666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[22] + 0.06666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[25] + 0.21666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[26] + 0.01666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[37] + 0.13333333f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[38] - 0.06666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[41] + 0.08333333f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[42] - 0.11666666f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion
    }

    [Test]
    public void PressureSolverTest_MatrixFreePCG()
    {
        #region MatrixFreePCG1
        Vector3Int Resolution = new Vector3Int(4, 4, 4);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] DstVectorx = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] VectorbValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstMatrixA = new float[4 * Resolution.x * Resolution.y * Resolution.z];

        float[] ParticlesPosValue = new float[3 * 8];
        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        CEulerParticles EulerParticles = new CEulerParticles(8, 8, ParticlesPosValue);

        float[] SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }    
        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;
        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        float[] FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 1;
                }
            }
        }
        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }
        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }

        CFaceCenteredVectorField FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        CFaceCenteredVectorField FCVFluidVelFieldCOPY = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        CFaceCenteredVectorField FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        CPressureSolver PressureSolver = new CPressureSolver(Resolution, Origin, Spacing);

        for(int z = 0; z < 100; z++)
        {
            FCVFluidVelField.resize(FCVFluidVelFieldCOPY);
            PressureSolver.executeHelmholtzHodgDecomposition(FCVFluidVelField, 2, FCVSolidVelField, CCSSolidSDFField, EulerParticles.getParticlesPos());
        }

        PressureSolver.getVectorx().GetData(DstVectorx);
        PressureSolver.getVectorb().GetData(VectorbValueResult);
        PressureSolver.getFdmMatrixA().GetData(DstMatrixA);

        Assert.IsTrue(Mathf.Abs(DstVectorx[21] + 64.10625691f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[22] + 59.1624367f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[25] + 40.88984875f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[26] + 36.39546673f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[37] + 40.83756019f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[38] + 35.8937402f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[41] + 23.60453116f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[42] + 19.11014936f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion
    }

    [Test]
    public void PressureSolverTest_SolvePressure()
    {
        #region SolvePressure1
        Vector3Int Resolution = new Vector3Int(4, 4, 4);
        Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(0, 0, 0);
        Vector3 Spacing = new Vector3(10, 20, 30);

        float[] FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstVelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        float[] DstVelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        float[] DstVelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        float[] ParticlesPosValue = new float[3 * 8];
        ComputeBuffer ParticlesPosData = new ComputeBuffer(ParticlesPosValue.Length, sizeof(float));

        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosData.SetData(ParticlesPosValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }

        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;

        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x + 1;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y + 1;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z + 1;
                }
            }
        }

        CFaceCenteredVectorField FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        CFaceCenteredVectorField FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        CPressureSolver PressureSolver = new CPressureSolver(Resolution, Origin, Spacing);

        PressureSolver.executeHelmholtzHodgDecomposition(FCVFluidVelField, 2, FCVSolidVelField, CCSSolidSDFField, ParticlesPosData);

        FCVFluidVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
        FCVFluidVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
        FCVFluidVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[26] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[28] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[31] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[33] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[46] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[48] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[51] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[53] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[25] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[26] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[45] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[46] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[21] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[22] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[25] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[26] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[53] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[54] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[57] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[58] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region SolvePressure2
        Resolution = new Vector3Int(4, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(10, 20, 30);

        FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosData = new ComputeBuffer(ParticlesPosValue.Length, sizeof(float));

        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosData.SetData(ParticlesPosValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }

        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;

        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 1;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }

        FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        PressureSolver = new CPressureSolver(Resolution, Origin, Spacing);

        PressureSolver.executeHelmholtzHodgDecomposition(FCVFluidVelField, 2, FCVSolidVelField, CCSSolidSDFField, ParticlesPosData);

        FCVFluidVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
        FCVFluidVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
        FCVFluidVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[27] - 1.011228f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[32] - 1.101123f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[47] - 1.011228f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[52] - 1.101123f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[29] + 0.3216407f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[30] + 0.2766933f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[33] + 1.088985f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[34] + 0.6395467f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[49] - 0.276697f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[50] - 0.321641f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[53] - 0.639547f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[54] - 1.088985f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[37] - 0.4487535f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[38] - 0.448756f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[41] - 0.8475453f) < 10 * CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[42] - 0.8476455f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region SolverPressure3
        Resolution = new Vector3Int(4, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        float[] DstMatrixA = new float[4 * Resolution.x * Resolution.y * Resolution.z];
        float[] VectorbValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
        float[] DstVectorx = new float[Resolution.x * Resolution.y * Resolution.z];

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosData = new ComputeBuffer(ParticlesPosValue.Length, sizeof(float));

        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosData.SetData(ParticlesPosValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }

        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;

        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 4 - x;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 1;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }

        FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        PressureSolver = new CPressureSolver(Resolution, Origin, Spacing);

        PressureSolver.executeHelmholtzHodgDecomposition(FCVFluidVelField, 1, FCVSolidVelField, CCSSolidSDFField, ParticlesPosData);

        PressureSolver.getFdmMatrixA().GetData(DstMatrixA);

        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < 64; z++)
        {
            if (z != 29 && z != 30 && z != 45 && z != 46 && z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 1] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 2] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 3] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        PressureSolver.getVectorb().GetData(VectorbValueResult);

        for (int z = 0; z < 64; z++)
        {
            if (z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(VectorbValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[21] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[22] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[25] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[26] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[37] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[38] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[41] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[42] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        PressureSolver.getVectorx().GetData(DstVectorx);

        for (int z = 0; z < 64; z++)
        {
            if (z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(DstVectorx[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        Assert.IsTrue(Mathf.Abs(DstVectorx[21] + 0.45454545f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[22] - 0.45454545f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[25] + 0.36363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[26] - 0.36363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[37] + 0.45454545f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[38] - 0.45454545f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[41] + 0.36363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[42] - 0.36363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        FCVFluidVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
        FCVFluidVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
        FCVFluidVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[26] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[28] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[31] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[33] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[46] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[48] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[51] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[53] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[27] - 1.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[32] - 1.272727f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[47] - 1.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[52] - 1.272727f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[29] + 0.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[30] - 0.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[33] + 0.363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[34] - 0.363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[49] + 0.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[50] - 0.090909f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[53] + 0.363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[54] - 0.363636f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[37] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[38] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[41] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[42] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region SolverPressure4
        Resolution = new Vector3Int(4, 4, 4);
        ResolutionX = Resolution + new Vector3Int(1, 0, 0);
        ResolutionY = Resolution + new Vector3Int(0, 1, 0);
        ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0, 0, 0);
        Spacing = new Vector3(1, 1, 1);

        FluidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        FluidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        FluidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        SolidSDFFieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
        SolidVelFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        SolidVelFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        SolidVelFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstVelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
        DstVelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
        DstVelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
        DstMatrixA = new float[4 * Resolution.x * Resolution.y * Resolution.z];
        VectorbValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
        DstVectorx = new float[Resolution.x * Resolution.y * Resolution.z];

        ParticlesPosValue = new float[3 * 8];
        ParticlesPosData = new ComputeBuffer(ParticlesPosValue.Length, sizeof(float));

        ParticlesPosValue[0] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[1] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[2] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[3] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[4] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[5] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[6] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[7] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[8] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[9] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[10] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[11] = Origin.z + 1.5f * Spacing.z;
        ParticlesPosValue[12] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[13] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[14] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[15] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[16] = Origin.y + 1.5f * Spacing.y;
        ParticlesPosValue[17] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[18] = Origin.x + 1.5f * Spacing.x;
        ParticlesPosValue[19] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[20] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosValue[21] = Origin.x + 2.5f * Spacing.x;
        ParticlesPosValue[22] = Origin.y + 2.5f * Spacing.y;
        ParticlesPosValue[23] = Origin.z + 2.5f * Spacing.z;
        ParticlesPosData.SetData(ParticlesPosValue);

        for (int z = 0; z < Resolution.z; z++)
        {
            for (int y = 0; y < Resolution.y; y++)
            {
                for (int x = 0; x < Resolution.x; x++)
                {
                    SolidSDFFieldValue[z * Resolution.x * Resolution.y + y * Resolution.x + x] = -0.5f;
                }
            }
        }

        SolidSDFFieldValue[17] = 0.5f;
        SolidSDFFieldValue[18] = 0.5f;
        SolidSDFFieldValue[21] = 0.5f;
        SolidSDFFieldValue[22] = 0.5f;
        SolidSDFFieldValue[25] = 0.5f;
        SolidSDFFieldValue[26] = 0.5f;
        SolidSDFFieldValue[29] = 0.5f;
        SolidSDFFieldValue[30] = 0.5f;

        SolidSDFFieldValue[33] = 0.5f;
        SolidSDFFieldValue[34] = 0.5f;
        SolidSDFFieldValue[37] = 0.5f;
        SolidSDFFieldValue[38] = 0.5f;
        SolidSDFFieldValue[41] = 0.5f;
        SolidSDFFieldValue[42] = 0.5f;
        SolidSDFFieldValue[45] = 0.5f;
        SolidSDFFieldValue[46] = 0.5f;

        for (int z = 0; z < ResolutionX.z; z++)
        {
            for (int y = 0; y < ResolutionX.y; y++)
            {
                for (int x = 0; x < ResolutionX.x; x++)
                {
                    FluidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 0;
                    SolidVelFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 0;
                }
            }
        }

        for (int z = 0; z < ResolutionY.z; z++)
        {
            for (int y = 0; y < ResolutionY.y; y++)
            {
                for (int x = 0; x < ResolutionY.x; x++)
                {
                    FluidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = -9.8f;
                    SolidVelFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
                }
            }
        }

        for (int z = 0; z < ResolutionZ.z; z++)
        {
            for (int y = 0; y < ResolutionZ.y; y++)
            {
                for (int x = 0; x < ResolutionZ.x; x++)
                {
                    FluidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                    SolidVelFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
                }
            }
        }

        FCVFluidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, FluidVelFieldValueX, FluidVelFieldValueY, FluidVelFieldValueZ);
        FCVSolidVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SolidVelFieldValueX, SolidVelFieldValueY, SolidVelFieldValueZ);
        CCSSolidSDFField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SolidSDFFieldValue);

        PressureSolver = new CPressureSolver(Resolution, Origin, Spacing);

        PressureSolver.executeHelmholtzHodgDecomposition(FCVFluidVelField, 1, FCVSolidVelField, CCSSolidSDFField, ParticlesPosData);

        PressureSolver.getFdmMatrixA().GetData(DstMatrixA);

        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 21 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 22 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 25 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 26 + 3] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 37 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 2] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 38 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 1] + 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 41 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 1] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 2] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * 42 + 3] + 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        for (int z = 0; z < 64; z++)
        {
            if (z != 29 && z != 30 && z != 45 && z != 46 && z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 1] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 2] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                Assert.IsTrue(Mathf.Abs(DstMatrixA[4 * z + 3] + 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        PressureSolver.getVectorb().GetData(VectorbValueResult);

        for (int z = 0; z < 64; z++)
        {
            if (z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(VectorbValueResult[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[21] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[22] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[25] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[26] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[37] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[38] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[41] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(VectorbValueResult[42] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        PressureSolver.getVectorx().GetData(DstVectorx);

        for (int z = 0; z < 64; z++)
        {
            if (z != 21 && z != 22 && z != 25 && z != 26 && z != 37 && z != 38 && z != 41 && z != 42)
            {
                Assert.IsTrue(Mathf.Abs(DstVectorx[z] - 0) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }
        Assert.IsTrue(Mathf.Abs(DstVectorx[21] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[22] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[25] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[26] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[37] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[38] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[41] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVectorx[42] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        FCVFluidVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
        FCVFluidVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
        FCVFluidVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[26] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[28] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[31] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[33] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[46] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[48] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[51] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[53] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[27] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[32] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[47] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[52] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[29] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[30] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[33] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[34] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[49] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[50] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[53] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[54] + 9.8f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[37] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[38] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[41] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[42] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion
    }

    [Test]
    public void SolveCGImprovedTest()
    {
        #region InitData
        int Size = 100000;

        ComputeBuffer X = new ComputeBuffer(Size, sizeof(float));
        ComputeBuffer R = new ComputeBuffer(Size, sizeof(float));
        ComputeBuffer P = new ComputeBuffer(Size, sizeof(float));
        ComputeBuffer AP = new ComputeBuffer(Size, sizeof(float));
        ComputeBuffer Alpha = new ComputeBuffer(1, sizeof(float));
        ComputeBuffer RkDotRk = new ComputeBuffer(1, sizeof(float));
        ComputeBuffer Beta = new ComputeBuffer(1, sizeof(float));

        float[] ArrayX = new float[Size];
        float[] ArrayR = new float[Size];
        float[] ArrayP = new float[Size];
        float[] ArrayAP = new float[Size];
        float[] ArrayAlpha = new float[1];
        float[] ArrayRkDotRk = new float[1];
        float[] ArrayBeta = new float[1];

        for (int z = 0; z < Size; z++)
        {
            ArrayX[z] = Random.Range(-10, 10);
            ArrayR[z] = Random.Range(-10, 10);
            ArrayP[z] = Random.Range(-10, 10);
            ArrayAP[z] = Random.Range(-10, 10);
        }
        X.SetData(ArrayX);
        R.SetData(ArrayR);
        P.SetData(ArrayP);
        AP.SetData(ArrayAP);
        #endregion

        #region ComputeAlpha
        float RkdotRkSatandard = CMathTool.dot(R, R);
        float PdotAP = CMathTool.dot(P, AP);
        float AlphaStandard = RkdotRkSatandard / PdotAP;

        CMatrixFreePCGToolInvokers.computeAlphaInvoker(R, P, AP, Alpha, RkDotRk);
        Alpha.GetData(ArrayAlpha);
        RkDotRk.GetData(ArrayRkDotRk);

        Assert.IsTrue(Mathf.Abs(AlphaStandard - ArrayAlpha[0]) < 1e-3f);
        Assert.IsTrue(Mathf.Abs(RkdotRkSatandard - ArrayRkDotRk[0]) < 1e-3f);
        #endregion

        #region ComputeBeta
        CMathTool.plusAlphaX(X, P, AlphaStandard);
        CMathTool.plusAlphaX(R, AP, -AlphaStandard);
        float NewRkDotNewRk = CMathTool.dot(R, R);
        float BetaStandard = NewRkDotNewRk / RkdotRkSatandard;

        X.SetData(ArrayX);
        R.SetData(ArrayR);
        CMatrixFreePCGToolInvokers.computeBetaInvoker(1e-2f, Alpha, P, AP, RkDotRk, X, R, Beta);
        Beta.GetData(ArrayBeta);

        Assert.IsTrue(Mathf.Abs(BetaStandard - ArrayBeta[0]) < 1e-2);
        #endregion
    }
}
