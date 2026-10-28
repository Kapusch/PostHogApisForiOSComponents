#!/usr/bin/env python3
"""Fail when the pinned PostHog iOS SDK version differs between the files that declare it.

DependencyLocks/posthog-ios.json is the source of truth. The SwiftPM manifest must pin the same
exact version, Package.resolved the same version and revision, and the package notices must name
the same version. Run it after every SDK bump; CI runs it before the native build.
"""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
NATIVE = ROOT / "src/Kapusch.PostHogApisForiOSComponents/Native/iOS/KapuschPostHogInterop"


def main() -> int:
    lock = json.loads((ROOT / "DependencyLocks/posthog-ios.json").read_text(encoding="utf-8"))
    version, revision, repository = lock["version"], lock["revision"], lock["repository"]
    errors = []

    if not re.fullmatch(r"\d+\.\d+\.\d+", version):
        errors.append(f"lock: {version!r} is not a stable x.y.z release")
    if not re.fullmatch(r"[0-9a-f]{40}", revision):
        errors.append("lock: revision is not a full commit SHA")

    manifest = (NATIVE / "Package.swift").read_text(encoding="utf-8")
    pins = re.findall(r'\.package\(url:\s*"([^"]+)",\s*exact:\s*"([^"]+)"\)', manifest)
    if pins != [(repository, version)]:
        errors.append(f"Package.swift: expected one exact pin {repository} {version}, found {pins}")

    resolved = json.loads((NATIVE / "Package.resolved").read_text(encoding="utf-8"))
    states = {pin["location"]: pin["state"] for pin in resolved["pins"]}
    if set(states) != {repository}:
        errors.append(f"Package.resolved: expected only {repository}, found {sorted(states)}")
    elif states[repository] != {"revision": revision, "version": version}:
        errors.append(f"Package.resolved: {states[repository]} differs from the lock")

    for name in ("THIRD_PARTY_NOTICES.md", "src/Kapusch.PostHogApisForiOSComponents/nuget-readme.md"):
        found = set(re.findall(r"PostHog iOS (\d+\.\d+\.\d+)", (ROOT / name).read_text(encoding="utf-8")))
        if found != {version}:
            errors.append(f"{name}: names PostHog iOS {sorted(found) or 'no version'}, lock is {version}")

    if errors:
        print("Dependency lock check failed:\n  " + "\n  ".join(errors), file=sys.stderr)
        return 1
    print(f"Dependency lock verified (posthog-ios {version} @ {revision[:12]})")
    return 0


if __name__ == "__main__":
    sys.exit(main())
