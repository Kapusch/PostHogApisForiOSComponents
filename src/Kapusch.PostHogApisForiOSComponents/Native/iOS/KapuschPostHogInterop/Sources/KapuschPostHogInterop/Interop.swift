import Foundation
import PostHog

private func text(_ p: UnsafePointer<CChar>?) -> String? {
    guard let p else { return nil }; return String(validatingCString: p)
}
private func object(_ p: UnsafePointer<CChar>?) -> [String: Any]? {
    guard let s = text(p), let data = s.data(using: .utf8),
          let value = try? JSONSerialization.jsonObject(with: data) as? [String: Any] else { return nil }
    return value
}
@_cdecl("kph_setup")
public func setup(_ key: UnsafePointer<CChar>?, _ host: UnsafePointer<CChar>?, _ debug: Int32, _ lifecycle: Int32) -> Int32 {
    guard let key = text(key), key.hasPrefix("phc_"), let host = text(host),
          let url = URL(string: host), url.scheme == "https", url.host != nil else { return -1 }
    let config = PostHogConfig(projectToken: key, host: host)
    config.debug = debug != 0
    config.captureApplicationLifecycleEvents = lifecycle != 0
    config.captureScreenViews = false
    config.sessionReplay = false
    config.surveys = false
    config.capturePushNotificationSubscriptions = false
    config.capturePushNotificationOpened = false
    config.errorTrackingConfig.autoCapture = false
    config.preloadFeatureFlags = false
    config.flushAt = 20
    config.maxQueueSize = 1000
    config.flushIntervalSeconds = 5
    PostHogSDK.shared.setup(config)
    return 0
}
@_cdecl("kph_capture")
public func capture(_ event: UnsafePointer<CChar>?, _ properties: UnsafePointer<CChar>?) -> Int32 {
    guard let event = text(event), !event.isEmpty, let properties = object(properties) else { return -1 }
    PostHogSDK.shared.capture(event, properties: properties)
    return 0
}
@_cdecl("kph_identify")
public func identify(_ id: UnsafePointer<CChar>?) -> Int32 {
    guard let id = text(id), !id.isEmpty else { return -1 }
    PostHogSDK.shared.identify(id)
    return 0
}
@_cdecl("kph_reset") public func reset() { PostHogSDK.shared.reset() }
@_cdecl("kph_flush") public func flush() { PostHogSDK.shared.flush() }
@_cdecl("kph_opt_out") public func optOut() { PostHogSDK.shared.optOut() }
@_cdecl("kph_opt_in") public func optIn() { PostHogSDK.shared.optIn() }
@_cdecl("kph_distinct_id") public func distinctId() -> UnsafeMutablePointer<CChar>? {
    strdup(PostHogSDK.shared.getDistinctId())
}
@_cdecl("kph_free") public func freeString(_ p: UnsafeMutablePointer<CChar>?) { free(p) }
