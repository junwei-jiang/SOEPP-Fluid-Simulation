import numpy as np
from PIL import Image

Res = [256, 256, 1]

def readData(voOriginalData, vFile):
    IndexX = 0
    IndexY = 0
    for line in vFile:
        if(IndexX == Res[0]):
            IndexX =0
            IndexY += 1
        if(IndexX == 0):
            voOriginalData.append([])
        LineData = line.split(" ")
        voOriginalData[IndexY].append([float(LineData[0]), float(LineData[1]), float(LineData[2])])
        IndexX += 1
        

for Index in range(290, 2001):
    TempData = []
    # InputFileName = "../ExperimentData/TaylorVortex/1FirstOrderPressure/MixPICAndFLIP/RK1_Linear/txt/Frame" + str(Index) + ".txt"
    # OutputFileName = "../ExperimentData/TaylorVortex/1FirstOrderPressure/MixPICAndFLIP/RK1_Linear/pic/Frame" + str(Index) + ".png"
    # InputFileName = "../ExperimentData/TaylorVortex/3Reflection/NormalTimeStep/MixPICAndFLIP/RK1_Linear/txt/Frame" + str(Index) + ".txt"
    # OutputFileName = "../ExperimentData/TaylorVortex/3Reflection/NormalTimeStep/MixPICAndFLIP/RK1_Linear/pic/Frame" + str(Index) + ".png"
    InputFileName = "../ExperimentData/Test/txt/Frame" + str(Index) + ".txt"
    OutputFileName = "../ExperimentData/Test/pic/Frame" + str(Index) + ".png"
    GridDataFile = open(InputFileName, 'r')
    readData(TempData, GridDataFile)

    map_data = np.array(TempData)
    map_data = np.asarray(map_data, np.uint8)
    pic = Image.fromarray(map_data)
    pic.save(OutputFileName)