// make_block_special.sce

clear; xdel(winsid());

Lx = 0.15;
Ly = 0.05;
Lz = 0.30;

// Vertices
VertexData = [Lx*ones(8,1),Ly*ones(8,1),Lz*ones(8,1)]...
.*[0,0,0;
1,0,0;
0,1,0;
0,0,1;
1,1,0;
0,1,1;
1,0,1;
1,1,1];

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
h_axes.data_bounds = [-0.15,-0.2,-0.1;0.35,0.3,0.4];
xgrid;
//
