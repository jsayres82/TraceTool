function [PatchData_X,PatchData_Y,PatchData_Z] = GeoPatMakeCylinderBottom(VertexData, SideCount)

// Vertices
n_side = SideCount;

// Bottom Patches
Index_Patch(1,:) = [1:n_side];
Index_Patch(2,:) = [n_side+1:2*n_side];

for i_pat=1:2

    // Bottom patches data
    PatchData_X(:,i_pat) = VertexData(Index_Patch(i_pat,:),1);
    PatchData_Y(:,i_pat) = VertexData(Index_Patch(i_pat,:),2);
    PatchData_Z(:,i_pat) = VertexData(Index_Patch(i_pat,:),3);
end


// Bottom Patches
Index_Patch2(1,:) = [1:n_side];
Index_Patch2(2,:) = [n_side+1:2*n_side];

for i_pat=1:2

    // Bottom patches data
    PatchData_X(:,i_pat) = VertexData(Index_Patch2(i_pat,:),1);
    PatchData_Y(:,i_pat) = VertexData(Index_Patch2(i_pat,:),2);
    PatchData_Z(:,i_pat) = VertexData(Index_Patch2(i_pat,:),3);
end

endfunction
