#pragma once

// Правит текстовый DXF/DWG: $INSUNITS = 4 (миллиметры), $MEASUREMENT = 1.
// Координаты не меняет. Двоичный DXF и двоичный DWG не трогает.
bool PatchDxfUnitsToMillimeters(const wchar_t* path);
