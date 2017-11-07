function [PatchData_X,PatchData_Y,PatchData_Z] = GeoPatMakeCylinderSides(VertexData, SideCount)

// Vertices
n_side = SideCount;

// Side Patches
for i_pat=1:n_side-1
    Index_Patch(i_pat,:) = [i_pat,i_pat+1,i_pat+1+n_side,i_pat+n_side];
end
Index_Patch(n_side,:) = [n_side,1,1+n_side,2*n_side];

for i_pat=1:n_side

    // Side patches data
    PatchData_X(:,i_pat) = VertexData(Index_Patch(i_pat,:),1);
    PatchData_Y(:,i_pat) = VertexData(Index_Patch(i_pat,:),2);
    PatchData_Z(:,i_pat) = VertexData(Index_Patch(i_pat,:),3);
end


endfunction
