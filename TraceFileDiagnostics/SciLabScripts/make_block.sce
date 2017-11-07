// make_block.sce

clear; xdel(winsid());

exec('Euler2R.sci',-1);
exec('GeoVerMakeBlock.sci',-1);
exec('GeoPatMakeBlock.sci',-1);

// Block specification
r = [1,1,1];            // Reference position
A = [-%pi/3, 0, %pi/6]; // Reference orientation (x-y-z Euler angle)

Lx = 0.15;
Ly = 0.05;
Lz = 0.30;

// Euler angle -> Orientation matrix
R = Euler2R(A);

// Vertices
VertexData = GeoVerMakeBlock(r,R,[Lx,Ly,Lz]);

// Patches
[PatchData_X,PatchData_Y,PatchData_Z] = GeoPatMakeBlock(VertexData);

// Draw patch
figure(1);
plot3d(PatchData_X,PatchData_Y,PatchData_Z);
h_fac3d = gce();
h_fac3d.color_mode = 4;
h_fac3d.foreground = 1;
h_fac3d.hiddencolor = 4;

// Axes settings
xlabel("x",'fontsize',2);
ylabel("y",'fontsize',2);
zlabel("z",'fontsize',2);
h_axes = gca();
h_axes.font_size = 2;
h_axes.isoview = "on";
h_axes.box = "off";
h_axes.rotation_angles = [63.5,-127];
h_axes.data_bounds = [0.8,0.9,0.8;1.3,1.4,1.3];
xgrid;
