// make_cylinder_general.sce

clear; xdel(winsid());

// Cylinder specification
r = [1,1,1];        // Reference position
A = [-%pi/3, 0, 0]; // Reference orientation (x-y-z Euler angle)

Radius = 0.1;
Height = 0.3;
SideCount = 20;

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
n_side = SideCount;

for i_ver=1:n_side
    VertexData_0(i_ver,:) = [Radius*cos(2*%pi/n_side*i_ver),Radius*sin(2*%pi/n_side*i_ver),0];
    VertexData_0(n_side+i_ver,:) = [Radius*cos(2*%pi/n_side*i_ver),Radius*sin(2*%pi/n_side*i_ver),Height];
end

n_ver = 2*n_side;

for i_ver=1:n_ver
    VertexData(i_ver,:) = r + VertexData_0(i_ver,:)*R';
end

// Side Patches
for i_pat=1:n_side-1
    Index_Patch1(i_pat,:) = [i_pat,i_pat+1,i_pat+1+n_side,i_pat+n_side];
end
Index_Patch1(n_side,:) = [n_side,1,1+n_side,2*n_side];

for i_pat=1:n_side

    // Side patches data
    PatchData1_X(:,i_pat) = VertexData(Index_Patch1(i_pat,:),1);
    PatchData1_Y(:,i_pat) = VertexData(Index_Patch1(i_pat,:),2);
    PatchData1_Z(:,i_pat) = VertexData(Index_Patch1(i_pat,:),3);
end

// Draw side patches
figure(1);
plot3d(PatchData1_X,PatchData1_Y,PatchData1_Z);
h_fac3d = gce();
h_fac3d.color_mode = 4;
h_fac3d.foreground = 1;
h_fac3d.hiddencolor = 4;

// Bottom Patches
Index_Patch2(1,:) = [1:n_side];
Index_Patch2(2,:) = [n_side+1:2*n_side];

for i_pat=1:2

    // Bottom patches data
    PatchData2_X(:,i_pat) = VertexData(Index_Patch2(i_pat,:),1);
    PatchData2_Y(:,i_pat) = VertexData(Index_Patch2(i_pat,:),2);
    PatchData2_Z(:,i_pat) = VertexData(Index_Patch2(i_pat,:),3);
end

// Draw bottom patches
figure(1);
plot3d(PatchData2_X,PatchData2_Y,PatchData2_Z);
h2_fac3d = gce();
h2_fac3d.color_mode = 4;
h2_fac3d.foreground = 1;
h2_fac3d.hiddencolor = 4;

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
