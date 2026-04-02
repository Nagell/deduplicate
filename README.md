<a id="readme-top"></a>

<!-- PROJECT SHIELDS -->
[![MIT][license-shield]][license-url]
[![.NET 9][net-shield]][net-url]
[![Windows App SDK][winSDK-shield]][winSDK-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<div align="center">
  <a href="https://github.com/Nagell/deduplicate">
    <img src="./docs/logo.svg" alt="Logo" width="80" height="80">
  </a>
  <h3 align="center">deduplicate</h3>
  <p align="center">
    A fast, native Windows 11 duplicate file finder — no installation required
    <br />
    <a href="./docs/DEVELOPMENT.md"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Nagell/deduplicate/issues/new?labels=bug">Report Bug</a>
    ·
    <a href="https://github.com/Nagell/deduplicate/issues/new?labels=enhancement">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://github.com/Nagell/deduplicate)

A native Windows 11 duplicate file finder built with WinUI 3. Scan a folder with one of three detection methods, review grouped results, and bulk-delete duplicates — all without installing anything on the end user's machine.

**Detection methods:**

- **Quick** *(recommended)* — groups by filename + file size, instant results
- **Smart** — pre-filters by size, then confirms with SHA-256 hash
- **Full Hash** — SHA-256 hashes every file regardless of size

Hash-based scans show live progress (MB/s + ETA) and can be cancelled at any time, returning the partial results found so far.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

- [![WinUI 3][WinUI-shield]][WinUI-url]
- [![.NET 9][net-shield]][net-url]
- [![C#][csharp-shield]][csharp-url]
- [![Windows App SDK][winSDK-shield]][winSDK-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

**Building from source:**

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Windows 10 version 1809 or later (Windows 11 recommended for Mica backdrop)

**Running a release build:**

- Windows 10 version 1809 or later
- [Windows App SDK runtime](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads) — required for WinUI 3; .NET 9 is bundled in the zip and does not need a separate install

### Installation

1. Clone the repository

   ```sh
   git clone https://github.com/Nagell/deduplicate.git
   ```

2. Restore dependencies

   ```sh
   dotnet restore
   ```

3. Build the project

   ```sh
   dotnet build
   ```

4. Run the application

   ```sh
   dotnet run
   ```

> No MSIX packaging or Visual Studio required — the app runs unpackaged directly from the build output.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

- [x] WinUI 3 unpackaged app (no installer for end users)
- [x] Quick scan — name + size matching
- [x] Smart scan — size pre-filter + SHA-256 confirmation
- [x] Full SHA-256 scan
- [x] Recursive scanning with toggle
- [x] Cancellable hash scans with live progress (MB/s + ETA)
- [x] Partial results returned on cancel
- [x] Grouped results view
- [x] Open File / Open Folder buttons per result
- [x] Per-file checkboxes with bulk selection
- [x] Delete selected with confirmation dialog
- [x] Warning when all copies of a group are selected
- [x] Mica backdrop + custom title bar (Windows 11)
- [x] Error handling with user-facing messages
- [x] Status bar showing group count, extra files, and reclaimable space
- [x] Selection statistics footer (files selected · space to free)
- [ ] Export results to CSV
- [ ] Exclusion patterns (glob / regex)
- [ ] Minimum file size filter
- [ ] Move to folder instead of delete

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Dawid Nitka — [LinkedIn](https://www.linkedin.com/in/dawidnitka)

Project Link: [https://github.com/Nagell/deduplicate](https://github.com/Nagell/deduplicate)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[license-shield]: https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge
[license-url]: ./LICENSE
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/dawidnitka
[product-screenshot]: /docs/screenshot.png

[net-shield]: https://img.shields.io/badge/.NET-9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[net-url]: https://dotnet.microsoft.com/
[winSDK-shield]: https://img.shields.io/badge/Windows%20App%20SDK-1.6-0078D4?style=for-the-badge&logo=windows&logoColor=white
[winSDK-url]: https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/
[WinUI-shield]: https://img.shields.io/badge/WinUI%203-0078D4?style=for-the-badge&logo=windows&logoColor=white
[WinUI-url]: https://learn.microsoft.com/en-us/windows/apps/winui/winui3/
[csharp-shield]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white
[csharp-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
