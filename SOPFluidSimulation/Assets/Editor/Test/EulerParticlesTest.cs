using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class EulerParticlesTest
{
    [Test]
    public void generateParticlesInFluidTest()
    {
        CGlobalMacroAndFunc.init();

        //初次生成流体
        #region Test1

        Vector3Int Res = new Vector3Int(4, 4, 4);
        Vector3 Origin = new Vector3(-10.0f, -20.0f, -30.0f);
        Vector3 Spacing = new Vector3(10.0f, 20.0f, 30.0f);

        int NumOfPerGrid = 8;

        float[] FluidSDFValue = new float[Res.x * Res.y * Res.z];
        float[] SolidSDFValue = new float[Res.x * Res.y * Res.z];

        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            FluidSDFValue[i] = 1.0f;
            SolidSDFValue[i] = -1.0f;
        }

        FluidSDFValue[21] = -1.0f; SolidSDFValue[21] = 1.0f;
        FluidSDFValue[22] = -1.0f; SolidSDFValue[22] = 1.0f;
        FluidSDFValue[25] = -1.0f; SolidSDFValue[25] = 1.0f;
        FluidSDFValue[26] = -1.0f; SolidSDFValue[26] = 1.0f;
        FluidSDFValue[37] = -1.0f; SolidSDFValue[37] = 1.0f;
        FluidSDFValue[38] = -1.0f; SolidSDFValue[38] = 1.0f;
        FluidSDFValue[41] = -1.0f; SolidSDFValue[41] = 1.0f;
        FluidSDFValue[42] = -1.0f; SolidSDFValue[42] = 1.0f;

        SolidSDFValue[29] = 1.0f;
        SolidSDFValue[30] = 1.0f;
        SolidSDFValue[45] = 1.0f;
        SolidSDFValue[46] = 1.0f;

        CCellCenteredScalarField CCSFluidSDFField = new CCellCenteredScalarField(Res, Origin, Spacing, FluidSDFValue);
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Res, Origin, Spacing, SolidSDFValue);

        CEulerParticles EulerParticles = new CEulerParticles();
        EulerParticles.addParticles(CCSFluidSDFField, CCSSolidSDFField, NumOfPerGrid);

        Assert.IsTrue(EulerParticles.getNumOfParticles() == 8 * NumOfPerGrid);

        ComputeBuffer ParticlesPos = EulerParticles.getParticlesPos();
        float[] ParticlesPosResult = new float[EulerParticles.getNumOfParticles() * 3];
        ParticlesPos.GetData(ParticlesPosResult);

        for (int i = 0; i < EulerParticles.getNumOfParticles(); i++)
        {
            Vector3 FluidGridMin = Origin + mul(new Vector3(1.0f, 1.0f, 1.0f), Spacing);
            Vector3 FluidGridMax = Origin + mul(new Vector3(3.0f, 3.0f, 3.0f), Spacing);

            Assert.GreaterOrEqual(ParticlesPosResult[3 * i], FluidGridMin.x);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMin.y);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMin.z);
            Assert.LessOrEqual(ParticlesPosResult[3 * i], FluidGridMax.x);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMax.y);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMax.z);
        }
        #endregion

        //后续添加流体
        #region Test2

        float[] ExternalFluidSDFValue = new float[Res.x * Res.y * Res.z];
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            ExternalFluidSDFValue[i] = 1.0f;
        }

        ExternalFluidSDFValue[21] = -1.0f;
        ExternalFluidSDFValue[22] = -1.0f;
        ExternalFluidSDFValue[25] = -1.0f;
        ExternalFluidSDFValue[26] = -1.0f;
        ExternalFluidSDFValue[37] = -1.0f;
        ExternalFluidSDFValue[38] = -1.0f;
        ExternalFluidSDFValue[41] = -1.0f;
        ExternalFluidSDFValue[42] = -1.0f;
        ExternalFluidSDFValue[29] = -1.0f;
        ExternalFluidSDFValue[30] = -1.0f;

        CCellCenteredScalarField CCSExternalFluidSDFField = new CCellCenteredScalarField(Res, Origin, Spacing, ExternalFluidSDFValue);

        EulerParticles.addParticles(CCSExternalFluidSDFField, CCSFluidSDFField, CCSSolidSDFField, NumOfPerGrid);

        Assert.IsTrue(EulerParticles.getNumOfParticles() == 10 * NumOfPerGrid);

        ParticlesPos = EulerParticles.getParticlesPos();
        ParticlesPosResult = new float[EulerParticles.getNumOfParticles() * 3];
        ParticlesPos.GetData(ParticlesPosResult);

        for (int i = 0; i < EulerParticles.getNumOfParticles(); i++)
        {
            Vector3 FluidGridMin = Origin + mul(new Vector3(1.0f, 1.0f, 1.0f), Spacing);
            Vector3 FluidGridMax = Origin + mul(new Vector3(3.0f, 4.0f, 3.0f), Spacing);

            Assert.GreaterOrEqual(ParticlesPosResult[3 * i], FluidGridMin.x);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMin.y);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMin.z);
            Assert.LessOrEqual(ParticlesPosResult[3 * i], FluidGridMax.x);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMax.y);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMax.z);
        }
        #endregion

        //使用BBox的方式添加流体
        #region Test3

        EulerParticles.resize();

        SBoundingBox FluidBBox = new SBoundingBox(Origin + mul(new Vector3(1.0f, 2.5f, 1.0f), Spacing), Origin + mul(new Vector3(3.0f, 4.0f, 3.0f), Spacing));

        EulerParticles.addParticles(FluidBBox, CCSSolidSDFField, NumOfPerGrid);

        Assert.IsTrue(EulerParticles.getNumOfParticles() >= 4 * NumOfPerGrid);
        Assert.IsTrue(EulerParticles.getNumOfParticles() <= 8 * NumOfPerGrid);

        ParticlesPos = EulerParticles.getParticlesPos();
        ParticlesPosResult = new float[EulerParticles.getNumOfParticles() * 3];
        ParticlesPos.GetData(ParticlesPosResult);

        for (int i = 0; i < EulerParticles.getNumOfParticles(); i++)
        {
            Vector3 FluidGridMin = Origin + mul(new Vector3(1.0f, 2.5f, 1.0f), Spacing);
            Vector3 FluidGridMax = Origin + mul(new Vector3(3.0f, 4.0f, 3.0f), Spacing);

            Assert.GreaterOrEqual(ParticlesPosResult[3 * i], FluidGridMin.x);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMin.y);
            Assert.GreaterOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMin.z);
            Assert.LessOrEqual(ParticlesPosResult[3 * i], FluidGridMax.x);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 1], FluidGridMax.y);
            Assert.LessOrEqual(ParticlesPosResult[3 * i + 2], FluidGridMax.z);
        }
        #endregion

        #region free
        CCSFluidSDFField.free();
        CCSSolidSDFField.free();
        EulerParticles.free();
        if (ParticlesPos != null)
        {
            ParticlesPos.Release();
        }
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void deleteOutsideParticlesTest()
    {
        CGlobalMacroAndFunc.init();

        Vector3Int Res = new Vector3Int(4, 4, 4);
        Vector3 Origin = new Vector3(-10.0f, -20.0f, -30.0f);
        Vector3 Spacing = new Vector3(10.0f, 20.0f, 30.0f);

        float[] ParticlesPosValue = new float[12];
        float[] ParticlesVelValue = new float[12];
        ParticlesPosValue[0] = -20.0f;
        ParticlesPosValue[1] = 10.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 8.0f;
        ParticlesPosValue[4] = 69.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesPosValue[6] = 5.0f;
        ParticlesPosValue[7] = 10.0f;
        ParticlesPosValue[8] = 20.0f;
        ParticlesPosValue[9] = 10.0f;
        ParticlesPosValue[10] = 13.0f;
        ParticlesPosValue[11] = 102.0f;

        for (int i = 0; i < 12; i++)
        {
            ParticlesVelValue[i] = i;
        }

        CEulerParticles EulerParticles = new CEulerParticles(4, 4, ParticlesPosValue, ParticlesVelValue);

        EulerParticles.deleteOutsideParticles(Res, Origin, Spacing);

        Assert.IsTrue(Mathf.Abs(EulerParticles.getNumOfParticles() - 1.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        float[] ParticlesPosValueResult = new float[EulerParticles.getNumOfParticles() * 3];
        float[] ParticlesVelValueResult = new float[EulerParticles.getNumOfParticles() * 3];
        EulerParticles.getParticlesPos().GetData(ParticlesPosValueResult);
        EulerParticles.getParticlesVel().GetData(ParticlesVelValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[0] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[1] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[2] - 20.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVelValueResult[0] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVelValueResult[1] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVelValueResult[2] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void tranferParticles2CCSFieldTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        float[] ParticlesScalarValue = new float[2];
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;
        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ComputeBuffer ParticlesScalarData = new ComputeBuffer(2, sizeof(float));
        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCellCenteredScalarField CCSParticlesScalarField = new CCellCenteredScalarField(Res, Origin, Spacing);
        CCellCenteredScalarField CCSParticlesWeightField = new CCellCenteredScalarField(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField
        );

        float[] ParticlesScalarValueResult = new float[CCSParticlesScalarField.getGridData().count];
        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[2] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[4] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[5] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[6] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[7] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[8] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[9] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[10] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[11] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[13] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[14] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[15] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region LinearTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCSParticlesScalarField.resize(Res, Origin, Spacing);
        CCSParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField
        );

        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        for (int i = 0; i < 16; i++)
        {
            if (i != 5 && i != 6 && i != 9 && i != 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[i] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[i] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCSParticlesScalarField.resize(Res, Origin, Spacing);
        CCSParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[2] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[4] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[5] - 38.0f * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[6] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[7] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[8] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[9] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[10] - 73.0f * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[11] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[13] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[14] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[15] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCSParticlesScalarField.resize(Res, Origin, Spacing);
        CCSParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        for (int i = 0; i < 16; i++)
        {
            if (i != 5 && i != 6 && i != 9 && i != 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[i] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[i] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCSParticlesScalarField.resize(Res, Origin, Spacing);
        CCSParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[2] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[4] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[5] - (64.0f * 16.0f + 128.0f) / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[6] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[7] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[8] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[9] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[10] - (64.0f + 128.0f * 16.0f) / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[11] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[13] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[14] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[15] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesScalarValue[0] = 64f;
        ParticlesScalarValue[1] = 128f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesScalarData.SetData(ParticlesScalarValue);

        CCSParticlesScalarField.resize(Res, Origin, Spacing);
        CCSParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarData,
            CCSParticlesScalarField,
            CCSParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        CCSParticlesScalarField.getGridData().GetData(ParticlesScalarValueResult);

        for (int i = 0; i < 16; i++)
        {
            Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[i] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        }
        #endregion

        #region free
        EulerParticles.free();
        ParticlesScalarData.Release();
        CCSParticlesScalarField.free();
        CCSParticlesWeightField.free();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void tranferParticles2CCVFieldTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        float[] ParticlesVectorValue = new float[6];
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;
        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ComputeBuffer ParticlesVectorData = new ComputeBuffer(6, sizeof(float));
        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCellCenteredVectorField CCVParticlesVectorField = new CCellCenteredVectorField(Res, Origin, Spacing);
        CCellCenteredVectorField CCVParticlesWeightField = new CCellCenteredVectorField(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.LINEAR
        );

        float[] ParticlesVectorValueResultX = new float[CCVParticlesVectorField.getGridDataX().count];
        float[] ParticlesVectorValueResultY = new float[CCVParticlesVectorField.getGridDataY().count];
        float[] ParticlesVectorValueResultZ = new float[CCVParticlesVectorField.getGridDataZ().count];

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 16; i++)
        {
            if (i == 5)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else if (i == 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region LinearTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCVParticlesVectorField.resize(Res, Origin, Spacing);
        CCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.LINEAR
        );

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 16; i++)
        {
            if (i == 5 || i == 6 || i == 9 || i == 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCVParticlesVectorField.resize(Res, Origin, Spacing);
        CCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[0] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[1] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[2] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[4] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[5] - (9.0f / 16.0f * 2.0f + 1.0f / 64.0f * 8.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[6] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[7] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[8] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[9] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[10] - (1.0f / 64.0f * 2.0f + 9.0f / 16.0f * 8.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[11] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[13] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[14] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[15] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[0] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[1] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[2] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[4] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[5] - (9.0f / 16.0f * 4.0f + 1.0f / 64.0f * 10.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[6] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[7] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[8] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[9] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[10] - (1.0f / 64.0f * 4.0f + 9.0f / 16.0f * 10.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[11] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[13] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[14] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[15] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[0] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[1] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[2] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[4] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[5] - (9.0f / 16.0f * 6.0f + 1.0f / 64.0f * 12.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[6] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[7] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[8] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[9] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[10] - (1.0f / 64.0f * 6.0f + 9.0f / 16.0f * 12.0f) * 64.0f / 37.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[11] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[13] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[14] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[15] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCVParticlesVectorField.resize(Res, Origin, Spacing);
        CCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 16; i++)
        {
            if (i == 5 || i == 6 || i == 9 || i == 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -25.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -15.0f;
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCVParticlesVectorField.resize(Res, Origin, Spacing);
        CCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[0] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[1] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[2] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[4] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[5] - (4.0f / 9.0f * 2.0f + 1.0f / 36.0f * 8.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[6] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[7] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[8] - 2.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[9] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[10] - (1.0f / 36.0f * 2.0f + 4.0f / 9.0f * 8.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[11] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[13] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[14] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[15] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[0] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[1] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[2] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[4] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[5] - (4.0f / 9.0f * 4.0f + 1.0f / 36.0f * 10.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[6] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[7] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[8] - 4.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[9] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[10] - (1.0f / 36.0f * 4.0f + 4.0f / 9.0f * 10.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[11] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[13] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[14] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[15] - 10.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[0] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[1] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[2] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[3] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[4] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[5] - (4.0f / 9.0f * 6.0f + 1.0f / 36.0f * 12.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[6] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[7] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[8] - 6.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[9] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[10] - (1.0f / 36.0f * 6.0f + 4.0f / 9.0f * 12.0f) * 36.0f / 17.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[11] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[12] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[13] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[14] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[15] - 12.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 2;
        ParticlesVectorValue[1] = 4;
        ParticlesVectorValue[2] = 6;
        ParticlesVectorValue[3] = 8;
        ParticlesVectorValue[4] = 10;
        ParticlesVectorValue[5] = 12;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        CCVParticlesVectorField.resize(Res, Origin, Spacing);
        CCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            CCVParticlesVectorField,
            CCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        CCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        CCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        CCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 16; i++)
        {
            Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 5.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 7.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 9.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        }
        #endregion

        #region free
        EulerParticles.free();
        ParticlesVectorData.Release();
        CCVParticlesVectorField.free();
        CCVParticlesWeightField.free();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void tranferParticles2FCVFieldTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 30.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        float[] ParticlesVectorValue = new float[6];
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ComputeBuffer ParticlesVectorData = new ComputeBuffer(6, sizeof(float));
        ParticlesVectorData.SetData(ParticlesVectorValue);

        CFaceCenteredVectorField FCVParticlesVectorField = new CFaceCenteredVectorField(Res, Origin, Spacing);
        CFaceCenteredVectorField FCVParticlesWeightField = new CFaceCenteredVectorField(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.LINEAR
        );

        float[] ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        float[] ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        float[] ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 20; i++)
        {
            if (i == 6)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else if (i == 12)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[i] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region LinearTest2
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 40.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 20; i++)
        {
            if (i == 5)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else if (i == 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[i] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region LinearTest3
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -20.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);


        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        for (int i = 0; i < 20; i++)
        {
            if (i == 5)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else if (i == 10)
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[i] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
            }
        }
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 30.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[0] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[1] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[2] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[4] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[5] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[6] - (9.0f / 16.0f * 1.0f + 1.0f / 64.0f * 2.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[7] - 1.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[8] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[9] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[10] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[11] - 1.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[12] - (1.0f / 64.0f * 1.0f + 9.0f / 16.0f * 2.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[13] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[14] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[15] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[16] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[17] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[18] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 40.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[0] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[1] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[2] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[4] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[5] - (9.0f / 16.0f * 3.0f + 1.0f / 64.0f * 4.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[6] - 3.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[7] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[8] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[9] - 3.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[10] - (1.0f / 64.0f * 3.0f + 9.0f / 16.0f * 4.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[11] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[13] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[14] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[15] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[16] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[17] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[18] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region QuadraticTest3
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -20.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);


        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[0] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[1] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[2] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[4] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[5] - (9.0f / 16.0f * 5.0f + 1.0f / 64.0f * 6.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[6] - 5.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[7] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[8] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[9] - 5.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[10] - (1.0f / 64.0f * 5.0f + 9.0f / 16.0f * 6.0f) * 64.0f / 37.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[11] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[13] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[14] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[15] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[16] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[17] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[18] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 30.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[0] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[1] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[2] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[4] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[5] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[6] - (4.0f / 9.0f * 1.0f + 1.0f / 36.0f * 2.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[7] - 1.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[8] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[9] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[10] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[11] - 1.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[12] - (1.0f / 36.0f * 1.0f + 4.0f / 9.0f * 2.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[13] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[14] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[15] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[16] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[17] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[18] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultX[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 40.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);

        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[0] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[1] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[2] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[4] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[5] - (4.0f / 9.0f * 3.0f + 1.0f / 36.0f * 4.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[6] - 3.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[7] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[8] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[9] - 3.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[10] - (1.0f / 36.0f * 3.0f + 4.0f / 9.0f * 4.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[11] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[13] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[14] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[15] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[16] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[17] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[18] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultY[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region CubicTest3
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -20.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        ParticlesVectorValue[0] = 1;
        ParticlesVectorValue[1] = 3;
        ParticlesVectorValue[2] = 5;
        ParticlesVectorValue[3] = 2;
        ParticlesVectorValue[4] = 4;
        ParticlesVectorValue[5] = 6;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue, ParticlesVectorValue);

        ParticlesVectorData.SetData(ParticlesVectorValue);

        FCVParticlesVectorField.resize(Res, Origin, Spacing);
        FCVParticlesWeightField.resize(Res, Origin, Spacing);


        CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorData,
            FCVParticlesVectorField,
            FCVParticlesWeightField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorValueResultX = new float[FCVParticlesVectorField.getGridDataX().count];
        FCVParticlesVectorField.getGridDataX().GetData(ParticlesVectorValueResultX);
        ParticlesVectorValueResultY = new float[FCVParticlesVectorField.getGridDataY().count];
        FCVParticlesVectorField.getGridDataY().GetData(ParticlesVectorValueResultY);
        ParticlesVectorValueResultZ = new float[FCVParticlesVectorField.getGridDataZ().count];
        FCVParticlesVectorField.getGridDataZ().GetData(ParticlesVectorValueResultZ);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[0] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[1] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[2] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[3] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[4] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[5] - (4.0f / 9.0f * 5.0f + 1.0f / 36.0f * 6.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[6] - 5.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[7] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[8] - 5.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[9] - 5.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[10] - (1.0f / 36.0f * 5.0f + 4.0f / 9.0f * 6.0f) * 36.0f / 17.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[11] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[12] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[13] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[14] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[15] - 6.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[16] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[17] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[18] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResultZ[19] - 0.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST_EXTERNAL);
        #endregion

        #region free
        EulerParticles.free();
        ParticlesVectorData.Release();
        FCVParticlesVectorField.free();
        FCVParticlesWeightField.free();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void transferCCSField2ParticlesTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        float[] CCSFieldValue = new float[Res.x * Res.y * Res.z];
        CCSFieldValue[0] = 64.0f;
        CCSFieldValue[1] = 64.0f;
        CCSFieldValue[2] = 64.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 64.0f;
        CCSFieldValue[5] = 64.0f;
        CCSFieldValue[6] = 96.0f;
        CCSFieldValue[7] = 128.0f;
        CCSFieldValue[8] = 64.0f;
        CCSFieldValue[9] = 96.0f;
        CCSFieldValue[10] = 128.0f;
        CCSFieldValue[11] = 128.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 128.0f;
        CCSFieldValue[14] = 128.0f;
        CCSFieldValue[15] = 128.0f;

        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCellCenteredScalarField CCSField = new CCellCenteredScalarField(Res, Origin, Spacing, CCSFieldValue);

        ComputeBuffer ParticlesScalarDataResult = new ComputeBuffer(2, sizeof(float));

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.LINEAR
        );

        float[] ParticlesScalarValueResult = new float[ParticlesScalarDataResult.count];
        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region LinearTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCSFieldValue[0] = 64.0f;
        CCSFieldValue[1] = 64.0f;
        CCSFieldValue[2] = 64.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 64.0f;
        CCSFieldValue[5] = 64.0f;
        CCSFieldValue[6] = 96.0f;
        CCSFieldValue[7] = 128.0f;
        CCSFieldValue[8] = 64.0f;
        CCSFieldValue[9] = 96.0f;
        CCSFieldValue[10] = 128.0f;
        CCSFieldValue[11] = 128.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 128.0f;
        CCSFieldValue[14] = 128.0f;
        CCSFieldValue[15] = 128.0f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCSField.resize(Res, Origin, Spacing, CCSFieldValue);

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        CCSFieldValue[0] = 64.0f;
        CCSFieldValue[1] = 64.0f;
        CCSFieldValue[2] = 64.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 64.0f;
        CCSFieldValue[5] = 64.0f;
        CCSFieldValue[6] = 96.0f;
        CCSFieldValue[7] = 128.0f;
        CCSFieldValue[8] = 64.0f;
        CCSFieldValue[9] = 96.0f;
        CCSFieldValue[10] = 128.0f;
        CCSFieldValue[11] = 128.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 128.0f;
        CCSFieldValue[14] = 128.0f;
        CCSFieldValue[15] = 128.0f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCSField.resize(Res, Origin, Spacing, CCSFieldValue);

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.QUADRATIC
        );

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 71.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 121.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -15.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCSFieldValue[0] = 64.0f;
        CCSFieldValue[1] = 64.0f;
        CCSFieldValue[2] = 64.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 64.0f;
        CCSFieldValue[5] = 64.0f;
        CCSFieldValue[6] = 96.0f;
        CCSFieldValue[7] = 128.0f;
        CCSFieldValue[8] = 64.0f;
        CCSFieldValue[9] = 96.0f;
        CCSFieldValue[10] = 128.0f;
        CCSFieldValue[11] = 128.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 128.0f;
        CCSFieldValue[14] = 128.0f;
        CCSFieldValue[15] = 128.0f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCSField.resize(Res, Origin, Spacing, CCSFieldValue);

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 71.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 121.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        CCSFieldValue[0] = 36.0f;
        CCSFieldValue[1] = 36.0f;
        CCSFieldValue[2] = 36.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 36.0f;
        CCSFieldValue[5] = 36.0f;
        CCSFieldValue[6] = 54.0f;
        CCSFieldValue[7] = 72.0f;
        CCSFieldValue[8] = 36.0f;
        CCSFieldValue[9] = 54.0f;
        CCSFieldValue[10] = 72.0f;
        CCSFieldValue[11] = 72.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 72.0f;
        CCSFieldValue[14] = 72.0f;
        CCSFieldValue[15] = 72.0f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCSField.resize(Res, Origin, Spacing, CCSFieldValue);

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.CUBIC
        );

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 41.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 67.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -15.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCSFieldValue[0] = 36.0f;
        CCSFieldValue[1] = 36.0f;
        CCSFieldValue[2] = 36.0f;
        CCSFieldValue[3] = 0.0f;
        CCSFieldValue[4] = 36.0f;
        CCSFieldValue[5] = 36.0f;
        CCSFieldValue[6] = 54.0f;
        CCSFieldValue[7] = 72.0f;
        CCSFieldValue[8] = 36.0f;
        CCSFieldValue[9] = 54.0f;
        CCSFieldValue[10] = 72.0f;
        CCSFieldValue[11] = 72.0f;
        CCSFieldValue[12] = 0.0f;
        CCSFieldValue[13] = 72.0f;
        CCSFieldValue[14] = 72.0f;
        CCSFieldValue[15] = 72.0f;

        EulerParticles.free();
        EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCSField.resize(Res, Origin, Spacing, CCSFieldValue);

        CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesScalarDataResult,
            CCSField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesScalarDataResult.GetData(ParticlesScalarValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[0] - 41.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesScalarValueResult[1] - 67.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region free
        EulerParticles.free();
        CCSField.free();
        ParticlesScalarDataResult.Release();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void tranferCCVField2ParticlesTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        float[] CCVFieldValueX = new float[Res.x * Res.y * Res.z];
        float[] CCVFieldValueY = new float[Res.x * Res.y * Res.z];
        float[] CCVFieldValueZ = new float[Res.x * Res.y * Res.z];
        CCVFieldValueX[0] = 64.0f;
        CCVFieldValueX[1] = 64.0f;
        CCVFieldValueX[2] = 64.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 64.0f;
        CCVFieldValueX[5] = 64.0f;
        CCVFieldValueX[6] = 96.0f;
        CCVFieldValueX[7] = 128.0f;
        CCVFieldValueX[8] = 64.0f;
        CCVFieldValueX[9] = 96.0f;
        CCVFieldValueX[10] = 128.0f;
        CCVFieldValueX[11] = 128.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 128.0f;
        CCVFieldValueX[14] = 128.0f;
        CCVFieldValueX[15] = 128.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CCellCenteredVectorField CCVField = new CCellCenteredVectorField(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        ComputeBuffer ParticlesVectorDataResult = new ComputeBuffer(3 * 2, sizeof(float));

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.LINEAR
        );

        float[] ParticlesVectorValueResult = new float[ParticlesVectorDataResult.count];
        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 64.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 192.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 256.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 384.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region LinearTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 60.0f;
        ParticlesPosValue[2] = -10.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCVFieldValueX[0] = 64.0f;
        CCVFieldValueX[1] = 64.0f;
        CCVFieldValueX[2] = 64.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 64.0f;
        CCVFieldValueX[5] = 64.0f;
        CCVFieldValueX[6] = 96.0f;
        CCVFieldValueX[7] = 128.0f;
        CCVFieldValueX[8] = 64.0f;
        CCVFieldValueX[9] = 96.0f;
        CCVFieldValueX[10] = 128.0f;
        CCVFieldValueX[11] = 128.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 128.0f;
        CCVFieldValueX[14] = 128.0f;
        CCVFieldValueX[15] = 128.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        CCVField.resize(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 96.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 192.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 288.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 128.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 256.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 384.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        CCVFieldValueX[0] = 64.0f;
        CCVFieldValueX[1] = 64.0f;
        CCVFieldValueX[2] = 64.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 64.0f;
        CCVFieldValueX[5] = 64.0f;
        CCVFieldValueX[6] = 96.0f;
        CCVFieldValueX[7] = 128.0f;
        CCVFieldValueX[8] = 64.0f;
        CCVFieldValueX[9] = 96.0f;
        CCVFieldValueX[10] = 128.0f;
        CCVFieldValueX[11] = 128.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 128.0f;
        CCVFieldValueX[14] = 128.0f;
        CCVFieldValueX[15] = 128.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        CCVField.resize(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 71.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 142.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 213.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 121.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 242.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 363.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -15.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCVFieldValueX[0] = 64.0f;
        CCVFieldValueX[1] = 64.0f;
        CCVFieldValueX[2] = 64.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 64.0f;
        CCVFieldValueX[5] = 64.0f;
        CCVFieldValueX[6] = 96.0f;
        CCVFieldValueX[7] = 128.0f;
        CCVFieldValueX[8] = 64.0f;
        CCVFieldValueX[9] = 96.0f;
        CCVFieldValueX[10] = 128.0f;
        CCVFieldValueX[11] = 128.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 128.0f;
        CCVFieldValueX[14] = 128.0f;
        CCVFieldValueX[15] = 128.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        CCVField.resize(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 71.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 142.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 213.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 121.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 242.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 363.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;

        CCVFieldValueX[0] = 36.0f;
        CCVFieldValueX[1] = 36.0f;
        CCVFieldValueX[2] = 36.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 36.0f;
        CCVFieldValueX[5] = 36.0f;
        CCVFieldValueX[6] = 54.0f;
        CCVFieldValueX[7] = 72.0f;
        CCVFieldValueX[8] = 36.0f;
        CCVFieldValueX[9] = 54.0f;
        CCVFieldValueX[10] = 72.0f;
        CCVFieldValueX[11] = 72.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 72.0f;
        CCVFieldValueX[14] = 72.0f;
        CCVFieldValueX[15] = 72.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        CCVField.resize(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.CUBIC
        );

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 41.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 82.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 123.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 67.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 134.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 201.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(1, 4, 4);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -15.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -5.0f;

        CCVFieldValueX[0] = 36.0f;
        CCVFieldValueX[1] = 36.0f;
        CCVFieldValueX[2] = 36.0f;
        CCVFieldValueX[3] = 0.0f;
        CCVFieldValueX[4] = 36.0f;
        CCVFieldValueX[5] = 36.0f;
        CCVFieldValueX[6] = 54.0f;
        CCVFieldValueX[7] = 72.0f;
        CCVFieldValueX[8] = 36.0f;
        CCVFieldValueX[9] = 54.0f;
        CCVFieldValueX[10] = 72.0f;
        CCVFieldValueX[11] = 72.0f;
        CCVFieldValueX[12] = 0.0f;
        CCVFieldValueX[13] = 72.0f;
        CCVFieldValueX[14] = 72.0f;
        CCVFieldValueX[15] = 72.0f;
        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            CCVFieldValueY[i] = 2 * CCVFieldValueX[i];
            CCVFieldValueZ[i] = 3 * CCVFieldValueX[i];
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        CCVField.resize(Res, Origin, Spacing, CCVFieldValueX, CCVFieldValueY, CCVFieldValueZ);

        CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            CCVField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[0] - 41.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[1] - 82.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[2] - 123.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3] - 67.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[4] - 134.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[5] - 201.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region free
        EulerParticles.free();
        CCVField.free();
        ParticlesVectorDataResult.Release();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void tranferFCVField2ParticlesTest()
    {
        CGlobalMacroAndFunc.init();

        #region LinearTest1
        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3Int ResX = Res + new Vector3Int(1, 0, 0);
        Vector3Int ResY = Res + new Vector3Int(0, 1, 0);
        Vector3Int ResZ = Res + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Vector3 Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        float[] ParticlesPosValue = new float[6];
        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        float[] FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        float[] FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        float[] FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        CEulerParticles EulerParticles = new CEulerParticles(2, 2, ParticlesPosValue);

        CFaceCenteredVectorField FCVField = new CFaceCenteredVectorField(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        ComputeBuffer ParticlesVectorDataResult = new ComputeBuffer(3 * 2, sizeof(float));

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.LINEAR
        );

        float[] ParticlesVectorValueResult = new float[EulerParticles.getParticlesVel().count];
        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0] - 1.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region LinearTest2
        Res = new Vector3Int(4, 4, 1);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 1] - 2.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 1] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region LinearTest3
        Res = new Vector3Int(1, 4, 4);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -15.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.LINEAR
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 2] - 3.5f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 2] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest1
        Res = new Vector3Int(4, 4, 1);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 30.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest2
        Res = new Vector3Int(4, 4, 1);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 40.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 1] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 1] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region QuadraticTest3
        Res = new Vector3Int(1, 4, 4);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -20.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.QUADRATIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 2] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 2] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest1
        Res = new Vector3Int(4, 4, 1);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 30.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 60.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -26.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0] - 1.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest2
        Res = new Vector3Int(4, 4, 1);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 45.0f;
        ParticlesPosValue[1] = 40.0f;
        ParticlesPosValue[2] = -24.0f;
        ParticlesPosValue[3] = 75.0f;
        ParticlesPosValue[4] = 60.0f;
        ParticlesPosValue[5] = -26.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 1] - 2.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 1] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region CubicTest3
        Res = new Vector3Int(1, 4, 4);
        ResX = Res + new Vector3Int(1, 0, 0);
        ResY = Res + new Vector3Int(0, 1, 0);
        ResZ = Res + new Vector3Int(0, 0, 1);
        Origin = new Vector3(0.0f, 20.0f, -30.0f);
        Spacing = new Vector3(30.0f, 20.0f, 10.0f);

        ParticlesPosValue[0] = 15.0f;
        ParticlesPosValue[1] = 50.0f;
        ParticlesPosValue[2] = -20.0f;
        ParticlesPosValue[3] = 15.0f;
        ParticlesPosValue[4] = 70.0f;
        ParticlesPosValue[5] = -10.0f;
        FCVFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        FCVFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        FCVFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];
        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    FCVFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    FCVFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = y + 1;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    FCVFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = z + 2;
                }
            }
        }

        EulerParticles.resize(2, 2, ParticlesPosValue);

        FCVField.resize(Res, Origin, Spacing, FCVFieldValueX, FCVFieldValueY, FCVFieldValueZ);

        CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker
        (
            EulerParticles.getParticlesPos(),
            ParticlesVectorDataResult,
            FCVField,
            EPGTransferAlgorithm.CUBIC
        );

        ParticlesVectorDataResult.GetData(ParticlesVectorValueResult);

        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 0 + 2] - 3.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        Assert.IsTrue(Mathf.Abs(ParticlesVectorValueResult[3 * 1 + 2] - 4.0f) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
        #endregion

        #region free
        EulerParticles.free();
        FCVField.free();
        ParticlesVectorDataResult.Release();
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void advectParticlesInVelFieldTest()
    {
        CGlobalMacroAndFunc.init();

        #region RK1_NoSubSteps

        Vector3Int Res = new Vector3Int(4, 4, 1);
        Vector3Int ResX = Res + new Vector3Int(1, 0, 0);
        Vector3Int ResY = Res + new Vector3Int(0, 1, 0);
        Vector3Int ResZ = Res + new Vector3Int(0, 0, 1);
        Vector3 Origin = new Vector3(-10.0f, -20.0f, -30.0f);
        Vector3 Spacing = new Vector3(10.0f, 20.0f, 30.0f);

        Vector3Int ParticlesRes = new Vector3Int(Res.x, Res.y, Res.z);
        int NumOfparticles = ParticlesRes.x * ParticlesRes.y * ParticlesRes.z;

        Vector3 BasePos = new Vector3((Origin.x + 1.25f * Spacing.x), (Origin.y + 1.25f * Spacing.y), -15.0f);
        float[] ParticlesPosValue = new float[3 * NumOfparticles];

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] = BasePos.x + x * 0.5f * Spacing.x;
                    ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] = BasePos.y + y * 0.5f * Spacing.y;
                    ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] = BasePos.z;
                }
            }
        }

        float[] VelFieldValueX = new float[ResX.x * ResX.y * ResX.z];
        float[] VelFieldValueY = new float[ResY.x * ResY.y * ResY.z];
        float[] VelFieldValueZ = new float[ResZ.x * ResZ.y * ResZ.z];

        for (int z = 0; z < ResX.z; z++)
        {
            for (int y = 0; y < ResX.y; y++)
            {
                for (int x = 0; x < ResX.x; x++)
                {
                    VelFieldValueX[z * ResX.x * ResX.y + y * ResX.x + x] = 10 * x;
                }
            }
        }
        for (int z = 0; z < ResY.z; z++)
        {
            for (int y = 0; y < ResY.y; y++)
            {
                for (int x = 0; x < ResY.x; x++)
                {
                    VelFieldValueY[z * ResY.x * ResY.y + y * ResY.x + x] = 20 * y;
                }
            }
        }
        for (int z = 0; z < ResZ.z; z++)
        {
            for (int y = 0; y < ResZ.y; y++)
            {
                for (int x = 0; x < ResZ.x; x++)
                {
                    VelFieldValueZ[z * ResZ.x * ResZ.y + y * ResZ.x + x] = 30 * z;
                }
            }
        }

        CEulerParticles EulerParticles = new CEulerParticles(NumOfparticles, NumOfparticles, ParticlesPosValue);
        CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        float DeltaT = 0.1f;
        float CFLNumber = 1.0f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK1, ESamplingAlgorithm.LINEAR);

        ComputeBuffer ParticlesPosDataResult = EulerParticles.getParticlesPos();
        float[] ParticlesPosValueResult = new float[ParticlesPosDataResult.count];
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;
                    RelOriPos = div(RelOriPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + DeltaT * 10.0f * RelOriPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + DeltaT * 20.0f * RelOriPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + DeltaT * 30.0f * RelOriPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK1_NoSubStepsImproved

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.1f;
        CFLNumber = 1.0f;
        CEulerParticles EulerParticlesImproved = new CEulerParticles(NumOfparticles, NumOfparticles, ParticlesPosValue);
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK1);
        EulerParticlesImproved.advectParticlesInVelFieldImproved(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK1);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);
        ComputeBuffer ParticlesPosDataResultImproved = EulerParticlesImproved.getParticlesPos();
        float[] ParticlesPosValueResultImproved = new float[ParticlesPosDataResultImproved.count];
        ParticlesPosDataResultImproved.GetData(ParticlesPosValueResultImproved);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;
                    RelOriPos = div(RelOriPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + DeltaT * 10.0f * RelOriPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + DeltaT * 20.0f * RelOriPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + DeltaT * 30.0f * RelOriPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK1_3SubSteps

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.4f;
        CFLNumber = 1.1f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK1, ESamplingAlgorithm.LINEAR);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;

                    Vector3 SecondRelPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    Vector3 ThirdRelPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    RelOriPos = div(RelOriPos, Spacing);
                    SecondRelPos += mul(0.2f * new Vector3(10, 20, 30), RelOriPos);
                    SecondRelPos = div(SecondRelPos, Spacing);
                    ThirdRelPos += mul(0.2f * new Vector3(10, 20, 30), RelOriPos);
                    ThirdRelPos += mul(0.1f * new Vector3(10, 20, 30), SecondRelPos);
                    ThirdRelPos = div(ThirdRelPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + 0.2f * 10 * RelOriPos.x + 0.1f * 10 * SecondRelPos.x + 0.1f * 10 * ThirdRelPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + 0.2f * 20 * RelOriPos.y + 0.1f * 20 * SecondRelPos.y + 0.1f * 20 * ThirdRelPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + 0.2f * 30 * RelOriPos.z + 0.1f * 30 * SecondRelPos.z + 0.1f * 30 * ThirdRelPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK2_NoSubSteps

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.1f;
        CFLNumber = 1.0f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK2, ESamplingAlgorithm.LINEAR);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;

                    Vector3 RelMidPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    RelOriPos = div(RelOriPos, Spacing);

                    RelMidPos += mul(0.5f * DeltaT * new Vector3(10, 20, 30), RelOriPos);
                    RelMidPos = div(RelMidPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + DeltaT * 10 * RelMidPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + DeltaT * 20 * RelMidPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + DeltaT * 30 * RelMidPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK2_NoSubStepsImproved

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        EulerParticlesImproved.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.1f;
        CFLNumber = 1.0f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK2, ESamplingAlgorithm.LINEAR);
        EulerParticlesImproved.advectParticlesInVelFieldImproved(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK2);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);
        ParticlesPosDataResultImproved = EulerParticlesImproved.getParticlesPos();
        ParticlesPosDataResultImproved.GetData(ParticlesPosValueResultImproved);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;

                    Vector3 RelMidPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    RelOriPos = div(RelOriPos, Spacing);

                    RelMidPos += mul(0.5f * DeltaT * new Vector3(10, 20, 30), RelOriPos);
                    RelMidPos = div(RelMidPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + DeltaT * 10 * RelMidPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + DeltaT * 20 * RelMidPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + DeltaT * 30 * RelMidPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK3_NoSubSteps

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.1f;
        CFLNumber = 1.0f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK3, ESamplingAlgorithm.LINEAR);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;

                    Vector3 RelMidPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    Vector3 RelThreeFourthsPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    RelOriPos = div(RelOriPos, Spacing);

                    RelMidPos += mul(0.5f * DeltaT * new Vector3(10, 20, 30), RelOriPos);
                    RelMidPos = div(RelMidPos, Spacing);

                    RelThreeFourthsPos += mul(0.75f * DeltaT * new Vector3(10, 20, 30), RelMidPos);
                    RelThreeFourthsPos = div(RelThreeFourthsPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + 2.0f / 9.0f * DeltaT * 10 * RelOriPos.x + 3.0f / 9.0f * DeltaT * 10 * RelMidPos.x + 4.0f / 9.0f * DeltaT * 10 * RelThreeFourthsPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + 2.0f / 9.0f * DeltaT * 20 * RelOriPos.y + 3.0f / 9.0f * DeltaT * 20 * RelMidPos.y + 4.0f / 9.0f * DeltaT * 20 * RelThreeFourthsPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + 2.0f / 9.0f * DeltaT * 30 * RelOriPos.z + 3.0f / 9.0f * DeltaT * 30 * RelMidPos.z + 4.0f / 9.0f * DeltaT * 30 * RelThreeFourthsPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        #region RK3_NoSubStepsImproved

        EulerParticles.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        EulerParticlesImproved.resize(NumOfparticles, NumOfparticles, ParticlesPosValue);
        FCVVelField.resize(Res, Origin, Spacing, VelFieldValueX, VelFieldValueY, VelFieldValueZ);

        DeltaT = 0.1f;
        CFLNumber = 1.0f;
        EulerParticles.advectParticlesInVelField(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK3, ESamplingAlgorithm.LINEAR);
        EulerParticlesImproved.advectParticlesInVelFieldImproved(FCVVelField, DeltaT, CFLNumber, EAdvectionAccuracy.RK3);

        ParticlesPosDataResult = EulerParticles.getParticlesPos();
        ParticlesPosDataResult.GetData(ParticlesPosValueResult);
        ParticlesPosDataResultImproved = EulerParticlesImproved.getParticlesPos();
        ParticlesPosDataResultImproved.GetData(ParticlesPosValueResultImproved);

        for (int z = 0; z < ParticlesRes.z; z++)
        {
            for (int y = 0; y < ParticlesRes.y; y++)
            {
                for (int x = 0; x < ParticlesRes.x; x++)
                {
                    Vector3 RelOriPos = new Vector3(
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1],
                        ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2]) - Origin;

                    Vector3 RelMidPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    Vector3 RelThreeFourthsPos = new Vector3(RelOriPos.x, RelOriPos.y, RelOriPos.z);
                    RelOriPos = div(RelOriPos, Spacing);

                    RelMidPos += mul(0.5f * DeltaT * new Vector3(10, 20, 30), RelOriPos);
                    RelMidPos = div(RelMidPos, Spacing);

                    RelThreeFourthsPos += mul(0.75f * DeltaT * new Vector3(10, 20, 30), RelMidPos);
                    RelThreeFourthsPos = div(RelThreeFourthsPos, Spacing);

                    float ResultPosX = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] + 2.0f / 9.0f * DeltaT * 10 * RelOriPos.x + 3.0f / 9.0f * DeltaT * 10 * RelMidPos.x + 4.0f / 9.0f * DeltaT * 10 * RelThreeFourthsPos.x;
                    float ResultPosY = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] + 2.0f / 9.0f * DeltaT * 20 * RelOriPos.y + 3.0f / 9.0f * DeltaT * 20 * RelMidPos.y + 4.0f / 9.0f * DeltaT * 20 * RelThreeFourthsPos.y;
                    float ResultPosZ = ParticlesPosValue[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] + 2.0f / 9.0f * DeltaT * 30 * RelOriPos.z + 3.0f / 9.0f * DeltaT * 30 * RelMidPos.z + 4.0f / 9.0f * DeltaT * 30 * RelThreeFourthsPos.z;

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResult[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);

                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x)] - ResultPosX) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 1] - ResultPosY) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                    Assert.IsTrue(Mathf.Abs(ParticlesPosValueResultImproved[3 * (z * ParticlesRes.x * ParticlesRes.y + y * ParticlesRes.x + x) + 2] - ResultPosZ) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
                }
            }
        }
        #endregion

        CGlobalMacroAndFunc.free();
    }

    [Test]
    public void statisticalFluidDensityTest()
    {
        CGlobalMacroAndFunc.init();

        Vector3Int Res = new Vector3Int(4, 4, 4);
        Vector3 Origin = new Vector3(-10.0f, -20.0f, -30.0f);
        Vector3 Spacing = new Vector3(10.0f, 20.0f, 30.0f);
        int NumOfPerGrid = 8;
        float[] FluidSDFFieldValue = new float[Res.x * Res.y * Res.z];
        float[] SolidSDFFieldValue = new float[Res.x * Res.y * Res.z];

        for (int i = 0; i < FluidSDFFieldValue.Length; i++) FluidSDFFieldValue[i] = 1.0f;
        for (int i = 0; i < SolidSDFFieldValue.Length; i++) SolidSDFFieldValue[i] = -1.0f;

        FluidSDFFieldValue[21] = -1.0f; SolidSDFFieldValue[21] = 1.0f;
        FluidSDFFieldValue[22] = -1.0f; SolidSDFFieldValue[22] = 1.0f;
        FluidSDFFieldValue[25] = -1.0f; SolidSDFFieldValue[25] = 1.0f;
        FluidSDFFieldValue[26] = -1.0f; SolidSDFFieldValue[26] = 1.0f;
        FluidSDFFieldValue[37] = -1.0f; SolidSDFFieldValue[37] = 1.0f;
        FluidSDFFieldValue[38] = -1.0f; SolidSDFFieldValue[38] = 1.0f;
        FluidSDFFieldValue[41] = -1.0f; SolidSDFFieldValue[41] = 1.0f;
        FluidSDFFieldValue[42] = -1.0f; SolidSDFFieldValue[42] = 1.0f;

        Vector3[] FluidGridIndex = new Vector3[8]
        {
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(2.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 2.0f, 1.0f),
            new Vector3(2.0f, 2.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 2.0f),
            new Vector3(2.0f, 1.0f, 2.0f),
            new Vector3(1.0f, 2.0f, 2.0f),
            new Vector3(2.0f, 2.0f, 2.0f)
        };

        CCellCenteredScalarField CCSFluidSDFField = new CCellCenteredScalarField(Res, Origin, Spacing, FluidSDFFieldValue);
        CCellCenteredScalarField CCSSolidSDFField = new CCellCenteredScalarField(Res, Origin, Spacing, SolidSDFFieldValue);

        CEulerParticles EulerParticles = new CEulerParticles(4 * 4 * 4 * 64);
        EulerParticles.addParticles(CCSFluidSDFField, CCSSolidSDFField, NumOfPerGrid);

        CCellCenteredScalarField CCSFluidDensityField = new CCellCenteredScalarField(Res, Origin, Spacing);
        EulerParticles.statisticalFluidDensity(CCSFluidDensityField);
        ComputeBuffer FluidDensityDataResult = CCSFluidDensityField.getGridData();
        int[] FluidDensityValueResult = new int[FluidDensityDataResult.count];
        FluidDensityDataResult.GetData(FluidDensityValueResult);

        for (int i = 0; i < Res.x * Res.y * Res.z; i++)
        {
            if (i == 21 || i == 22 || i == 25 || i == 26 || i == 37 || i == 38 || i == 41 || i == 42)
            {
                Assert.IsTrue(Mathf.Abs(FluidDensityValueResult[i] - 8.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
            else
            {
                Assert.IsTrue(Mathf.Abs(FluidDensityValueResult[i] - 0.0f) <= CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
            }
        }

        CGlobalMacroAndFunc.free();
    }

    Vector3 mul(Vector3 vVector1, Vector3 vVector2)
    {
        return new Vector3(vVector1.x * vVector2.x, vVector1.y * vVector2.y, vVector1.z * vVector2.z);
    }

    Vector3 div(Vector3 vVector1, Vector3 vVector2)
    {
        return new Vector3(vVector1.x / vVector2.x, vVector1.y / vVector2.y, vVector1.z / vVector2.z);
    }
}