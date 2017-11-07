// make_block_special.sce

clear; xdel(winsid());

// Block specification
r = [1,1,1];            // Reference position
A = [-%pi/3, 0, %pi/6]; // Reference orientation (x-y-z Euler angle)

Lx = 0.15;
Ly = 0.05;
Lz = 0.30;

// Euler angle -> Orientation matrix
a1 = A(1);
a2 = A(2);
a3 = A(3);

R1 = ...
[1, 0, 0;
0, cos(a1), -sin(a1);
0, sin(a1), cos(a1)];

R2 = ...
[cos(a2), 0, sin(a2);
0, 1, 0;
-sin(a2), 0, cos(a2)];

R3 = ...
[cos(a3), -sin(a3), 0;
sin(a3), cos(a3), 0;
0, 0, 1];

R = R1*R2*R3;

// Vertices
VertexData_0 = [Lx*ones(8,1),Ly*ones(8,1),Lz*ones(8,1)]...
.*[0,0,0;
1,0,0;
0,1,0;
0,0,1;
1,1,0;
0,1,1;
1,0,1;
1,1,1];

n_ver = 8;

for i_ver=1:n_ver
    VertexData(i_ver,:) = r + VertexData_0(i_ver,:)*R';
end

// Patches
Index_Patch = ...
[1,2,5,3;
1,3,6,4;
1,4,7,2;
4,7,8,6;
2,5,8,7;
3,6,8,5];

n_pat = 6;

for i_pat=1:n_pat

    // Patches data
    PatchData_X(:,i_pat) = VertexData(Index_Patch(i_pat,:),1);
    PatchData_Y(:,i_pat) = VertexData(Index_Patch(i_pat,:),2);
    PatchData_Z(:,i_pat) = VertexData(Index_Patch(i_pat,:),3);
end

// Draw patches
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
