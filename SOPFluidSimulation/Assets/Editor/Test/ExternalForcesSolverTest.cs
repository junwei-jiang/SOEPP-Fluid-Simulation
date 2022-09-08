using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EulerFluidEngine;

public class ExternalForcesSolverTest
{
    [Test]
    public void ExternalForcesTest()
    {
		Vector3Int Resolution = new Vector3Int(14, 15, 16);
		Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
		Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
		Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
		Vector3 Origin = new Vector3(0, 0, 0);
		Vector3 Spacing = new Vector3(10, 20, 30);

		float[] VelVectorFieldDataX = new float[ResolutionX.x* ResolutionX.y* ResolutionX.z];
		float[] VelVectorFieldDataY = new float[ResolutionY.x* ResolutionY.y* ResolutionY.z];
		float[] VelVectorFieldDataZ = new float[ResolutionZ.x* ResolutionZ.y* ResolutionZ.z];
		float[] DstVelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
		float[] DstVelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
		float[] DstVelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

		for (int z = 0; z < ResolutionX.z; z++)
		{
			for (int y = 0; y < ResolutionX.y; y++)
			{
				for (int x = 0; x < ResolutionX.x; x++)
				{
					VelVectorFieldDataX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] = x;
				}
			}
		}

		for (int z = 0; z < ResolutionY.z; z++)
		{
			for (int y = 0; y < ResolutionY.y; y++)
			{
				for (int x = 0; x < ResolutionY.x; x++)
				{
					VelVectorFieldDataY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] = y;
				}
			}
		}

		for (int z = 0; z < ResolutionZ.z; z++)
		{
			for (int y = 0; y < ResolutionZ.y; y++)
			{
				for (int x = 0; x < ResolutionZ.x; x++)
				{
					VelVectorFieldDataZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] = z;
				}
			}
		}

		CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(Resolution, Origin, Spacing, VelVectorFieldDataX, VelVectorFieldDataY, VelVectorFieldDataZ);

		CExternalForcesSolver ExternalForcesSolver = new CExternalForcesSolver();

		ExternalForcesSolver.addExternalForces(new Vector3(10, 0, 50));

		for (int n = 1; n < 10; n++)
		{
			ExternalForcesSolver.applyExternalForces(FCVVelField, 2);

			FCVVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
			FCVVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
			FCVVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

			for (int z = 0; z < ResolutionX.z; z++)
			{
				for (int y = 0; y < ResolutionX.y; y++)
				{
					for (int x = 0; x < ResolutionX.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] - (x + 10 * n * 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}

			for (int z = 0; z < ResolutionY.z; z++)
			{
				for (int y = 0; y < ResolutionY.y; y++)
				{
					for (int x = 0; x < ResolutionY.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] - (y - 9.8f * n * 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}

			for (int z = 0; z < ResolutionZ.z; z++)
			{
				for (int y = 0; y < ResolutionZ.y; y++)
				{
					for (int x = 0; x < ResolutionZ.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] - (z + 50 * n * 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}
		}

		ExternalForcesSolver.resizeExternalForcesSolver();
		FCVVelField.resize(Resolution, Origin, Spacing, VelVectorFieldDataX, VelVectorFieldDataY, VelVectorFieldDataZ);

		for (int n = 1; n < 10; n++)
		{
			ExternalForcesSolver.applyExternalForces(FCVVelField, 2);

			FCVVelField.getGridDataX().GetData(DstVelVectorFieldDataX);
			FCVVelField.getGridDataY().GetData(DstVelVectorFieldDataY);
			FCVVelField.getGridDataZ().GetData(DstVelVectorFieldDataZ);

			for (int z = 0; z < ResolutionX.z; z++)
			{
				for (int y = 0; y < ResolutionX.y; y++)
				{
					for (int x = 0; x < ResolutionX.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataX[z * ResolutionX.x * ResolutionX.y + y * ResolutionX.x + x] - x) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}

			for (int z = 0; z < ResolutionY.z; z++)
			{
				for (int y = 0; y < ResolutionY.y; y++)
				{
					for (int x = 0; x < ResolutionY.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataY[z * ResolutionY.x * ResolutionY.y + y * ResolutionY.x + x] - (y - 9.8f * n * 2)) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}

			for (int z = 0; z < ResolutionZ.z; z++)
			{
				for (int y = 0; y < ResolutionZ.y; y++)
				{
					for (int x = 0; x < ResolutionZ.x; x++)
					{
						Assert.IsTrue(Mathf.Abs(DstVelVectorFieldDataZ[z * ResolutionZ.x * ResolutionZ.y + y * ResolutionZ.x + x] - z) < CGlobalMacroAndFunc.GRID_SOLVER_EPSILON_FOR_TEST);
					}
				}
			}
		}
	}
}
