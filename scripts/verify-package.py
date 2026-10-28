from pathlib import Path
import re
import subprocess
import tempfile
import zipfile

ROOT = Path(__file__).resolve().parents[1]
packages = list((ROOT / "artifacts/nuget").glob("Kapusch.PostHog.iOS.*.nupkg"))
assert len(packages) == 1, "Expected one qualification package"
package = packages[0]
version = package.name.removeprefix("Kapusch.PostHog.iOS.").removesuffix(".nupkg")

# The sample restores the packed package by exact version; a stale reference would test an old cache entry.
sample = (ROOT / "samples/Smoke/Smoke.csproj").read_text(encoding="utf-8")
assert f'Include="Kapusch.PostHog.iOS" Version="{version}"' in sample, f"Sample must reference {version}"

# Every managed DllImport entry point must be exported by each native slice.
managed = (ROOT / "src/Kapusch.PostHogApisForiOSComponents/NativePostHog.cs").read_text(encoding="utf-8")
entry_points = set(re.findall(r'EntryPoint = "(kph_\w+)"', managed))
assert entry_points, "No native entry points found"

with zipfile.ZipFile(package) as archive:
 names = archive.namelist()
 assert any(n.startswith("buildTransitive/") and n.endswith(".targets") for n in names)
 assert "THIRD_PARTY_NOTICES.md" in names
 assert any(n.startswith("licenses/") for n in names)
 assert any(n.startswith("lib/") and n.endswith(".dll") for n in names)
 libraries = [n for n in names if n.startswith("native/kposthog.xcframework/") and n.endswith(".a")]
 assert len(libraries) == 2, f"Expected device and simulator slices, found {libraries}"
 assert sum(n.endswith("PrivacyInfo.xcprivacy") for n in names) >= 2
 assert not any("/spm/" in n or "/.gradle/" in n for n in names)
 with tempfile.TemporaryDirectory() as temp:
  for library in libraries:
   path = archive.extract(library, temp)
   symbols = subprocess.check_output(["nm", "-gU", path], text=True, stderr=subprocess.DEVNULL)
   exported = set(re.findall(r" T _(kph_\w+)$", symbols, re.MULTILINE))
   missing = entry_points - exported
   assert not missing, f"{library} lacks {sorted(missing)}"
print(f"Package layout verified ({version}, {len(entry_points)} native entry points)")
