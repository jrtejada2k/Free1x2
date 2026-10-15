// swift-tools-version: 6.2
// Free1X2Kit: todo lo que no es interfaz (docs/mac/ESPEC_11_ARQUITECTURA_SWIFT.md §3).
// Grafo acíclico: CasosDeUso → Formatos → Motor → Base; CasosDeUso → Online → Base.
import PackageDescription

// Swift 6 estricto en todos los targets (ESPEC_11 §3.3).
let ajustes: [SwiftSetting] = [
    .enableUpcomingFeature("ExistentialAny"),
    .enableUpcomingFeature("NonisolatedNonsendingByDefault"),
]

let package = Package(
    name: "Free1X2Kit",
    platforms: [.macOS(.v14)],
    products: [
        // La app solo ve casos de uso y tipos base; importar Free1X2Motor está prohibido (ESPEC_11 §3.2).
        .library(name: "Free1X2Kit", targets: ["Free1X2CasosDeUso", "Free1X2Base"])
    ],
    // ponytail: swift-collections (único tercero permitido) y Free1X2Bench se añaden con su primer uso en M3.
    targets: [
        .target(name: "Free1X2Base", swiftSettings: ajustes),
        .target(name: "Free1X2Motor", dependencies: ["Free1X2Base"], swiftSettings: ajustes),
        .target(name: "Free1X2Formatos", dependencies: ["Free1X2Base", "Free1X2Motor"], swiftSettings: ajustes),
        .target(name: "Free1X2Online", dependencies: ["Free1X2Base"], swiftSettings: ajustes),
        .target(
            name: "Free1X2CasosDeUso",
            dependencies: ["Free1X2Base", "Free1X2Motor", "Free1X2Formatos", "Free1X2Online"],
            swiftSettings: ajustes
        ),
        // Dobles de prueba y oráculo por lectura; los datos van como recursos del propio target (DT-28).
        .target(
            name: "Free1X2Pruebas",
            dependencies: ["Free1X2CasosDeUso"],
            resources: [.copy("Golden"), .copy("Fixtures")],
            swiftSettings: ajustes
        ),
        .testTarget(name: "Free1X2BaseTests", dependencies: ["Free1X2Base", "Free1X2Pruebas"], swiftSettings: ajustes),
        .testTarget(name: "Free1X2MotorTests", dependencies: ["Free1X2Motor", "Free1X2Pruebas"], swiftSettings: ajustes),
        .testTarget(
            name: "Free1X2FormatosTests",
            dependencies: ["Free1X2Formatos", "Free1X2Motor", "Free1X2Pruebas"],
            swiftSettings: ajustes
        ),
        .testTarget(name: "Free1X2OnlineTests", dependencies: ["Free1X2Online", "Free1X2Pruebas"], swiftSettings: ajustes),
        .testTarget(
            name: "Free1X2CasosDeUsoTests",
            dependencies: ["Free1X2CasosDeUso", "Free1X2Base", "Free1X2Pruebas"],
            swiftSettings: ajustes
        ),
    ],
    swiftLanguageModes: [.v6]
)
