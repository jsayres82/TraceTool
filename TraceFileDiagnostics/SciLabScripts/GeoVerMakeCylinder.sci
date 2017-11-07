function VertexData = GeoVerMakeCylinder(Location,Orientation,VialHeight,VialRadius)

// Cylinder specification
r = Location;       // Reference position
A = Orientation; // Reference orientation (x-y-z Euler angle)


R = Orientation;

// Euler angle -> Orientation matrix
a1 = A(1);
a2 = A(2);
a3 = A(3);

Radius = VialRadius;
Height = VialHeight;
SideCount = 20;

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


endfunction
