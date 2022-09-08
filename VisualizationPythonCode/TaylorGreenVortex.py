import numpy as np
from PIL import Image
import matplotlib.pyplot as plt

Res = [256, 256, 1]

def readData(voOriginalDataX, voOriginalDataY, voOriginalData, vFile):
    IndexX = 0
    IndexY = 0
    for line in vFile:
        if(IndexX == Res[0]):
            IndexX =0
            IndexY += 1
        if(IndexX == 0):
            voOriginalDataX.append([])
            voOriginalDataY.append([])
            voOriginalData.append([])
        LineData = line.split(" ")
        voOriginalDataX[IndexY].append(float(LineData[0]))
        voOriginalDataY[IndexY].append(float(LineData[1]))
        voOriginalData[IndexY].append(float(LineData[2]))
        IndexX += 1

# def readData(voOriginalData, vFile):
#     IndexX = 0
#     IndexY = 0
#     for line in vFile:
#         if(IndexX == Res[0]):
#             IndexX =0
#             IndexY += 1
#         if(IndexX == 0):
#             voOriginalData.append([])
#         LineData = line.split(" ")
#         voOriginalData[IndexY].append(float(LineData[0]))
#         IndexX += 1

# x轴的刻度向内
plt.rcParams['xtick.direction'] = 'in' 
# y轴刻度向内
plt.rcParams['ytick.direction'] = 'in' 

# x轴和y轴的步长
step = 0.0245436921875

# x轴和y轴的变化范围
x = np.arange(0,2 * 3.1415926,step); y = np.arange(0,2 * 3.1415926,step) 

#对x,y网格化，得到X,Y
X,Y = np.meshgrid(x,y) 

TempDataX = []
TempDataY = []
TempData = []
InputFileName = "../ExperimentData/TaylorGreenVortex/T_X=1280/0.1_Our+R+NOA/txt/Frame" + str(10) + ".txt"
OutputFileName = "../ExperimentData/Test/pic/Frame" + str(10) + ".png"
GridDataFile = open(InputFileName, 'r')
readData(TempDataX, TempDataY, TempData, GridDataFile)

fig, ax = plt.subplots(figsize=(8,8),dpi=100)
plt.tick_params(labelsize=20)
# 画contour图
# CS = ax.contourf(TempDataX, TempDataY, TempData, 50, cmap = 'rainbow')
CS = ax.contourf(TempDataX, TempDataY, TempData, 500, cmap = 'rainbow', vmin=0, vmax=0.00135)
# CS = ax.contourf(TempDataX, TempDataY, TempData, 500, cmap = 'rainbow', vmin=0, vmax=0.0168)
# 画轮廓线
# CS = ax.contour(TempDataX, TempDataY, TempData, 50)
# CBar = plt.colorbar(CS)

plt.xlabel("x", fontsize = 30)
plt.ylabel("y", fontsize = 30)
plt.tick_params(labelsize = 20)
#设置x,y轴刻度
plt.xticks(np.arange(0,2 * 3.1415926))
plt.yticks(np.arange(0,2 * 3.1415926))
# 避免图片显示不完全
# plt.tight_layout()
# plt.show()
plt.savefig('0.1_Our+R+NOA.png', bbox_inches='tight')