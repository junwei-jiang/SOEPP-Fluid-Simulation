from cProfile import label
from math import pi
from turtle import color
import numpy as np
import matplotlib.pyplot as plt
from shapely.geometry import LineString
from haishoku.haishoku import Haishoku

# image = 'Google.png'
# haishoku = Haishoku.loadHaishoku(image)
# Haishoku.showPalette(image)

X = [0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0]
# Y = [0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0]
StableFluids_Y = [0.007725723, 0.00766125, 0.007551739, 0.007335884, 0.006723595, 0.0102778, 0.01854455, 0.04349121, 0.08381414, 0.1739449]
MacCormack_Y = [0.0003226223, 0.0005208118, 0.0009470256, 0.001850798, 0.004578992, 0.009057283, 0.0177389, 0.04180335, 0.07608283, 0.1184957]
BDF2_Y = [0.0001606481, 0.0001126446, 0.0001037305, 0.0001019264, 0.0001036893, 0.0001648151, 0.000691057, 0.006853363, 0.038719, 0.1183011]
Reflection_Y = [0.0001506242, 0.0001093748, 0.000104195, 0.0001024576, 0.0001026503, 0.0001087864, 0.0001543505, 0.000659316, 0.002622607, 0.01110453]
SOReflection_Y = [0.0001504953, 0.000107068, 0.0001041368, 0.0001027176, 0.000103263, 0.0001112943, 0.0001649296, 0.0007381318, 0.002953575, 0.01207112]
OurRNOA_Y = [0.0001767621, 0.000108996, 0.0001027643, 0.0001019454, 0.0001072952, 0.0001422017, 0.0003675043, 0.002111118, 0.0067602, 0.01110453]
OurR_Y = [0.0001700558, 0.0001081306, 0.0001025945, 0.0001019839, 0.0001106603, 0.0001687775, 0.0004755339, 0.002286449, 0.006760182, 0.01110453]

# IsoValueX = 0.000
# IsoValueY = 0.001
# IsolineX = [IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX]
# IsolineY = [IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY, IsoValueY]

XDeltaT = [0.001, 0.01, 0.1]
YDeltaT = [0.001, 0.01, 0.1]
XDeltaThreeT = [0.01, 0.1]
YDeltaThreeT = [0.0001, 0.01]
XDeltaSecondT = [0.25, 2.5]
YDeltaSecondT = [0.0001, 0.01]

# first_line = LineString(np.column_stack((X, IsolineY)))
# second_line = LineString(np.column_stack((X, Reflection_Y)))
# intersection = first_line.intersection(second_line)

plt.figure(figsize=(12,12), dpi = 240)
plt.tick_params(labelsize=20)
plt.xlabel("Δt", fontsize = 30)
plt.ylabel("RMS Error in u", fontsize = 30)
plt.text(0.005, 0.015, 'O(Δt)', fontsize = 30)
plt.loglog(XDeltaT, YDeltaT, label='O(Δt)', color = 'black', ls='dotted')
plt.text(0.8, 0.0005, 'O(Δt²)', fontsize = 30)
plt.loglog(XDeltaSecondT, YDeltaSecondT, label='O(Δt2)', color = 'black', ls='dashed')
plt.loglog(X, StableFluids_Y, label='SF', marker = "s", markersize=12, color = '#FF0000')
plt.loglog(X, MacCormack_Y, label='MC', marker = "o", markersize=12, color = '#FF7F00')
plt.loglog(X, BDF2_Y, label='MC+BDF2', marker = "D", markersize=12, color = '#FFD700')
plt.loglog(X, Reflection_Y, label='MC+R', marker = "v", markersize=12, color = '#8A2BE2')
plt.loglog(X, SOReflection_Y, label='MC+$\mathregular{R^2}$', marker = "^", markersize=12, color = '#1E90FF')
plt.loglog(X, OurRNOA_Y, label='MC+Ours(do not advect pressure)', marker = "p", markersize=16, color = '#00CED1')
plt.loglog(X, OurR_Y, label='MC+Ours', marker = "*", markersize=14, color = '#3CB371')
# plt.loglog(X, IsolineY, color = 'black')

# if intersection.geom_type == 'MultiPoint':
#     plt.loglog(*LineString(intersection).xy, 'o')
# elif intersection.geom_type == 'Point':
#     # plt.loglog(*intersection.xy, 'o')
#     x, y = intersection.xy
#     IsoValueX = x
#     IsolineX = [IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX, IsoValueX]
#     plt.loglog(IsolineX, Y,'k--',linewidth=2.5)


legend  = plt.legend(fontsize=14)
# plt.show()
plt.savefig('Figure4.4_Taylor-Green Vortex.png', bbox_inches='tight')