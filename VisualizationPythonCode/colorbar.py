import matplotlib as mpl
import matplotlib.pyplot as plt

fig, ax = plt.subplots(figsize=(1.5,8),dpi=100)
plt.tick_params(labelsize=40)
fraction = 1  # .05

norm = mpl.colors.Normalize(vmin=0, vmax=0.00135)
cbar = ax.figure.colorbar(
            mpl.cm.ScalarMappable(norm=norm, cmap = 'rainbow'),
            ax=ax, pad=.05, fraction=fraction)

cbar.ax.tick_params(labelsize='20')

ax.axis('off')
plt.savefig('colorbar.png', bbox_inches='tight')