## Summary

- What does this change do?

## Checklist

- [ ] No secrets, real project keys or private diagnostics committed
- [ ] Public tree scan: `python3 scripts/verify-public.py`
- [ ] Dependency lock: `python3 scripts/verify-dependency-lock.py`
- [ ] Built wrapper: `bash src/Kapusch.PostHogApisForiOSComponents/Native/iOS/build.sh`
- [ ] Packed NuGet: `dotnet pack src/Kapusch.PostHogApisForiOSComponents/Kapusch.PostHogApisForiOSComponents.csproj -c Release -o artifacts/nuget`
- [ ] Verified package: `python3 scripts/verify-package.py`
- [ ] Sample built: `dotnet build samples/Smoke/Smoke.csproj -c Release -p:RuntimeIdentifier=iossimulator-x64`
- [ ] Updated docs and `CHANGELOG.md` if behavior/integration changed
