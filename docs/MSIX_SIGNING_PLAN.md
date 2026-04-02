# MSIX Packaging + Code Signing — Research & Plan

## Status: Deferred

Not proceeding now. Kept for reference in case the project gains enough community traction to qualify for SignPath Foundation, or if budget for a paid cert becomes available.

---

## Why We Investigated This

Current distribution: unpackaged WinUI 3 app (zip). Users must:

1. Unblock the zip (Properties → Unblock)
2. Install Windows App SDK runtime separately
3. Click through SmartScreen ("More info → Run anyway")

Goals were to eliminate the runtime prerequisite and the SmartScreen warning via MSIX + code signing.

---

## What We Learned About Self-Contained Deployment

Tried `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` in `Deduplicate.csproj`.

**Result: crash on startup** — `STATUS_STOWED_EXCEPTION` (0xC000027B) in `combase.dll`.

**Root cause:** `WindowsAppSDKSelfContained=true` uses manifest-based WinRT activation instead of bootstrap (`MddBootstrapInitialize`). For this to work, the DDLM (Dynamic Dependency Lifetime Manager) COM server must be present. In an unpackaged app on .NET 9, it isn't — the manifest-based path is only production-ready for .NET 6 packaged apps.

**Conclusion:** No config, flag, or NuGet package fixes this for `net9.0` + `WindowsPackageType=None`. The flag was reverted.

---

## What We Learned About MSIX Packaging

MSIX packaging **would work** and solves both problems:

- WinAppSDK runtime is declared as a framework dependency in the manifest; Windows handles it automatically
- MSIX apps can be signed (required for user-facing distribution)

Key facts about WinUI 3 MSIX:

- Full-trust Desktop Bridge app — **no file system restrictions** (not a UWP sandbox)
- Proper "Apps & Features" uninstall entry
- Works fine with .NET 9 (it's the primary deployment model for WinUI 3)
- Users install via double-clicking an `.msix` file
- **Requires signing** — unsigned MSIX cannot be installed without Developer Mode

MSIX manifest requirements:

- `Identity Publisher` must exactly match the signing certificate's Subject string
- Version must be 4-part: `0.1.9` → `0.1.9.0`
- WinAppSDK declared as `<PackageDependency>` in manifest

---

## What We Learned About Code Signing Options

| Option | Cost | Verdict |
| --- | --- | --- |
| Paid EV cert (DigiCert, Sectigo) | ~$200-400/yr | Not worth it for personal project |
| Self-signed cert | Free | Worse UX: users must install cert manually in Trusted Publishers |
| SignPath Foundation | Free for OSS | See below — not viable right now |
| Microsoft Store | Free | Microsoft signs it; own review process |

---

## SignPath Foundation — Full Research

**What it is:** Free code signing for OSS projects. Certificate issued to "SignPath Foundation" (not to you). SignPath verifies every build comes from source code via CI.

**Requirements page:** [https://signpath.org/terms](https://signpath.org/terms)

**Note:** signpath.org = SignPath Foundation (free OSS program). signpath.io = the commercial signing service. The free OSS program lives entirely on signpath.org.

**Requirements we checked:**

| Requirement | Status |
| --- | --- |
| OSI-approved license (no NC/commercial restrictions) | Fixed — changed to MIT |
| No malware | ✓ |
| No proprietary code | ✓ |
| Actively maintained | ✓ |
| Already released | ✓ |
| Documented on homepage | ✓ |
| No hacking tools | ✓ |
| Respect user privacy | ✓ |
| Announce system changes (delete confirmation) | ✓ |
| Provide uninstallation (MSIX handles this) | would be ✓ with MSIX |
| MFA on GitHub + SignPath | needs verification |
| Code signing policy section on homepage | would need adding to README |
| Automated CI build (binaries from source) | ✓ GitHub Actions |
| Artifact metadata config (product name/version enforced) | done in SignPath dashboard |

**The blocker — Reputation field:**

> "we cannot sign binaries based on source code that nobody knows. For executable programs that may be downloaded and executed based on our signature, we require a certain verifiable reputation."

They look for: download counts, stars, forks, blog articles, media coverage, Wikipedia, GitHub Insights.

**Why we didn't apply:** Project was personal/learning-driven, no community, no stars, no forks. Submitting would likely result in rejection, and they'd be right to do so — handing their certificate name to a zero-reputation vibe-coded app would risk their reputation with every future applicant.

**When this becomes viable:**

- Significant download numbers (hundreds per release minimum)
- GitHub stars / forks showing real community use
- At least one external mention (blog post, Softpedia listing, etc.)

**Technical pipeline steps (if applying in future):**

1. Add "Code Signing Policy" section to README:
   - Text: `"Free code signing provided by SignPath.io, certificate by SignPath Foundation"`
   - Team roles (even solo: Author/Reviewer/Approver = same person)
   - Privacy statement: `"This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it"`
2. Register at signpath.io → apply for Open Source program at signpath.org
3. After approval: get exact Publisher string from their certificate
4. Add `Package.appxmanifest` with correct publisher identity
5. Add `signpath/github-action-submit-signing-request@v1` to release workflow
6. Add secrets to GitHub repo: `SIGNPATH_API_TOKEN`, `SIGNPATH_ORG_ID`

**GitHub Actions signing step (for reference):**

```yaml
- name: Sign MSIX via SignPath
  uses: signpath/github-action-submit-signing-request@v1
  with:
    api-token: ${{ secrets.SIGNPATH_API_TOKEN }}
    organization-id: ${{ vars.SIGNPATH_ORG_ID }}
    signing-policy-slug: release-signing
    artifact-configuration-slug: initial-config
    github-artifact-id: ${{ steps.upload-msix.outputs.artifact-id }}
    wait-for-completion: true
    output-artifact-directory: ./signed/
```

---

## Decision

**Stay on current zip distribution for now.** It works. SmartScreen is a one-time friction for a niche tool's audience (GitHub users who know what they're downloading). The WinAppSDK runtime prerequisite stays in the README.

Revisit if:

- Project gains real download/community numbers → SignPath Foundation becomes viable
- Budget exists for a paid cert → MSIX immediately viable
- Want Store distribution → Microsoft Store path (different process)
