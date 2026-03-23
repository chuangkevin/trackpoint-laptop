FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

annotation { "Feature Type Name" : "Lenovo L390 Battery" }
export const l390Battery = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Thickness" }
        isLength(definition.thickness, { (millimeter) : [1, 6.7, 20] } as LengthBoundSpec);
    }
    {
        // Lenovo ThinkPad L390 Battery (L17C3P53 / L17L3P53 / L17M3P55)
        var length = 291.3 * millimeter;
        var maxHeight = 81.5 * millimeter; // Height including the raised control board
        var mainHeight = 73.0 * millimeter; // Height of the left/right main sections
        var thickness = definition.thickness;
        
        var sketch1 = newSketchOnPlane(context, id + "sketch1", {
            "sketchPlane" : plane(vector(0, 0, 0) * meter, vector(0, 0, 1))
        });

        var bottomY = -maxHeight/2;
        var mainTopY = bottomY + mainHeight;
        var raisedTopY = maxHeight/2;

        // 1. Main battery body (lower section, full length)
        skRectangle(sketch1, "mainBody", {
            "firstCorner" : vector(-length/2, bottomY),
            "secondCorner" : vector(length/2, mainTopY)
        });
        
        // 2. Raised control board section (middle-right)
        var raisedStartX = -10.65 * millimeter; // Steps up after the barcode
        var raisedEndX = 43.35 * millimeter;    // Drops down right before Top Tab 2
        
        skRectangle(sketch1, "raisedSection", {
            "firstCorner" : vector(raisedStartX, mainTopY - 1 * millimeter), // Overlap to merge regions
            "secondCorner" : vector(raisedEndX, raisedTopY)
        });

        // Function to add mounting tabs with screw holes
        const addTab = function(sketch, tabId, xCenter, yBase, yTip, isTop) {
            var tabWidth = 12 * millimeter;
            var holeRadius = 1.25 * millimeter; // 2.5mm hole for M2
            var yDir = isTop ? 1 : -1;
            
            // Overlap by 2mm into the body
            var yInner = yBase - 2 * millimeter * yDir;
            
            skRectangle(sketch, tabId ~ "_rect", {
                "firstCorner" : vector(xCenter - tabWidth/2, yInner),
                "secondCorner" : vector(xCenter + tabWidth/2, yTip)
            });
            
            // Hole is 3mm from the tip
            skCircle(sketch, tabId ~ "_hole", {
                "center" : vector(xCenter, yTip - 3 * millimeter * yDir),
                "radius" : holeRadius
            });
        };
        
        // 3. Add Top Tabs
        // topTab1: Left side. Tip is flush with the raised section max height.
        addTab(sketch1, "topTab1", -60.65 * millimeter, mainTopY, raisedTopY, true);
        
        // topTab2: Middle-right. Left edge is flush with the drop-down of the raised section.
        addTab(sketch1, "topTab2", 49.35 * millimeter, mainTopY, raisedTopY, true);
        
        // topTab3: Far right corner.
        addTab(sketch1, "topTab3", 139.35 * millimeter, mainTopY, raisedTopY, true);
        
        // 4. Add Top Connector Bump (cable exit on top of the raised section)
        var bumpWidth = 20 * millimeter;
        var bumpHeight = 3 * millimeter;
        var bumpCenter = 15.0 * millimeter;
        skRectangle(sketch1, "connectorBump", {
            "firstCorner" : vector(bumpCenter - bumpWidth/2, raisedTopY - 1 * millimeter),
            "secondCorner" : vector(bumpCenter + bumpWidth/2, raisedTopY + bumpHeight)
        });
        
        // 5. Add Bottom Tabs (protruding downwards)
        var bottomTabTip = bottomY - 8 * millimeter;
        // botTab1: Center-right (directly below the cable bump)
        addTab(sketch1, "botTab1", 14.35 * millimeter, bottomY, bottomTabTip, false);
        // botTab2: Right side
        addTab(sketch1, "botTab2", 89.35 * millimeter, bottomY, bottomTabTip, false);

        skSolve(sketch1);

        // Extrude the entire sketch
        opExtrude(context, id + "extrude1", {
            "entities" : qSketchRegion(id + "sketch1"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : thickness
        });

        // 6. Add fillets to edges to simulate shrink-wrapped cells
        try { 
            opFillet(context, id + "filletEdges", {
                "entities" : qCreatedBy(id + "extrude1", EntityType.EDGE),
                "radius" : 0.25 * millimeter
            }); 
        }
    });