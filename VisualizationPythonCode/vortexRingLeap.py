import taichi as ti
import numpy as np
import math

# ti.init(arch=ti.cpu)
ti.init(arch=ti.gpu)

N = 256
Nx = N
Ny = N
Nz = N
nTotal = Nx * Ny * Nz

dx = 1.0 / N
dt = 0.5 / N
num_substeps = 2  # to deal with large CFL
one_over_six = 1.0 / 6.0
one_over_two_dx = 1.0 / (2.0 * dx)

vel = ti.Vector.field(n=3, dtype=ti.f32, shape=(Nx, Ny, Nz))  # velocity
omega_0 = ti.Vector.field(n=3, dtype=ti.f32, shape=(Nx, Ny, Nz))  # vorticity
omega_1 = ti.Vector.field(n=3, dtype=ti.f32, shape=(Nx, Ny, Nz))  # vorticity
sigma = ti.Vector.field(n=3, dtype=ti.f32, shape=(Nx, Ny, Nz))  # stream function
volume = ti.field(dtype=ti.f32, shape=(Nx, Ny, Nz))  # stream function


class TexPair:
    def __init__(self, cur, nxt):
        self.cur = cur
        self.nxt = nxt

    def swap(self):
        self.cur, self.nxt = self.nxt, self.cur


@ti.kernel
def copy_buffer(dst: ti.template(), src: ti.template()):
    for I in ti.grouped(src):
        dst[I] = src[I]


omega = TexPair(omega_0, omega_1)

color_buffer = ti.Vector.field(n=3, dtype=ti.f32, shape=(Nx, Ny))


@ti.data_oriented
class ColorMap:
    def __init__(self, h, wl, wr, c):
        self.h = h
        self.wl = wl
        self.wr = wr
        self.c = c

    @ti.func
    def clamp(self, x):
        return ti.max(0.0, ti.min(1.0, x))

    @ti.func
    def map(self, x):
        w = 0.0
        if x < self.c:
            w = self.wl
        else:
            w = self.wr
        return self.clamp((w-ti.abs(self.clamp(x)-self.c))/w*self.h)


jetR = ColorMap(1.5, .37, .37, .75)
jetG = ColorMap(1.5, .37, .37, .5)
jetB = ColorMap(1.5, .37, .37, .25)

bwrR = ColorMap(1.0, .25, 1, .5)
bwrG = ColorMap(1.0, .5, .5, .5)
bwrB = ColorMap(1.0, 1, .25, .5)


@ti.func
def color_map(c):
    # this works by chance, must use ti.func in ti.kernel
    # return ti.Vector([jetR.map(c),
    #                   jetG.map(c),
    #                   jetB.map(c)])
    return ti.Vector([bwrR.map(c),
                      bwrG.map(c),
                      bwrB.map(c)])


# -------------------------------------

# [-1, 1]
@ti.func
def xCoord(i):
    return ((i + 0.5) / Nx) * 2.0 - 1.0

# [-1, 1]
@ti.func
def yCoord(j):
    return ((j + 0.5) / Ny) * 2.0 - 1.0

# [-1, 1]
@ti.func
def zCoord(k):
    return ((k + 0.5) / Nz) * 2.0 - 1.0


@ti.func
def mod(a, b):
    return int(a % b)


@ti.func
def sq(x):
    return x ** 2


@ti.kernel
def initialize_vorticity_leapfrog():
    for i, j, k in omega.cur:
        x = xCoord(i)
        y = yCoord(j)
        z = zCoord(k)

        wx = 0.0
        wy = 0.0
        wz = 0.0

        s = 0.5
        ss = -0.9
        sigma = 0.012
        t = 500.0

        d = ti.sqrt(x * x + z * z)

        # add vortex ring
        rr = sq(d - s) + sq(y - ss)
        mag = ti.exp(- rr / (2.0 * sq(sigma))) * t / d
        wx += mag * z
        wz += -mag * x
        # add vortex ring
        rr = sq(d - s * 0.7) + sq(y - ss)
        mag = ti.exp(- rr / (2.0 * sq(sigma))) * t / d
        wx += mag * z
        wz += -mag * x

        omega.cur[i, j, k] = ti.Vector([wx, wy, wz])


