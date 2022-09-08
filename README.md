# A Second-Order Explicit Pressure Projection Method for Eulerian Fluid Simulation

#### Junwei Jiang, Xiangda Shen, Yuning Gong, Zeng Fan, Yanli Liu, Guanyu Xing, Xiaohua Ren, Yanci Zhang

#### Accepted by ACM SIGGRAPH / Eurographics Symposium on Computer Animation 2022 / CGF

![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure1_CoverPicture.png)

#### Abstract

In this paper, we propose a novel second-order explicit midpoint method to address the issue of energy loss and vorticity dissipation in Eulerian fluid simulation. The basic idea is to explicitly compute the pressure gradient at the middle time of each time step and apply it to the velocity field after advection. Theoretically, our solver can achieve higher accuracy than the first-order solvers at similar computational cost. On the other hand, our method is twice and even faster than the implicit second-order solvers at the cost of a small loss of accuracy. We have carried out a large number of 2D, 3D and numerical experiments to verify the effectiveness and availability ofour algorithm.

#### 2D Results
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.1_TaylorVortexResults.png)
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.2_2DVortexLeapfroggingResults.png)
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.3_VortexSheetResults.png)
#### 3D Results
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.6_3DSmokePlumeResults.png)
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.7_DamBreakResults.png)
#### Numerical Results
![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure4.4_Taylor-Green Vortex.png)