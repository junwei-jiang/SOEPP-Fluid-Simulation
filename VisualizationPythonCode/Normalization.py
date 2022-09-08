import numpy as np
from PIL import Image
import imageio
import scipy.misc
import matplotlib.image as mpimg
import cv2

Res = [1280, 720, 0]

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
        

TempData = []
InputFileName = "../ExperimentData/DamBreak/0.05_FLIP99RK1_250/figures/Fluid.karma1.0060.exr"
OutputFileName = "../ExperimentData/DamBreak/0.05_FLIP99RK1_250/figures/Fluid.karma1.0060_1.exr"
CurImage = cv2.imread(InputFileName, cv2.IMREAD_UNCHANGED)
Nm = np.array(CurImage)

map_data = np.array(TempData)
map_data = np.asarray(map_data, np.uint8)
pic = Image.fromarray(map_data)
pic.save(OutputFileName)
