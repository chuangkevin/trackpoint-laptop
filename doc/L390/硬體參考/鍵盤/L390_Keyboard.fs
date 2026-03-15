FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

annotation { "Feature Type Name" : "L390 Keyboard" }
export const l390Keyboard = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        // ============================================================
        // ThinkPad L390 Keyboard - actual shape from reference photos
        // The base plate is NOT a rectangle - bottom has notches,
        // only center extends down for TrackPoint buttons + mount bar
        // ============================================================

        var u = 19 * millimeter;
        var cap = 15 * millimeter;
        var hcap = cap / 2;
        var plateThk = 1.5 * millimeter;
        var keyH = 1.6 * millimeter;

        // Function row dimensions
        var fw = 14 * millimeter;
        var fh = 12 * millimeter;
        var hfw = fw / 2;
        var hfh = fh / 2;

        // Main body dimensions
        var kbW = 300 * millimeter;
        var fnRowH = 16 * millimeter;
        var mainRowsH = 5 * u; // rows 1-5
        var mainH = fnRowH + mainRowsH; // ~111mm

        // Bottom extensions - shallow protrusions
        var btnStripH = 4 * millimeter;
        var btnGapAbove = 1 * millimeter;
        var mountBarH = 6 * millimeter;
        var mountGap = 1 * millimeter;
        var extH = btnGapAbove + btnStripH + mountGap + mountBarH; // ~12mm

        // Center extension (TrackPoint buttons + mount bar)
        var extW = 90 * millimeter;
        var halfExtW = extW / 2;

        // Right extension (nav cluster: PgUp/arrows/PgDn)
        var navExtW = 42 * millimeter;

        // Coordinate system: origin at center of main body
        var halfW = kbW / 2;
        var halfH = mainH / 2;

        // ==========================================
        // 1. BASE PLATE - non-rectangular outline
        // ==========================================
        //  +--------------------------------------------+
        //  |                                            |
        //  |          [all 6 key rows]                  |
        //  |                                            |
        //  +--------+                +---+--------------+
        //           |  [TP buttons]  |   |  [nav keys]  |
        //           |  [mount bar]   |   |              |
        //           +----------------+   +--------------+

        var bs = newSketchOnPlane(context, id + "bs", {
                "sketchPlane" : plane(vector(0, 0, 0) * meter, vector(0, 0, 1))
        });

        var margin = 3 * millimeter;
        var topY = halfH + margin;
        var botMain = -halfH;
        var botExt = botMain - extH;
        var navInnerX = halfW + margin - navExtW; // inner edge of nav protrusion

        skPolyline(bs, "outline", {
                "points" : [
                    vector(-halfW - margin, topY),              // 1. top-left
                    vector(halfW + margin, topY),               // 2. top-right
                    vector(halfW + margin, botExt),             // 3. right edge goes all the way down (nav protrusion)
                    vector(navInnerX, botExt),                  // 4. nav protrusion bottom-left
                    vector(navInnerX, botMain),                 // 5. step up to main body bottom
                    vector(halfExtW, botMain),                  // 6. gap: go left to center extension right edge
                    vector(halfExtW, botExt),                   // 7. center extension right side down
                    vector(-halfExtW, botExt),                  // 8. center extension bottom-left
                    vector(-halfExtW, botMain),                 // 9. center extension left side up
                    vector(-halfW - margin, botMain),           // 10. main body bottom-left
                    vector(-halfW - margin, topY)               // 11. close to top-left
                ]
        });

        skSolve(bs);

        opExtrude(context, id + "eb", {
                "entities" : qSketchRegion(id + "bs"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : plateThk
        });

        try { opFillet(context, id + "bf", {
                "entities" : qCreatedBy(id + "eb", EntityType.EDGE),
                "radius" : 1.5 * millimeter
        }); }

        // ==========================================
        // 2. KEYCAPS
        // ==========================================
        var ks = newSketchOnPlane(context, id + "ks", {
                "sketchPlane" : plane(vector(0, 0, plateThk), vector(0, 0, 1))
        });
        var ki = 0;
        var left = -halfW;

        // --- ROW 0: Function row ---
        var y0 = halfH - fnRowH / 2;
        var fkPitch = fw + 2 * millimeter;
        var groupGap = 6 * millimeter;
        var x = left + 10 * millimeter + hfw;

        // Esc
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - hfw, y0 - hfh),
                "secondCorner" : vector(x + hfw, y0 + hfh)
        });
        ki += 1;
        x = x + fkPitch + groupGap;

        // F1-F4, gap, F5-F8, gap, F9-F12, gap, Home-End-Ins-Del
        var fnGroups = [4, 4, 4, 4];
        for (var g = 0; g < 4; g += 1)
        {
            for (var i = 0; i < fnGroups[g]; i += 1)
            {
                skRectangle(ks, "k" ~ ki, {
                        "firstCorner" : vector(x - hfw, y0 - hfh),
                        "secondCorner" : vector(x + hfw, y0 + hfh)
                });
                ki += 1;
                x = x + fkPitch;
            }
            if (g < 3)
            {
                x = x + groupGap;
            }
        }

        // --- ROW 1: ` 1-0 - = Backspace ---
        var y1 = halfH - fnRowH - u / 2;
        x = left + u / 2;
        for (var i = 0; i < 13; i += 1)
        {
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x - hcap, y1 - hcap),
                    "secondCorner" : vector(x + hcap, y1 + hcap)
            });
            ki += 1;
            x = x + u;
        }
        var bsW = kbW - 13 * u;
        var bsCap = bsW - 4 * millimeter;
        x = left + 13 * u + bsW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - bsCap / 2, y1 - hcap),
                "secondCorner" : vector(x + bsCap / 2, y1 + hcap)
        });
        ki += 1;

        // --- ROW 2: Tab(1.5u) QWERTYUIOP[]\ ---
        var y2 = y1 - u;
        var tabW = 1.5 * u;
        x = left + tabW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (tabW - 4 * millimeter) / 2, y2 - hcap),
                "secondCorner" : vector(x + (tabW - 4 * millimeter) / 2, y2 + hcap)
        });
        ki += 1;
        x = left + tabW + u / 2;
        for (var i = 0; i < 12; i += 1)
        {
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x - hcap, y2 - hcap),
                    "secondCorner" : vector(x + hcap, y2 + hcap)
            });
            ki += 1;
            x = x + u;
        }
        var bslW = kbW - tabW - 12 * u;
        x = left + tabW + 12 * u + bslW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (bslW - 4 * millimeter) / 2, y2 - hcap),
                "secondCorner" : vector(x + (bslW - 4 * millimeter) / 2, y2 + hcap)
        });
        ki += 1;

        // --- ROW 3: CapsLock(1.75u) ASDFGHJKL;' Enter ---
        var y3 = y2 - u;
        var capsW = 1.75 * u;
        x = left + capsW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (capsW - 4 * millimeter) / 2, y3 - hcap),
                "secondCorner" : vector(x + (capsW - 4 * millimeter) / 2, y3 + hcap)
        });
        ki += 1;
        x = left + capsW + u / 2;
        for (var i = 0; i < 11; i += 1)
        {
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x - hcap, y3 - hcap),
                    "secondCorner" : vector(x + hcap, y3 + hcap)
            });
            ki += 1;
            x = x + u;
        }
        var entW = kbW - capsW - 11 * u;
        x = left + capsW + 11 * u + entW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (entW - 4 * millimeter) / 2, y3 - hcap),
                "secondCorner" : vector(x + (entW - 4 * millimeter) / 2, y3 + hcap)
        });
        ki += 1;

        // --- ROW 4: LShift(2.25u) ZXCVBNM,./ RShift ---
        var y4 = y3 - u;
        var lshW = 2.25 * u;
        x = left + lshW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (lshW - 4 * millimeter) / 2, y4 - hcap),
                "secondCorner" : vector(x + (lshW - 4 * millimeter) / 2, y4 + hcap)
        });
        ki += 1;
        x = left + lshW + u / 2;
        for (var i = 0; i < 10; i += 1)
        {
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x - hcap, y4 - hcap),
                    "secondCorner" : vector(x + hcap, y4 + hcap)
            });
            ki += 1;
            x = x + u;
        }
        var rshW = kbW - lshW - 10 * u;
        x = left + lshW + 10 * u + rshW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (rshW - 4 * millimeter) / 2, y4 - hcap),
                "secondCorner" : vector(x + (rshW - 4 * millimeter) / 2, y4 + hcap)
        });
        ki += 1;

        // --- ROW 5: Fn Ctrl Win Alt [SPACE] Alt PrtSc Ctrl ---
        // Then nav cluster on right: PgUp/Up/PgDn (top half), Left/Down/Right (bottom half)
        var y5 = y4 - u;

        // Left keys: Fn(1u) Ctrl(1.25u) Win(1u) Alt(1.25u)
        var r5L = [1.0, 1.25, 1.0, 1.25];
        x = left;
        var leftTotal = 0 * millimeter;
        for (var i = 0; i < 4; i += 1)
        {
            var kw = r5L[i] * u;
            var kc = kw - 4 * millimeter;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x + 2 * millimeter, y5 - hcap),
                    "secondCorner" : vector(x + kw - 2 * millimeter, y5 + hcap)
            });
            ki += 1;
            x = x + kw;
            leftTotal = leftTotal + kw;
        }

        // Right keys: Alt(1.25u) PrtSc(1u) Ctrl(1.25u)
        var r5R = [1.25, 1.0, 1.25];
        var rightModTotal = 0 * millimeter;
        for (var i = 0; i < 3; i += 1)
        {
            rightModTotal = rightModTotal + r5R[i] * u;
        }

        // Spacebar: fills between left mods and right mods (no nav in this row)
        var rightTotal = rightModTotal;
        var spaceW = kbW - leftTotal - rightTotal;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x + 2 * millimeter, y5 - hcap),
                "secondCorner" : vector(x + spaceW - 2 * millimeter, y5 + hcap)
        });
        ki += 1;
        x = x + spaceW;

        // Right modifier keys: Alt PrtSc Ctrl
        for (var i = 0; i < 3; i += 1)
        {
            var kw = r5R[i] * u;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x + 2 * millimeter, y5 - hcap),
                    "secondCorner" : vector(x + kw - 2 * millimeter, y5 + hcap)
            });
            ki += 1;
            x = x + kw;
        }

        // Nav cluster: in the RIGHT PROTRUSION below main body
        // 2 cols x 2 rows of small half-height keys
        var navPitch = 14 * millimeter; // smaller pitch for nav keys
        var navKeyW = 11 * millimeter;
        var navKeyH = 4 * millimeter; // very short keys
        var navCenterX = navInnerX + navExtW / 2;
        var navRow1Y = botMain - 2 * millimeter - navKeyH / 2;
        var navRow2Y = navRow1Y - navKeyH - 1.5 * millimeter;

        // Top row: PgUp, Up, PgDn
        for (var i = 0; i < 3; i += 1)
        {
            var nx = navCenterX + (i - 1) * navPitch;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(nx - navKeyW / 2, navRow1Y - navKeyH / 2),
                    "secondCorner" : vector(nx + navKeyW / 2, navRow1Y + navKeyH / 2)
            });
            ki += 1;
        }

        // Bottom row: Left, Down, Right
        for (var i = 0; i < 3; i += 1)
        {
            var nx = navCenterX + (i - 1) * navPitch;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(nx - navKeyW / 2, navRow2Y - navKeyH / 2),
                    "secondCorner" : vector(nx + navKeyW / 2, navRow2Y + navKeyH / 2)
            });
            ki += 1;
        }

        skSolve(ks);

        // Extrude all keycaps
        opExtrude(context, id + "ek", {
                "entities" : qSketchRegion(id + "ks"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : keyH
        });
        try { opFillet(context, id + "kf", {
                "entities" : qCreatedBy(id + "ek", EntityType.EDGE),
                "radius" : 0.3 * millimeter
        }); }

        // ==========================================
        // 3. TRACKPOINT (between G, H, B keys)
        // ==========================================
        var tpX = left + capsW + 5.5 * u;
        var tpY = (y3 + y4) / 2;

        // Square stem 4x4mm
        var ss = newSketchOnPlane(context, id + "ss", {
                "sketchPlane" : plane(vector(0, 0, 0) * meter, vector(0, 0, 1))
        });
        skRectangle(ss, "stem", {
                "firstCorner" : vector(tpX - 2 * millimeter, tpY - 2 * millimeter),
                "secondCorner" : vector(tpX + 2 * millimeter, tpY + 2 * millimeter)
        });
        skSolve(ss);
        opExtrude(context, id + "es", {
                "entities" : qSketchRegion(id + "ss"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : plateThk + keyH
        });

        // Dome cap via loft (7.6mm dia base, tapers to 5mm top, 3mm tall)
        var capZ = plateThk + keyH;
        var cbs = newSketchOnPlane(context, id + "cbs", {
                "sketchPlane" : plane(vector(0, 0, capZ), vector(0, 0, 1))
        });
        skCircle(cbs, "cb", { "center" : vector(tpX, tpY), "radius" : 3.8 * millimeter });
        skSolve(cbs);

        var cts = newSketchOnPlane(context, id + "cts", {
                "sketchPlane" : plane(vector(0, 0, capZ + 3 * millimeter), vector(0, 0, 1))
        });
        skCircle(cts, "ct", { "center" : vector(tpX, tpY), "radius" : 2.5 * millimeter });
        skSolve(cts);

        try { opLoft(context, id + "lc", {
                "profileSubqueries" : [qSketchRegion(id + "cbs"), qSketchRegion(id + "cts")]
        }); }
        try { opFillet(context, id + "cf", {
                "entities" : qCreatedBy(id + "lc", EntityType.EDGE),
                "radius" : 1.0 * millimeter
        }); }

        // TrackPoint hole through plate
        var hs = newSketchOnPlane(context, id + "hs", {
                "sketchPlane" : plane(vector(0, 0, -0.1 * millimeter), vector(0, 0, 1))
        });
        skCircle(hs, "th", { "center" : vector(tpX, tpY), "radius" : 3 * millimeter });
        skSolve(hs);
        opExtrude(context, id + "eh", {
                "entities" : qSketchRegion(id + "hs"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : plateThk + 0.2 * millimeter
        });
        try { opBoolean(context, id + "bh", {
                "tools" : qCreatedBy(id + "eh", EntityType.BODY),
                "targets" : qCreatedBy(id + "eb", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION
        }); }

        // ==========================================
        // 4. TRACKPOINT 3 BUTTONS (in center extension)
        // ==========================================
        var btnY = botMain - btnGapAbove - btnStripH / 2;
        var btnThk = 0.6 * millimeter;

        var bLW = 25 * millimeter;
        var bMW = 15 * millimeter;
        var bRW = 25 * millimeter;
        var bGap = 1.0 * millimeter;
        var bTotal = bLW + bMW + bRW + 2 * bGap;
        var bStart = -bTotal / 2;
        var bHalfH = btnStripH / 2;

        var bks = newSketchOnPlane(context, id + "bks", {
                "sketchPlane" : plane(vector(0, 0, plateThk), vector(0, 0, 1))
        });
        skRectangle(bks, "bl", {
                "firstCorner" : vector(bStart, btnY - bHalfH),
                "secondCorner" : vector(bStart + bLW, btnY + bHalfH)
        });
        skRectangle(bks, "bm", {
                "firstCorner" : vector(bStart + bLW + bGap, btnY - bHalfH),
                "secondCorner" : vector(bStart + bLW + bGap + bMW, btnY + bHalfH)
        });
        skRectangle(bks, "br", {
                "firstCorner" : vector(bStart + bLW + bGap + bMW + bGap, btnY - bHalfH),
                "secondCorner" : vector(bStart + bLW + bGap + bMW + bGap + bRW, btnY + bHalfH)
        });
        skSolve(bks);

        opExtrude(context, id + "ebt", {
                "entities" : qSketchRegion(id + "bks"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : btnThk
        });

        // ==========================================
        // 5. SCREW HOLES in mounting bar area
        // ==========================================
        var mountY = botMain - btnGapAbove - btnStripH - mountGap - mountBarH / 2;

        var shs = newSketchOnPlane(context, id + "shs", {
                "sketchPlane" : plane(vector(0, 0, -0.1 * millimeter), vector(0, 0, 1))
        });
        skCircle(shs, "sl", { "center" : vector(-47.5 * millimeter, mountY), "radius" : 1.5 * millimeter });
        skCircle(shs, "sr", { "center" : vector(47.5 * millimeter, mountY), "radius" : 1.5 * millimeter });
        skSolve(shs);

        opExtrude(context, id + "esh", {
                "entities" : qSketchRegion(id + "shs"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : plateThk + 0.2 * millimeter
        });
        try { opBoolean(context, id + "bsh", {
                "tools" : qCreatedBy(id + "esh", EntityType.BODY),
                "targets" : qCreatedBy(id + "eb", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION
        }); }

        // ==========================================
        // 6. UNION ALL
        // ==========================================
        try { opBoolean(context, id + "ua", {
                "tools" : qAllModifiableSolidBodies(),
                "operationType" : BooleanOperationType.UNION
        }); }
    });
