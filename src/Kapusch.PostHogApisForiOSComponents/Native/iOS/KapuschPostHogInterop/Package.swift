// swift-tools-version: 5.9
import PackageDescription
let package = Package(name: "KapuschPostHogInterop", platforms: [.iOS(.v15)],
 products: [.library(name: "KapuschPostHogInterop", type: .static, targets: ["KapuschPostHogInterop"])],
 dependencies: [.package(url: "https://github.com/PostHog/posthog-ios.git", exact: "3.90.2")],
 targets: [.target(name: "KapuschPostHogInterop", dependencies: [.product(name: "PostHog", package: "posthog-ios")])])
