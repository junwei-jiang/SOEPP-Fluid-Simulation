using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class SemiLagrangianTest
{
    [Test]
    //BackTrace测试(速度场恒定)
    public void SemiLagrangian_BackTrace1()
    {
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcPosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcPosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcPosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcPosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 1;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 2;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 3;
				}
			}
		}

		CCellCenteredVectorField CCVSrcPosField = new CCellCenteredVectorField (Resolution, Origin, Spacing, SrcPosVectorFieldValueX, SrcPosVectorFieldValueY, SrcPosVectorFieldValueZ);
		CCellCenteredVectorField CCVDstPosField = new CCellCenteredVectorField(CCVSrcPosField);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField (Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.backTrace(CCVSrcPosField, FCVVelField, 5, CCVDstPosField);

		float[] DstPosVectorFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstPosField.getGridDataX().GetData(DstPosVectorFieldValueResultX);
		CCVDstPosField.getGridDataY().GetData(DstPosVectorFieldValueResultY);
		CCVDstPosField.getGridDataZ().GetData(DstPosVectorFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - x * 10) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - y * 20) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - z * 30) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//BackTrace测试(速度场为0)
	public void SemiLagrangian_BackTrace2()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcPosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcPosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcPosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcPosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = 0;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = 0;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = 0;
				}
			}
		}

		CCellCenteredVectorField CCVSrcPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcPosVectorFieldValueX, SrcPosVectorFieldValueY, SrcPosVectorFieldValueZ);
		CCellCenteredVectorField CCVDstPosField = new CCellCenteredVectorField(CCVSrcPosField);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.backTrace(CCVSrcPosField, FCVVelField, 5, CCVDstPosField);

		float[] DstPosVectorFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstPosField.getGridDataX().GetData(DstPosVectorFieldValueResultX);
		CCVDstPosField.getGridDataY().GetData(DstPosVectorFieldValueResultY);
		CCVDstPosField.getGridDataZ().GetData(DstPosVectorFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - x * 10 - 5) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - y * 20 - 10) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - z * 30 - 15) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//BackTrace测试(RK1)
	public void SemiLagrangian_BackTrace3()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcPosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcPosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcPosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcPosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredVectorField CCVSrcPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcPosVectorFieldValueX, SrcPosVectorFieldValueY, SrcPosVectorFieldValueZ);
		CCellCenteredVectorField CCVDstPosField = new CCellCenteredVectorField(CCVSrcPosField);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.backTrace(CCVSrcPosField, FCVVelField, 5, CCVDstPosField, EAdvectionAccuracy.RK1);

		float[] DstPosVectorFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstPosField.getGridDataX().GetData(DstPosVectorFieldValueResultX);
		CCVDstPosField.getGridDataY().GetData(DstPosVectorFieldValueResultY);
		CCVDstPosField.getGridDataZ().GetData(DstPosVectorFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (x * 10 + 5 - (x + 0.5f) * 5)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (y * 20 + 10 - (y + 0.5f) * 5)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (z * 30 + 15 - (z + 0.5f) * 5)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//BackTrace测试(RK2)
	public void SemiLagrangian_BackTrace4()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcPosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcPosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcPosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcPosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredVectorField CCVSrcPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcPosVectorFieldValueX, SrcPosVectorFieldValueY, SrcPosVectorFieldValueZ);
		CCellCenteredVectorField CCVDstPosField = new CCellCenteredVectorField(CCVSrcPosField);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.backTrace(CCVSrcPosField, FCVVelField, 5, CCVDstPosField, EAdvectionAccuracy.RK2);

		float[] DstPosVectorFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstPosField.getGridDataX().GetData(DstPosVectorFieldValueResultX);
		CCVDstPosField.getGridDataY().GetData(DstPosVectorFieldValueResultY);
		CCVDstPosField.getGridDataZ().GetData(DstPosVectorFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (x * 10 + 5 - 5 * (x * 10 + 5 - (x + 0.5f) * 5 * 0.5f) / 10)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (y * 20 + 10 - 5 * (y * 20 + 10 - (y + 0.5f) * 5 * 0.5f) / 20)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - (z * 30 + 15 - 5 * (z * 30 + 15 - (z + 0.5f) * 5 * 0.5f) / 30)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//BackTrace测试(RK3)
	public void SemiLagrangian_BackTrace5()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcPosVectorFieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcPosVectorFieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcPosVectorFieldValueX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcPosVectorFieldValueY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcPosVectorFieldValueZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredVectorField CCVSrcPosField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcPosVectorFieldValueX, SrcPosVectorFieldValueY, SrcPosVectorFieldValueZ);
		CCellCenteredVectorField CCVDstPosField = new CCellCenteredVectorField(CCVSrcPosField);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.backTrace(CCVSrcPosField, FCVVelField, 5, CCVDstPosField, EAdvectionAccuracy.RK3);

		float[] DstPosVectorFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstPosVectorFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstPosField.getGridDataX().GetData(DstPosVectorFieldValueResultX);
		CCVDstPosField.getGridDataY().GetData(DstPosVectorFieldValueResultY);
		CCVDstPosField.getGridDataZ().GetData(DstPosVectorFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					float Vel1X = (x * 10 + 5) / 10.0f;
					float Vel2X = ((x * 10 + 5) - 0.5f * 5 * Vel1X) / 10.0f;
					float Vel3X = ((x * 10 + 5) - 0.75f * 5 * Vel2X) / 10.0f;
					float DstPosX = x * 10 + 5 - 5 * (2.0f / 9.0f * Vel1X + 3.0f / 9.0f * Vel2X + 4.0f / 9.0f * Vel3X);
					float Vel1Y = (y * 20 + 10) / 20.0f;
					float Vel2Y = ((y * 20 + 10) - 0.5f * 5 * Vel1Y) / 20.0f;
					float Vel3Y = ((y * 20 + 10) - 0.75f * 5 * Vel2Y) / 20.0f;
					float DstPosY = y * 20 + 10 - 5 * (2.0f / 9.0f * Vel1Y + 3.0f / 9.0f * Vel2Y + 4.0f / 9.0f * Vel3Y);
					float Vel1Z = (z * 30 + 15) / 30.0f;
					float Vel2Z = ((z * 30 + 15) - 0.5f * 5 * Vel1Z) / 30.0f;
					float Vel3Z = ((z * 30 + 15) - 0.75f * 5 * Vel2Z) / 30.0f;
					float DstPosZ = z * 30 + 15 - 5 * (2.0f / 9.0f * Vel1Z + 3.0f / 9.0f * Vel2Z + 4.0f / 9.0f * Vel3Z);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstPosX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstPosY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstPosVectorFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - DstPosZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//AdvectionCCS场
	public void SemiLagrangian_AdvectionSolver1()
	{
		Vector3Int Resolution = new Vector3Int(14, 1, 1);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcScalarFieldData = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcScalarFieldData[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 100 + 50;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField (Resolution, Origin, Spacing, SrcScalarFieldData);
		CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField (Resolution, Origin, Spacing);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.advect(CCSSrcField, FCVVelField, 5, CCSDstField);

		float[] DstFieldValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
		CCSDstField.getGridData().GetData(DstFieldValueResult);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					if (x == 0)
					{
						Assert.IsTrue(Mathf.Abs(DstFieldValueResult[z * Resolution.x * Resolution.y + y * Resolution.x + x] - SrcScalarFieldData[z * Resolution.x * Resolution.y + y * Resolution.x + x]) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
					else
					{
						Assert.IsTrue(Mathf.Abs(DstFieldValueResult[z * Resolution.x * Resolution.y + y * Resolution.x + x] - 10 * (x * 10 + 5 - 5 * (x * 10 + 5 - (x + 0.5f) * 5 * 0.5f) / 10.0f)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}
		}
	}

	[Test]
	//AdvectionCCS场(稍微复杂的情况)
	public void SemiLagrangian_AdvectionSolver2()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcScalarFieldData = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcScalarFieldData[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10.0f + 5.0f + y * 20.0f + 10.0f + z * 30.0f + 15.0f;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredScalarField CCSSrcField = new CCellCenteredScalarField(Resolution, Origin, Spacing, SrcScalarFieldData);
		CCellCenteredScalarField CCSDstField = new CCellCenteredScalarField(Resolution, Origin, Spacing);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.advect(CCSSrcField, FCVVelField, 5, CCSDstField);

		float[] DstFieldValueResult = new float[Resolution.x * Resolution.y * Resolution.z];
		CCSDstField.getGridData().GetData(DstFieldValueResult);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					float TempX = (x * 10.0f + 5.0f - 5.0f * (x * 10.0f + 5.0f - (x + 0.5f) * 5.0f * 0.5f) / 10.0f);
					float TempY = (y * 20.0f + 10.0f - 5.0f * (y * 20.0f + 10.0f - (y + 0.5f) * 5.0f * 0.5f) / 20.0f);
					float TempZ = (z * 30.0f + 15.0f - 5.0f * (z * 30.0f + 15.0f - (z + 0.5f) * 5.0f * 0.5f) / 30.0f);
                    if (x == 0)
                    {
                        TempX = 5.0f;
                    }
                    if (y == 0)
                    {
                        TempY = 10.0f;
                    }
                    if (z == 0)
                    {
                        TempZ = 15.0f;
                    }
					Assert.IsTrue(Mathf.Abs(DstFieldValueResult[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX - TempY - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//AdvectionCCV场(稍微复杂的情况)
	public void SemiLagrangian_AdvectionSolver3()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcScalarFieldDataX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcScalarFieldDataY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] SrcScalarFieldDataZ = new float[Resolution.x * Resolution.y * Resolution.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					SrcScalarFieldDataX[z * Resolution.x * Resolution.y + y * Resolution.x + x] = x * 10 + 5;
					SrcScalarFieldDataY[z * Resolution.x * Resolution.y + y * Resolution.x + x] = y * 20 + 10;
					SrcScalarFieldDataZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] = z * 30 + 15;
				}
			}
		}

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CCellCenteredVectorField CCVSrcField = new CCellCenteredVectorField(Resolution, Origin, Spacing, SrcScalarFieldDataX, SrcScalarFieldDataY, SrcScalarFieldDataZ);
		CCellCenteredVectorField CCVDstField = new CCellCenteredVectorField(Resolution, Origin, Spacing);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.advect(CCVSrcField, FCVVelField, 5, CCVDstField);

		float[] DstFieldValueResultX = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstFieldValueResultY = new float[Resolution.x * Resolution.y * Resolution.z];
		float[] DstFieldValueResultZ = new float[Resolution.x * Resolution.y * Resolution.z];
		CCVDstField.getGridDataX().GetData(DstFieldValueResultX);
		CCVDstField.getGridDataY().GetData(DstFieldValueResultY);
		CCVDstField.getGridDataZ().GetData(DstFieldValueResultZ);

		for (int z = 0; z < Resolution.z; z++)
		{
			for (int y = 0; y < Resolution.y; y++)
			{
				for (int x = 0; x < Resolution.x; x++)
				{
					float TempX = (x * 10 + 5 - 5 * (x * 10 + 5 - (x + 0.5f) * 5 * 0.5f) / 10);
					float TempY = (y * 20 + 10 - 5 * (y * 20 + 10 - (y + 0.5f) * 5 * 0.5f) / 20);
					float TempZ = (z * 30 + 15 - 5 * (z * 30 + 15 - (z + 0.5f) * 5 * 0.5f) / 30);
					if (x == 0)
					{
						TempX = 5;
					}
					if (y == 0)
					{
						TempY = 10;
					}
					if (z == 0)
					{
						TempZ = 15;
					}
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultX[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultY[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultZ[z * Resolution.x * Resolution.y + y * Resolution.x + x] - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}

	[Test]
	//AdvectionFCV场(稍微复杂的情况)
	public void SemiLagrangian_AdvectionSolver4()
	{
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] SrcScalarFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] SrcScalarFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] SrcScalarFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		float[] VelVectorFieldValueX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] VelVectorFieldValueY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] VelVectorFieldValueZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					SrcScalarFieldDataX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x * 10;
					VelVectorFieldValueX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					SrcScalarFieldDataY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y * 20;
					VelVectorFieldValueY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					SrcScalarFieldDataZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z * 30;
					VelVectorFieldValueZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CFaceCenteredVectorField FCVSrcField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, SrcScalarFieldDataX, SrcScalarFieldDataY, SrcScalarFieldDataZ);
		CFaceCenteredVectorField FCVDstField = new CFaceCenteredVectorField(Resolution, Origin, Spacing);
		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldValueX, VelVectorFieldValueY, VelVectorFieldValueZ);

		CSemiLagrangian SemiLagrangianSolver = new CSemiLagrangian(Resolution, Origin, Spacing);

		SemiLagrangianSolver.advect(FCVSrcField, FCVVelField, 5, FCVDstField);

		float[] DstFieldValueResultX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] DstFieldValueResultY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] DstFieldValueResultZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];
		FCVDstField.getGridDataX().GetData(DstFieldValueResultX);
		FCVDstField.getGridDataY().GetData(DstFieldValueResultY);
		FCVDstField.getGridDataZ().GetData(DstFieldValueResultZ);

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					float TempX = (x * 10.0f - 5.0f * (x * 10 - x * 5 * 0.5f) / 10.0f);
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] - TempX) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					float TempY = (y * 20 - 5 * (y * 20 - y * 5 * 0.5f) / 20);
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] - TempY) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					float TempZ = (z * 30 - 5 * (z * 30 - z * 5 * 0.5f) / 30);
					Assert.IsTrue(Mathf.Abs(DstFieldValueResultZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] - TempZ) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
				}
			}
		}
	}
}
