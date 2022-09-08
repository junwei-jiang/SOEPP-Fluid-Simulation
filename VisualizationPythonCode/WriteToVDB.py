# import os, os.path
# os.add_dll_directory("C:/Program Files/OpenVDB/bin")
# os.add_dll_directory("D:/vcpkg/installed/x64-windows/bin")
# import pyopenvdb as vdb

# Res = [150, 300, 300]

# # def readData(voOriginalData, vFile):
# #     voOriginalData.append([])
# #     for line in vFile:
# #         LineData = line.split(" ")
# #         voOriginalData[0].append(float(LineData[0]))
# def readData(voOriginalData, vFile):
#     voOriginalData.append([])
#     for line in vFile:
#         LineData = line.split(" ")
#         voOriginalData[0].append(float(LineData[0]))
#         voOriginalData[0].append(float(LineData[1]))

# for Index in range(75, 1005):
#     TempData = []
#     InputFileName = "../ExperimentData/Test/3D/txt/Frame" + str(Index) + ".txt"
#     OutputFileName = "../ExperimentData/Test/3D/vdb/Frame" + str(Index) + ".vdb"
#     GridDataFile = open(InputFileName, 'r')
#     readData(TempData, GridDataFile)

#     # map_data = np.array(TempData)
#     # map_data = np.asarray(map_data, np.uint8)
#     # pic = Image.fromarray(map_data)
#     # pic.save(OutputFileName)
    
#     Grid1 = vdb.FloatGrid()
#     Grid2 = vdb.FloatGrid()
#     GridAccessor1 = Grid1.getAccessor()
#     GridAccessor2 = Grid2.getAccessor()

#     for z in range(0, Res[2]):
#         for y in range (0, Res[1]):
#             for x in range (0, Res[0]):
#                 LinearIndex = z * Res[0] * Res[1] + y * Res[0] + x
#                 if TempData[0][LinearIndex] > 0.001:
#                     index = (x, y, z)
#                     GridAccessor1.setValueOn(index, TempData[0][2 * LinearIndex])
#                     GridAccessor2.setValueOn(index, TempData[0][2 * LinearIndex + 1])
    
#     Grid1.transform = vdb.createLinearTransform(voxelSize=0.0014)
#     Grid2.transform = vdb.createLinearTransform(voxelSize=0.0014)
#     Grid1.gridClass = vdb.GridClass.FOG_VOLUME
#     Grid2.gridClass = vdb.GridClass.FOG_VOLUME
#     Grid1.name = 'density1'
#     Grid2.name = 'density2'

#     vdb.write(OutputFileName, grids=[Grid1, Grid2])

import os, os.path
os.add_dll_directory("C:/Program Files/OpenVDB/bin")
os.add_dll_directory("D:/vcpkg/installed/x64-windows/bin")
import pyopenvdb as vdb

Res = [128, 512, 128]

def readData(voOriginalData, vFile):
    length = 0
    voOriginalData.append([])
    for line in vFile:
        length = length + 1
        LineData = line.split(" ")
        voOriginalData[0].append(float(LineData[0]))
        voOriginalData[0].append(float(LineData[1]))
        voOriginalData[0].append(float(LineData[2]))
    return length

for Index in range(0, 1005):
    TempData = []
    InputFileName = "../ExperimentData/Test/3D/txt/Frame" + str(Index) + ".txt"
    OutputFileName = "../ExperimentData/Test/3D/vdb/Frame" + str(Index) + ".vdb"
    GridDataFile = open(InputFileName, 'r')
    length = readData(TempData, GridDataFile)
    
    Grid1 = vdb.FloatGrid()
    Grid2 = vdb.FloatGrid()
    GridAccessor1 = Grid1.getAccessor()
    GridAccessor2 = Grid2.getAccessor()

    for i in range(0, length):
        x = int(TempData[0][3 * i] % Res[0])
        y = int((TempData[0][3 * i] % (Res[0] * Res[1])) / Res[0])
        z = int(TempData[0][3 * i] / (Res[0] * Res[1]))
        index = (x, y, z)
        GridAccessor1.setValueOn(index, float(TempData[0][3 * i + 1]))
        GridAccessor2.setValueOn(index, float(TempData[0][3 * i + 2]))
    
    Grid1.transform = vdb.createLinearTransform(voxelSize=0.0390625)
    Grid2.transform = vdb.createLinearTransform(voxelSize=0.0390625)
    Grid1.gridClass = vdb.GridClass.FOG_VOLUME
    Grid2.gridClass = vdb.GridClass.FOG_VOLUME
    Grid1.name = 'density1'
    Grid2.name = 'density2'

    vdb.write(OutputFileName, grids=[Grid1, Grid2])