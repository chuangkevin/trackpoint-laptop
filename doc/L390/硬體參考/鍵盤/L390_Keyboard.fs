FeatureScript 2909;
import(path : "onshape/std/common.fs", version : "2909.0");

annotation { "Feature Type Name" : "L390 Keyboard" }
export const l390Keyboard = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        // ============================================================
        // ThinkPad L390 鍵盤 CAD 模型
        // 照片量測數據：
        //   - 總寬 300mm (Photo1 尺確認)
        //   - 主體高 ~124mm (鍵區111mm + 上邊框9mm + 下邊框4mm)
        //   - 長寬比 306:124 ≈ 2.47:1
        //   - 側面厚度 ~4mm (Photo4 卡尺)
        //   - 輪廓：矩形主體 + 中央底部延伸 + 右側底部延伸
        //   - 中央延伸 92mm寬 × 15mm深 (TP按鍵+安裝座)
        //   - 右側延伸 44mm寬 × 13mm深 (方向鍵，與右邊共邊)
        // ============================================================

        // === 基本單位 ===
        var u = 19 * millimeter;
        var cap = 15 * millimeter;
        var hcap = cap / 2;
        var plateThk = 1.5 * millimeter;
        var keyH = 1.6 * millimeter;

        // 功能鍵尺寸
        var fw = 14 * millimeter;
        var fh = 12 * millimeter;
        var hfw = fw / 2;
        var hfh = fh / 2;

        // === 主體尺寸 ===
        var kbW = 300 * millimeter;
        var fnRowH = 16 * millimeter;
        var mainRowsH = 5 * u;             // 95mm
        var mainH = fnRowH + mainRowsH;    // 111mm (鍵區淨高)

        // 邊框 (照片觀察)
        var topMargin = 9 * millimeter;    // Esc 上方
        var botMargin = 4 * millimeter;    // 底列下方
        var sideMargin = 3 * millimeter;

        // === 中央延伸 (TP按鍵 + 安裝座) ===
        var btnStripH = 5 * millimeter;    // TrackPoint 三鍵高度
        var btnGapAbove = 1 * millimeter;
        var mountBarH = 8 * millimeter;    // 螺絲安裝條
        var mountGap = 1 * millimeter;
        var centerExtH = btnGapAbove + btnStripH + mountGap + mountBarH; // 15mm
        var centerExtW = 92 * millimeter;

        // === 右側延伸 (方向鍵，與右邊共邊) ===
        var navExtW = 44 * millimeter;
        var navExtH = 13 * millimeter;     // 較中央淺

        // === 座標系: 原點在鍵區中心 ===
        var halfW = kbW / 2;
        var halfH = mainH / 2;

        // ==========================================
        // 1. 底板輪廓
        // ==========================================
        var bs = newSketchOnPlane(context, id + "bs", {
                "sketchPlane" : plane(vector(0, 0, 0) * meter, vector(0, 0, 1))
        });

        // 關鍵座標
        var topY = halfH + topMargin;               // 頂邊
        var botMain = -halfH - botMargin;            // 主體底邊
        var botCenter = botMain - centerExtH;        // 中央延伸底 (最深)
        var botNav = botMain - navExtH;              // 右側延伸底 (較淺)
        var leftX = -halfW - sideMargin;             // 左邊
        var rightX = halfW + sideMargin;             // 右邊
        var navInnerX = rightX - navExtW;            // 右延伸內側 X
        var halfCenterW = centerExtW / 2;            // 中央延伸半寬

        //  A ────────────────────────────── B
        //  │         主體矩形               │
        //  J──I                       F──E  │
        //     │    中央延伸           │     │
        //     K───────────────────────L  G──D
        //                                 (nav)

        skPolyline(bs, "outline", {
                "points" : [
                    vector(leftX, topY),                     // A 左上
                    vector(rightX, topY),                    // B 右上
                    vector(rightX, botNav),                  // D 右邊延伸到 nav 底
                    vector(navInnerX, botNav),               // G nav 左下角
                    vector(navInnerX, botMain),              // F 回到主體底邊
                    vector(halfCenterW, botMain),            // E→ 中央延伸右上
                    vector(halfCenterW, botCenter),          // L 中央延伸右下
                    vector(-halfCenterW, botCenter),         // K 中央延伸左下
                    vector(-halfCenterW, botMain),           // I 中央延伸左上
                    vector(leftX, botMain),                  // J 主體左下
                    vector(leftX, topY)                      // A 閉合
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
        // 2. 鍵帽
        // ==========================================
        var ks = newSketchOnPlane(context, id + "ks", {
                "sketchPlane" : plane(vector(0, 0, plateThk), vector(0, 0, 1))
        });
        var ki = 0;
        var left = -halfW;

        // --- 第 0 列: 功能鍵 (Esc + F1-F12 + Home/End/Ins/Del) ---
        var y0 = halfH - fnRowH / 2;
        var fkPitch = fw + 1 * millimeter;   // 15mm
        var groupGap = 4 * millimeter;
        var x = left + 8 * millimeter + hfw;

        // Esc
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - hfw, y0 - hfh),
                "secondCorner" : vector(x + hfw, y0 + hfh)
        });
        ki += 1;
        x = x + fkPitch + groupGap;

        // 4 組 × 4 鍵
        for (var g = 0; g < 4; g += 1)
        {
            for (var i = 0; i < 4; i += 1)
            {
                skRectangle(ks, "k" ~ ki, {
                        "firstCorner" : vector(x - hfw, y0 - hfh),
                        "secondCorner" : vector(x + hfw, y0 + hfh)
                });
                ki += 1;
                x = x + fkPitch;
            }
            if (g < 3) { x = x + groupGap; }
        }

        // --- 第 1 列: ` 1-0 - = Backspace ---
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
        x = left + 13 * u + bsW / 2;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x - (bsW - 4 * millimeter) / 2, y1 - hcap),
                "secondCorner" : vector(x + (bsW - 4 * millimeter) / 2, y1 + hcap)
        });
        ki += 1;

        // --- 第 2 列: Tab(1.5u) QWERTYUIOP[]\ ---
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

        // --- 第 3 列: CapsLock(1.75u) ASDFGHJKL;' Enter ---
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

        // --- 第 4 列: LShift(2.25u) ZXCVBNM,./ RShift ---
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

        // --- 第 5 列: Fn Ctrl Win Alt [Space] Alt PrtSc Ctrl ---
        var y5 = y4 - u;
        var r5L = [1.0, 1.25, 1.0, 1.25];
        x = left;
        var leftTotal = 0 * millimeter;
        for (var i = 0; i < 4; i += 1)
        {
            var kw = r5L[i] * u;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(x + 2 * millimeter, y5 - hcap),
                    "secondCorner" : vector(x + kw - 2 * millimeter, y5 + hcap)
            });
            ki += 1;
            x = x + kw;
            leftTotal = leftTotal + kw;
        }

        var r5R = [1.25, 1.0, 1.25];
        var rightModTotal = 0 * millimeter;
        for (var i = 0; i < 3; i += 1)
        {
            rightModTotal = rightModTotal + r5R[i] * u;
        }

        var spaceW = kbW - leftTotal - rightModTotal;
        skRectangle(ks, "k" ~ ki, {
                "firstCorner" : vector(x + 2 * millimeter, y5 - hcap),
                "secondCorner" : vector(x + spaceW - 2 * millimeter, y5 + hcap)
        });
        ki += 1;
        x = x + spaceW;

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

        // --- 方向鍵 (右側延伸區) ---
        var navPitch = 13 * millimeter;
        var navKeyW = 10 * millimeter;
        var navKeyH = 4.5 * millimeter;
        var navCenterX = navInnerX + navExtW / 2;
        var navRow1Y = botMain - 2 * millimeter - navKeyH / 2;
        var navRow2Y = navRow1Y - navKeyH - 1.5 * millimeter;

        // 上排: PgUp ↑ PgDn
        for (var i = 0; i < 3; i += 1)
        {
            var nx = navCenterX + (i - 1) * navPitch;
            skRectangle(ks, "k" ~ ki, {
                    "firstCorner" : vector(nx - navKeyW / 2, navRow1Y - navKeyH / 2),
                    "secondCorner" : vector(nx + navKeyW / 2, navRow1Y + navKeyH / 2)
            });
            ki += 1;
        }
        // 下排: ← ↓ →
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
        // 3. TrackPoint (G/H/B 之間)
        // ==========================================
        var tpX = left + capsW + 5.5 * u;
        var tpY = (y3 + y4) / 2;

        // 方形底座
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

        // 圓頂帽 (loft: 底 ø7.6 → 頂 ø5, 高 3mm)
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

        // TrackPoint 穿孔
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
        // 4. TrackPoint 三鍵 (中央延伸區)
        // ==========================================
        var btnY = botMain - btnGapAbove - btnStripH / 2;
        var btnThk = 0.6 * millimeter;
        var bLW = 25 * millimeter;
        var bMW = 15 * millimeter;
        var bRW = 25 * millimeter;
        var bGap = 1.0 * millimeter;
        var bTotal = bLW + bMW + bRW + 2 * bGap;
        var bStart = -bTotal / 2;

        var bks = newSketchOnPlane(context, id + "bks", {
                "sketchPlane" : plane(vector(0, 0, plateThk), vector(0, 0, 1))
        });
        skRectangle(bks, "bl", {
                "firstCorner" : vector(bStart, btnY - btnStripH / 2),
                "secondCorner" : vector(bStart + bLW, btnY + btnStripH / 2)
        });
        skRectangle(bks, "bm", {
                "firstCorner" : vector(bStart + bLW + bGap, btnY - btnStripH / 2),
                "secondCorner" : vector(bStart + bLW + bGap + bMW, btnY + btnStripH / 2)
        });
        skRectangle(bks, "br", {
                "firstCorner" : vector(bStart + bLW + bGap + bMW + bGap, btnY - btnStripH / 2),
                "secondCorner" : vector(bStart + bLW + bGap + bMW + bGap + bRW, btnY + btnStripH / 2)
        });
        skSolve(bks);

        opExtrude(context, id + "ebt", {
                "entities" : qSketchRegion(id + "bks"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : btnThk
        });

        // ==========================================
        // 5. 螺絲孔 (安裝條)
        // ==========================================
        var mountY = botMain - btnGapAbove - btnStripH - mountGap - mountBarH / 2;
        var shs = newSketchOnPlane(context, id + "shs", {
                "sketchPlane" : plane(vector(0, 0, -0.1 * millimeter), vector(0, 0, 1))
        });
        skCircle(shs, "sl", { "center" : vector(-40 * millimeter, mountY), "radius" : 1.5 * millimeter });
        skCircle(shs, "sr", { "center" : vector(40 * millimeter, mountY), "radius" : 1.5 * millimeter });
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
        // 6. 合併
        // ==========================================
        try { opBoolean(context, id + "ua", {
                "tools" : qAllModifiableSolidBodies(),
                "operationType" : BooleanOperationType.UNION
        }); }
    });