def initialize_vortices():
    initialize_vorticity_leapfrog()
    reconstruct_velocity()


# solve L*x=-rhs using periodic boundary
# using red-black ordering for successive over-relaxation/Gauss-Seidel linear solver
@ti.kernel
def linear_solver_step(SOR_factor: ti.f32, mask: ti.template(),
                       x: ti.template(), rhs: ti.template()):
    for i, j, k in x:
        if mod(i + j + k, 2) == mask:
            i_p = mod(i + 1, Nx)
            i_n = mod(i - 1 + Nx, Nx)
            j_p = mod(j + 1, Ny)
            j_n = mod(j - 1 + Ny, Ny)
            k_p = mod(k + 1, Nz)
            k_n = mod(k - 1 + Nz, Nz)

            x_update = (
                - rhs[i, j, k] * sq(dx)
                - x[i_p, j, k]
                - x[i_n, j, k]
                - x[i, j_p, k]
                - x[i, j_n, k]
                - x[i, j, k_p]
                - x[i, j, k_n]) * -one_over_six

            x[i, j, k] = SOR_factor * x_update + \
                (1.0 - SOR_factor) * x[i, j, k]


# @ti.kernel
# def fix_streamfunc():
#     # since velocity is the gradient of sigma (the stream function),
#     # the constant drift in sigma is harmless to velocity,
#     # and the field is eased for better visualization
#     sigmaMean = 0.0
#     for I in ti.grouped(sigma):
#         sigmaMean += sigma[I]
#     sigmaMean /= nTotal
#     for I in sigma:
#         sigma[I] -= sigmaMean


def poisson_solver(num_iterations, x, rhs):
    SOR_factor = 1.99  # 1.0 for Gauss-Seidel

    for iters in range(num_iterations):
        linear_solver_step(SOR_factor, iters % 2, x, rhs)


def solve_streamfunc(num_iterations):
    # solve L*sigma=-omega using periodic boundary
    poisson_solver(num_iterations, sigma, omega.cur)

    # fix_streamfunc()


@ti.kernel
def reconstruct_velocity():
    # u = curl(sigma)
    for i, j, k in sigma:
        i_p = mod(i + 1, Nx)
        i_n = mod(i - 1 + Nx, Nx)
        j_p = mod(j + 1, Ny)
        j_n = mod(j - 1 + Ny, Ny)
        k_p = mod(k + 1, Nz)
        k_n = mod(k - 1 + Nz, Nz)

        vel[i, j, k][0] = ((sigma[i, j_p, k][2] - sigma[i, j_n, k][2]) -
                           (sigma[i, j, k_p][1] - sigma[i, j, k_n][1])) / (2.0 * dx)
        vel[i, j, k][1] = ((sigma[i, j, k_p][0] - sigma[i, j, k_n][0]) -
                           (sigma[i_p, j, k][2] - sigma[i_n, j, k][2])) / (2.0 * dx)
        vel[i, j, k][2] = ((sigma[i_p, j, k][1] - sigma[i_n, j, k][1]) -
                           (sigma[i, j_p, k][0] - sigma[i, j_n, k][0])) / (2.0 * dx)


