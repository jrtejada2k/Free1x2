// C#: Free1X2.Domain/Utils/UtilColumnas.cs (06 §1; ESPEC_11 §4.4)

/// Columna de 3 bits por partido (`1` = 100, `X` = 010, `2` = 001), igual que el `long` del C#.
public typealias Columna = Int64

/// Índice en base 3 de `ConvertidorDeBases` (05 §7, trampa T2). Es otro número: nunca se mezcla con `Columna`.
public typealias IndiceBase3 = Int32
