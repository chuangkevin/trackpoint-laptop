# ThrottleStop 省電設定 - Lenovo L390 (i5-8365U)

使用 ThrottleStop 9.6 限制功耗以提升電池續航。

## 設定總覽

| 項目 | 設定值 | 效果 |
|------|--------|------|
| Disable Turbo | 勾選 | 鎖定基頻 1.6GHz，避免 Turbo 飆功耗 |
| Speed Shift EPP | 255 | 最大省電，CPU 積極降頻 |
| FIVR CPU Core | -80.1 mV | 降壓減少功耗與溫度 |
| FIVR CPU Cache | -80.1 mV | 同上 |
| Turbo Ratio Limits | 1-2核 25x / 3-4核 20x | 限制 Turbo 最高頻率（Disable Turbo 時不生效）|

## 效果對比

| 指標 | 調整前 | 調整後 |
|------|--------|--------|
| CPU 功耗 | 4.4W (Max 15.7W) | 1.2W (Max 7.3W) |
| 溫度 | 57°C (Max 77°C) | 42°C (Max 52°C) |
| 頻率 | ~1.4 GHz | ~0.98 GHz |

## Profile 設定

使用 Battery profile（第 4 組），在 Options 中設定：

- AC Profile: 1
- Battery Profile: 4

切換 AC/電池時會自動套用對應 profile。

## FIVR 降壓設定步驟

1. 點 FIVR 按鈕
2. 上方選 **Battery** profile（第 4 個點）
3. 選 **CPU Core**
   - 勾選 Unlock Adjustable Voltage
   - 選 Adaptive
   - Offset Voltage 設為 **-80.1 mV**
4. 選 **CPU Cache**
   - 同樣設為 **-80.1 mV**
5. Intel GPU / iGPU Unslice / System Agent 維持預設不動
6. Save Voltage Changes 選「**OK - Save voltages immediately**」
7. 按 Apply → OK

> 穩定的話可逐步加大到 -100mV、-120mV，不穩定再退回。

## 已知限制

- **MSR 功耗限制被 BIOS 鎖定**：TPL 畫面中 MSR 的 PL1/PL2 有鎖頭圖示，ThrottleStop 無法修改。MMIO 顯示 51W，實際透過 Disable Turbo 和降壓來控制功耗。
- **Set Multiplier 無法勾選**：L390 BIOS 限制，改用 Disable Turbo 或 Turbo Ratio Limits 替代。
- **Limit Reasons 顯示 BD PROCHOT + THERMAL**：系統因溫度觸發降頻保護，屬正常行為。

## 開機自動啟動

ThrottleStop 需要管理員權限，使用工作排程器（Task Scheduler）設定：

1. 開啟 `taskschd.msc`
2. 建立基本工作，名稱填 `ThrottleStop`
3. 觸發程序：**當使用者登入時**
4. 動作：啟動程式 → `C:\Users\Kevin\Downloads\ThrottleStop_9.6\ThrottleStop.exe`
5. 內容中勾選「**以最高權限執行**」
6. 條件 → 取消勾選「僅在電腦使用 AC 電源時才啟動」

Options 中可勾選 **Start Minimized** 讓開機後自動縮到系統匣。
