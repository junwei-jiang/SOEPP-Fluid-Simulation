# A Second-Order Explicit Pressure Projection Method for Eulerian Fluid Simulation

#### Junwei Jiang, Xiangda Shen, Yuning Gong, Zeng Fan, Yanli Liu, Guanyu Xing, Xiaohua Ren, Yanci Zhang

#### Accepted by ACM SIGGRAPH / Eurographics Symposium on Computer Animation 2022 / Computer Graphics Forum

![image](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Figure1_CoverPicture.png)

#### Abstract

In this paper, we propose a novel second-order explicit midpoint method to address the issue of energy loss and vorticity dissipation in Eulerian fluid simulation. The basic idea is to explicitly compute the pressure gradient at the middle time of each time step and apply it to the velocity field after advection. Theoretically, our solver can achieve higher accuracy than the first-order solvers at similar computational cost. On the other hand, our method is twice and even faster than the implicit second-order solvers at the cost of a small loss of accuracy. We have carried out a large number of 2D, 3D and numerical experiments to verify the effectiveness and availability ofour algorithm.

#### Compile and Run
The project is built by Unity, and the parallel computing part uses Compute Shader. The versions of C + + and CUDA are in another project (only the basic engine is implemented, and there is no specific algorithm). Visualization may require Python, OpenVDB and Houdini. The code is not neat enough. If you have any questions, please ask me. My Email is 947205526@qq.com

#### Paper Videos
[![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/First%20frame.png)](https://www.youtube.com/watch?v=OpJk6lVpw9Q)

#### Other Videos

The following videos were generated using this engine:

|             *Dam Break PIC*             |    *Dam Break FLIP*    |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Dam%20Break%20PIC.png)](https://www.youtube.com/watch?v=gkT8r-dx-Oc) | [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Dam%20Break%20FLIP.png)](https://www.youtube.com/watch?v=ap1wn4v9Y0I)
|             *Complex Dynamic Boundary*             |    *SDF field visualization*    |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Complex%20Dynamic%20Boundary.png)](https://www.youtube.com/watch?v=2p0gEe0cDX0) | [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/SDF%20field%20visualization.png)](https://www.youtube.com/watch?v=YwoxHR0Mffw) 
|             *Dam*             |    *Overflow Weir*    |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Dam.png)](https://www.youtube.com/watch?v=WP8THyQTKhI) | [![Video](https://github.com/junwei-jiang/SOEPP-Fluid-Simulation/blob/main/images/Overflow%20Weir.png)](https://www.youtube.com/watch?v=aNk-95Fd9eY) 
