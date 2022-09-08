import numpy as np
import matplotlib.pyplot as plt

h = 1e-7

Resolution = []
Origin = []
Spacing = []

def readData(voOriginalDataCoordX, voOriginalDataCoordY, voOriginalDataValueX, voOriginalDataValueY, voResolution, voOrigin, voSpacing, vFile):
    for index, line in enumerate(vFile):
        LineData = line.split(" ")
        if(index == 0):
            voResolution.append(float(LineData[0]) + h)
            voResolution.append(float(LineData[1]) + h)
        elif(index == 1):
            voOrigin.append(float(LineData[0]) + h)
            voOrigin.append(float(LineData[1]) + h)
        elif(index == 2):
            voSpacing.append(float(LineData[0]) + h)
            voSpacing.append(float(LineData[1]) + h)
        else:
            voOriginalDataCoordX.append(float(LineData[0]) + h)
            voOriginalDataCoordY.append(float(LineData[1]) + h)
            voOriginalDataValueX.append(float(LineData[2]) + h)
            voOriginalDataValueY.append(float(LineData[3]) + h)

for Index in range(1, 1001):
    X = []
    Y = []
    U = []
    V = []
    InputFileName = "./VelVisualization/Vortex/SemiLagrangian/RK1_Linear/CurlFree/txt/Frame" + str(Index) + ".txt"
    OutputFileName = "./VelVisualization/Vortex/SemiLagrangian/RK1_Linear/CurlFree/pic/Frame" + str(Index) + ".png"
    # InputFileName = "./VelVisualization/TaylorVortex/Frame" + str(Index) + ".txt"
    # OutputFileName = "./VelVisualization/TaylorVortex/Frame" + str(Index) + ".png"
    GridDataFile = open(InputFileName, 'r')
    readData(X, Y, U, V, Resolution, Origin, Spacing, GridDataFile)

    plt.figure(figsize = (10, 10), dpi= 300)
    # plt.quiver(X,Y,U,V,angles="xy",color="#666666",pivot = 'mid')
    plt.quiver(X,Y,U,V,angles="xy", scale_units = 'xy', scale=0.5,color="#666666",pivot = 'mid')
    plt.xlim([Origin[0],Origin[0] + Resolution[0] * Spacing[0]])
    plt.ylim([Origin[1],Origin[1] + Resolution[1] * Spacing[1]])
    plt.savefig(OutputFileName)
    plt.close()