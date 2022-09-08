from cProfile import label
from matplotlib import markers
import numpy as np
import matplotlib.pyplot as plt
from turtle import color

def readData(voOriginalData, vFile):
    Index = 0
    for line in vFile:
        voOriginalData.append([])
        LineData = line.split(" ")
        if Index > 1:
            voOriginalData[Index].append(float(LineData[0]) / voOriginalData[1][0])
        else:
            voOriginalData[Index].append(float(LineData[0]))
        Index += 1
    voOriginalData[0] = voOriginalData[1] = [1]


x1 = np.arange(0,7.525,0.025)
x2 = np.arange(0,7.55,0.05)

# x1 = np.arange(0,5.1,0.1)
# x2 = np.arange(0,5.2,0.2)

TempData_SF = []
TempData_MC = []
TempData_BDF2 = []
TempData_R = []
TempData_SOR = []
TempData_Our = []
InputFileName_SF = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.025_SF.txt"
InputFileName_MC = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.025_MC.txt"
InputFileName_BDF2 = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.025_BDF2.txt"
InputFileName_R = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.05_Reflection.txt"
InputFileName_SOR = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.05_SOReflection.txt"
InputFileName_Our = "../ExperimentData/TaylorVortex_256X256_0.025_0.79_10.0/Energy/KineticEnergy_0.025_Our.txt"

# InputFileName_SF = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.1_SF.txt"
# InputFileName_MC = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.1_MC.txt"
# InputFileName_BDF2 = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.1_BDF2.txt"
# InputFileName_R = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.2_Reflection.txt"
# InputFileName_SOR = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.2_SOReflection.txt"
# InputFileName_Our = "../ExperimentData/TaylorGreenVortex/T_X=1280/Energy/KineticEnergy_0.1_Our.txt"
GridDataFile_SF = open(InputFileName_SF, 'r')
GridDataFile_MC = open(InputFileName_MC, 'r')
GridDataFile_BDF2 = open(InputFileName_BDF2, 'r')
GridDataFile_R = open(InputFileName_R, 'r')
GridDataFile_SOR = open(InputFileName_SOR, 'r')
GridDataFile_Our = open(InputFileName_Our, 'r')
readData(TempData_SF, GridDataFile_SF)
readData(TempData_MC, GridDataFile_MC)
readData(TempData_BDF2, GridDataFile_BDF2)
readData(TempData_R, GridDataFile_R)
readData(TempData_SOR, GridDataFile_SOR)
readData(TempData_Our, GridDataFile_Our)

plt.figure(figsize=(16,8), dpi = 120)
plt.xlabel("Time(s)", fontsize = 30)
plt.ylabel("Energy", fontsize = 30)
plt.tick_params(labelsize = 20)
plt.plot(x1,TempData_SF, label = 'SF', color='#FF0000',linewidth=3.0)
plt.plot(x1,TempData_MC, label = 'MC', color='#FF7F00',linewidth=3.0)
plt.plot(x1,TempData_BDF2, label = 'MC+BDF2', color='#FFD700',linewidth=3.0)
plt.plot(x2,TempData_R, label = 'MC+R', color='#8A2BE2',linewidth=3.0, marker = "v", markevery=10, markersize = 16)
plt.plot(x2,TempData_SOR, label = 'MC+$\mathregular{R^2}$', color='#1E90FF',linewidth=3.0, marker = "^", markevery=12, markersize = 16)
plt.plot(x1,TempData_Our, label = 'MC+Ours', color='#3CB371',linewidth=3.0, marker = "*", markevery=16, markersize = 16)

legend  = plt.legend(fontsize=20)
# plt.show()
plt.savefig('Figure4.11_Taylor Vortex Energy.png', bbox_inches='tight')