@ti.kernel
def advect_vorticity(dt_substep: ti.f32):
    # advection of omega by 2nd order upwind scheme
    for i, j, k in omega.cur:
        i_p = mod(i + 1, Nx)
        i_n = mod(i - 1 + Nx, Nx)
        j_p = mod(j + 1, Ny)
        j_n = mod(j - 1 + Ny, Ny)
        k_p = mod(k + 1, Nz)
        k_n = mod(k - 1 + Nz, Nz)

        i_p2 = mod(i + 2, Nx)
        i_n2 = mod(i - 2 + Nx, Nx)
        j_p2 = mod(j + 2, Ny)
        j_n2 = mod(j - 2 + Ny, Ny)
        k_p2 = mod(k + 2, Nz)
        k_n2 = mod(k - 2 + Nz, Nz)

        ux_pos = max(vel[i, j, k][0], 0.0)
        ux_neg = min(vel[i, j, k][0], 0.0)
        uy_pos = max(vel[i, j, k][1], 0.0)
        uy_neg = min(vel[i, j, k][1], 0.0)
        uz_pos = max(vel[i, j, k][2], 0.0)
        uz_neg = min(vel[i, j, k][2], 0.0)

        # first order
        # omega_dx_pos = (  omega.cur[i_p, j, k] - omega.cur[i, j, k]) / (dx)
        # omega_dx_neg = (- omega.cur[i_n, j, k] + omega.cur[i, j, k]) / (dx)
        # omega_dy_pos = (  omega.cur[i, j_p, k] - omega.cur[i, j, k]) / (dx)
        # omega_dy_neg = (- omega.cur[i, j_n, k] + omega.cur[i, j, k]) / (dx)
        # omega_dz_pos = (  omega.cur[i, j, k_p] - omega.cur[i, j, k]) / (dx)
        # omega_dz_neg = (- omega.cur[i, j, k_n] + omega.cur[i, j, k]) / (dx)

        # second order
        omega_dx_pos = \
            (- omega.cur[i_p2, j, k] + 4.0 * omega.cur[i_p, j, k] -
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)
        omega_dx_neg = \
            (omega.cur[i_n2, j, k] - 4.0 * omega.cur[i_n, j, k] +
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)
        omega_dy_pos = \
            (- omega.cur[i, j_p2, k] + 4.0 * omega.cur[i, j_p, k] -
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)
        omega_dy_neg = \
            (omega.cur[i, j_n2, k] - 4.0 * omega.cur[i, j_n, k] +
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)
        omega_dz_pos = \
            (- omega.cur[i, j, k_p2] + 4.0 * omega.cur[i, j, k_p] -
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)
        omega_dz_neg = \
            (omega.cur[i, j, k_n2] - 4.0 * omega.cur[i, j, k_n] +
             3.0 * omega.cur[i, j, k]) / (2.0 * dx)

        vort = omega.cur[i, j, k] - dt_substep * (
            ux_pos * omega_dx_neg + ux_neg * omega_dx_pos +
            uy_pos * omega_dy_neg + uy_neg * omega_dy_pos +
            uz_pos * omega_dz_neg + uz_neg * omega_dz_pos)

        # vortex stretching term
        u_dx = (vel[i_p, j, k] - vel[i_n, j, k]) * one_over_two_dx
        u_dy = (vel[i, j_p, k] - vel[i, j_n, k]) * one_over_two_dx
        u_dz = (vel[i, j, k_p] - vel[i, j, k_n]) * one_over_two_dx

        grad_u = ti.Matrix.cols([u_dx, u_dy, u_dz])

        vort += grad_u @ omega.cur[i, j, k] * dt_substep

        omega.nxt[i, j, k] = vort


def solve_velocity_streamfunc():
    solve_streamfunc(20)
    reconstruct_velocity()


def initialize():
    initialize_vortices()
    sigma.fill(ti.Vector([0.0, 0.0, 0.0]))
    solve_streamfunc(1000)


def run_iteration():
    dt_substep = dt / num_substeps
    for iters in range(num_substeps):
        advect_vorticity(dt_substep)
        # omega.swap() # not working
        copy_buffer(omega.cur, omega.nxt)

    solve_velocity_streamfunc()


@ti.func
def phong_shading(normal):
    light_dir = ti.Vector([1.0, 1.0, 1.0]).normalized()
    eye_dir = ti.Vector([0.0, 0.0, 1.0])
    half_dir = (light_dir + eye_dir).normalized()
    return 0.1 + ti.abs(light_dir.dot(normal)) + 3.0 * ti.pow(ti.abs(half_dir.dot(normal)), 60.0)


@ti.kernel
def volume_render(vol: ti.template(), fb: ti.template()):
    for i, j in fb:
        c = ti.Vector([0.0, 0.0, 0.0])
        f = 1.0
        for k in range(Nz):
            i_p = mod(i + 1, Nx)
            i_n = mod(i - 1 + Nx, Nx)
            j_p = mod(j + 1, Ny)
            j_n = mod(j - 1 + Ny, Ny)
            k_p = mod(k + 1, Nz)
            k_n = mod(k - 1 + Nz, Nz)
            normal = -ti.Vector([
                vol[i_p, j, k] - vol[i_n, j, k],
                vol[i, j_p, k] - vol[i, j_n, k],
                vol[i, j, k_p] - vol[i, j, k_n]])
            normal *= 1.0 / (1e-8 + normal.norm())
            shading = phong_shading(normal)
            v = ti.max(0.0, ti.min(1.0, vol[i, j, k] * 20.0))
            c += color_map(v) * f * shading * v
            f *= 1.0 - v * 0.05
        fb[i, j] = c * 0.08


