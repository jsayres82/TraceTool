// make_cylinder_special.sce

clear; xdel(winsid());

Radius = 0.1;
Height = 0.3;
SideCount = 20;

// Vertices
n_side = SideCount;

for i_ver=1:n_side
    VertexData(i_ver,:) = [Radius*cos(2*%pi/n_side*i_ver),Radius*sin(2*%pi/n_side*i_ver),0];
    VertexData(n_side+i_ver,:) = [Radius*cos(2*%pi/n_side*i_ver),Radius*sin(2*%pi/n_side*i_ver),Height];
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
h1_fac3d = gce();
h1_fac3d.color_mode = 4;
h1_fac3d.foreground = 1;
h1_fac3d.hiddencolor = 4;

// Bottom Patches
Index_Patch2(1,:) = [1:n_side];
Index_Patch2(2,:) = [n_side+1:2*n_side];

for i_pat=1:2

    // Bottom patch data
    PatchData2_X(:,i_pat) = VertexData(Index_Patch2(i_pat,:),1);
    PatchData2_Y(:,i_pat) = VertexData(Index_Patch2(i_pat,:),2);
    PatchData2_Z(:,i_pat) = VertexData(Index_Patch2(i_pat,:),3);
end

// Draw bottom patches
figure(1);
plot3d(PatchData2_X,PatchData2_Y,PatchData2_Z);
h2_fac3d(i_pat) = gce();
h2_fac3d(i_pat).color_mode = 4;
h2_fac3d(i_pat).foreground = 1;
h2_fac3d(i_pat).hiddencolor = 4;

// Axes settings
xlabel("x",'fontsize',2);
ylabel("y",'fontsize',2);
zlabel("z",'fontsize',2);
h_axes = gca();
h_axes.font_size = 2;
h_axes.isoview = "on";
h_axes.box = "off";
h_axes.rotation_angles = [63.5,-127];
h_axes.data_bounds = [-0.2,-0.2,0;0.2,0.2,0.4];
xgrid;
