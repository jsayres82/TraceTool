// anim_block_translate.sce

clear; xdel(winsid());

exec('Euler2R.sci',-1);
exec('GeoVerMakeBlock.sci',-1);
exec('GeoPatMakeBlock.sci',-1);

// Block specification
Lx = 0.15;
Ly = 0.05;
Lz = 0.30;

// Motion data
t = [0:0.005:2]';                 // Time data
r = [0.5*sin(2*%pi*t), 0*t, 0*t]; // Position data
A = [0*t, 0*t, 0*t];              // Orientation data (x-y-z Euler angle)

n_time = length(t);

// Compute propagation of vertices and patches
for i_time=1:n_time

    R = Euler2R(A(i_time,:));
    VertexData(:,:,i_time) = GeoVerMakeBlock(r(i_time,:),R,[Lx,Ly,Lz]);
    [X,Y,Z] = GeoPatMakeBlock(VertexData(:,:,i_time));
    PatchData_X(:,:,i_time) = X;
    PatchData_Y(:,:,i_time) = Y;
    PatchData_Z(:,:,i_time) = Z;
end

// Draw initial figure
figure(1);
plot3d(PatchData_X(:,:,1),PatchData_Y(:,:,1),PatchData_Z(:,:,1));
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
h_axes.data_bounds = [-0.5,-0.5,-0.5;0.7,0.5,0.5];
xgrid;

// Animation Loop
for i_time=1:n_time

    drawlater();
    h_fac3d.data.x = PatchData_X(:,:,i_time);
    h_fac3d.data.y = PatchData_Y(:,:,i_time);
    h_fac3d.data.z = PatchData_Z(:,:,i_time);
    drawnow();
end