@ti.kernel
def fill_velocity():
    for i, j in color_buffer:
        color_buffer[i, j] = color_map(vel[i, j, Nz // 2].norm())


@ti.kernel
def fill_vorticity():
    for i, j, k in omega.cur:
        volume[i, j, k] = omega.cur[i, j, k].norm() * 0.002


@ti.kernel
def fill_streamfunc():
    for i, j in color_buffer:
        c = sigma[i, j, Nz // 2].norm() * 5.0 + 0.5
        color_buffer[i, j] = color_map(c)


class Viewer:
    def __init__(self, dump):
        self.display_mode = 0
        self.is_active = True
        self.dump = dump
        self.frame = 0

        if self.dump:
            result_dir = "./results"
            self.video_manager = ti.VideoManager(
                output_dir=result_dir, framerate=24, automatic_build=False)

    def toggle(self):
        self.display_mode = (self.display_mode + 1) % 3

    def active(self):
        return self.is_active

    def draw(self, gui):
        if self.display_mode == 0:
            fill_vorticity()
            color_buffer.fill(ti.Vector([0.0, 0.0, 0.0]))
            volume_render(volume, color_buffer)
            display_name = "vorticity"
        elif self.display_mode == 1:
            fill_velocity()
            display_name = "velocity"
        else:
            fill_streamfunc()
            display_name = "stream function"

        img = color_buffer.to_numpy()
        gui.set_image(img)
        gui.text(content=f"display: {display_name}",
                 pos=(0.0, 0.99), color=0xFFFFFF)

        if self.dump:
            self.video_manager.write_frame(img)
            print(f"\rframe {self.frame} written", end="")
            self.frame = self.frame + 1

            if self.frame == 200:
                self.video_manager.make_video(gif=True, mp4=True)
                self.is_active = False

@ti.kernel
def writeTXT():
    file = open('VelocityField.txt', 'w')  #打开文件
    for i, j, k in vel:
        file.write(vel[i,j,k][0])
        file.write(',')
        file.write(vel[i,j,k][1])
        file.write(',')
        file.write(vel[i,j,k][2])
        file.write('\n')
        #np.savetxt("VelocityField.csv", vel.to_numpy(), delimiter=',')
    file.close()

def main():
    initialize()

    viewer = Viewer(False)
    gui = ti.GUI("vortex method", Nx, Ny)
    while viewer.active():
        for e in gui.get_events(ti.GUI.PRESS):
            if e.key == ti.GUI.ESCAPE or e.key == 'q':
                exit(0)
            elif e.key == 'r':
                initialize()
            elif e.key == ' ':
                viewer.toggle()

        for iters in range(10):
            run_iteration()

        viewer.draw(gui)

        gui.text(content="r: reset simulation", pos=(0, 0.86), color=0xFFFFFF)
        gui.text(content="q, esc: quit", pos=(0, 0.83), color=0xFFFFFF)
        gui.text(content="space: toggle display mode",
                 pos=(0, 0.8), color=0xFFFFFF)

        gui.show()
        Velocity = vel.to_numpy()
        
        print("%f,%f,%f \n" % (Velocity[0,0,0][0], Velocity[0,0,0][1], Velocity[0,0,0][2]))
        print("%f,%f,%f \n" % (Velocity[0,0,1][0], Velocity[0,0,1][1], Velocity[0,0,1][2]))
        print("%f,%f,%f \n" % (Velocity[0,1,0][0], Velocity[0,1,0][1], Velocity[0,1,0][2]))
        print("%f,%f,%f \n" % (Velocity[1,0,0][0], Velocity[1,0,0][1], Velocity[1,0,0][2]))
        #Velocity.tofile("VelField.txt", sep=",", format='%f')
        break


if __name__ == '__main__':
    main()