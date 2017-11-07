// anim_cylinder_translate.sce

clear; xdel(winsid());

exec('C:/Users/jsayres/Documents/Visual Studio 2012/Projects/ServiceTool - Copy/ServiceTool/SciLabScripts/Euler2R.sci',-1);
exec('C:/Users/jsayres/Documents/Visual Studio 2012/Projects/ServiceTool - Copy/ServiceTool/SciLabScripts/GeoVerMakeCylinder.sci',-1);
exec('C:/Users/jsayres/Documents/Visual Studio 2012/Projects/ServiceTool - Copy/ServiceTool/SciLabScripts/GeoPatMakeCylinderSides.sci',-1);
exec('C:/Users/jsayres/Documents/Visual Studio 2012/Projects/ServiceTool - Copy/ServiceTool/SciLabScripts/GeoPatMakeCylinderBottom.sci',-1);

// Motion data
t = [0:0.003:2]';                 // Time data
r = [0.5*sin(2*%pi*t), 0*t, 0*t]; // Position data
A = [0*t, 0*t, 0*t];              // Orientation data (x-y-z Euler angle)

n_time = length(t);

// Compute propagation of vertices and patches
for i_time=1:n_time

    R = Euler2R(A(i_time,:));
    VertexData(:,:,i_time) = GeoVerMakeCylinder(r(i_time,:),A,.3,.1);
    [XS,YS,ZS] = GeoPatMakeCylinderSides(VertexData(:,:,i_time), 20);
    PatchData1_X(:,:,i_time) = XS;
    PatchData1_Y(:,:,i_time) = YS;
    PatchData1_Z(:,:,i_time) = ZS;
    [XB,YB,ZB] = GeoPatMakeCylinderBottom(VertexData(:,:,i_time), 20);
    PatchData2_X(:,:,i_time) = XB;
    PatchData2_Y(:,:,i_time) = YB;
    PatchData2_Z(:,:,i_time) = ZB;
    
end

// Vertices
x(1)=0.75; y(1)=0.5; z(1)=-0.6;
x(2)=0.75; y(2)=-0.5; z(2)=-0.6;
x(3)=0.75; y(3)=-0.5; z(3)=0.6;
x(4)=0.75; y(4)=0.5; z(4)=0.6;

// Draw patch
figure(1);
plot3d(x,y,z);

// Draw initial figure
// Draw side patches
figure(1);
plot3d(PatchData1_X(:,:,1),PatchData1_Y(:,:,1),PatchData1_Z(:,:,1));
h_fac3d = gce();
h_fac3d.color_mode = 4;
h_fac3d.foreground = 1;
h_fac3d.hiddencolor = 4;

// Draw bottom patches
figure(1);
plot3d(PatchData2_X(:,:,1),PatchData2_Y(:,:,1),PatchData2_Z(:,:,1));
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
h_axes.data_bounds = [-0.5,-0.5,-0.5;0.7,0.5,0.5];
xgrid;

// Animation Loop
for i_time=1:n_time

    drawlater();
    h_fac3d.data.x = PatchData1_X(:,:,i_time);
    h_fac3d.data.y = PatchData1_Y(:,:,i_time);
    h_fac3d.data.z = PatchData1_Z(:,:,i_time);
    h2_fac3d.data.x = PatchData2_X(:,:,i_time);
    h2_fac3d.data.y = PatchData2_Y(:,:,i_time);
    h2_fac3d.data.z = PatchData2_Z(:,:,i_time);
    drawnow();
end
