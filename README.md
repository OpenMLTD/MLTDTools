# MLTDTools (MiriTore)

Tools for [THE iDOLM@STER Million Live! Theater Days](https://millionlive.idolmaster.jp/theaterdays/) (MiriShita/ミリシタ). A sister project of [OpenCGSS/DereTore](https://github.com/OpenCGSS/DereTore).

[![AppVeyor](https://img.shields.io/appveyor/ci/hozuki/mltdtools.svg)](https://ci.appveyor.com/project/hozuki/mltdtools)
[![GitHub contributors](https://img.shields.io/github/contributors/OpenMLTD/MLTDTools.svg)](https://github.com/OpenMLTD/MLTDTools/graphs/contributors)
[![GitHub (pre-)release](https://img.shields.io/github/release/OpenMLTD/MLTDTools/all.svg)](https://github.com/OpenMLTD/MLTDTools/releases)
[![Release Date](https://img.shields.io/github/release-date-pre/OpenMLTD/MLTDTools)](https://img.shields.io/github/release-date-pre/OpenMLTD/MLTDTools)
[![Github All Releases](https://img.shields.io/github/downloads/OpenMLTD/MLTDTools/total.svg)](https://github.com/OpenMLTD/MLTDTools/releases)
[![HitCount](http://hits.dwyl.com/OpenMLTD/MLTDTools.svg)](http://hits.dwyl.com/OpenMLTD/MLTDTools)

[![Commit](https://img.shields.io/github/last-commit/OpenMLTD/MLTDTools)](https://github.com/OpenMLTD/MLTDTools/commits)
[![Commit Activity](https://img.shields.io/github/commit-activity/m/OpenMLTD/MLTDTools)](https://github.com/OpenMLTD/MLTDTools/graphs/commit-activity)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/OpenMLTD/MLTDTools/pulls)

**Downloads:**

- [Releases](https://github.com/OpenMLTD/MLTDTools/releases)
- [CI auto build](https://ci.appveyor.com/api/projects/hozuki/mltdtools/artifacts/miritore-appveyor-latest.zip) (unstable, but may contain latest fixes) (Windows)

Requires .NET Framework 4.7.2.

This repository:

- contains some common tools to explore possibilities.
- does not serve as a card/event/CG/commu database. If you want that please visit [ミリシタDB](http://imas.gamedbs.jp/mlth/).
- DOES NOT PROVIDE TOOLS FOR YOU TO CHEAT. **Cheating is not fun at all. It must be punished.**

For the Unity project for resource generation, please visit [PlatinumTheater](https://github.com/OpenMLTD/PlatinumTheater).
To preview animations (as in MVs) in Unity, please visit [LeadActress](https://github.com/OpenMLTD/LeadActress).

## Details

The projects include some utilities to investigate MLTD data structures.

**ACB Packer**

Similar to [AcbMaker](https://github.com/OpenCGSS/DereTore/tree/master/Apps/AcbMaker) in DereTore, but packs live music into MLTD variant of ACB format.

**HCA Decoder**

Similar to [Hca2Wav](https://github.com/OpenCGSS/DereTore/tree/master/Apps/Hca2Wav) in DereTore, but uses MLTD's HCA cipher by default.

**ExtractAcb**

Extracts ACB file from the song bundles (`song3_*.acb.unity3d`).

**MLTD Information Viewer**

View <del>resource manifest</del> (please use Manifest Tools instead), card information and costume information from MLTD database files.

**Manifest Tools**

A collection of manifest-related utilities: viewing, downloading, diff-ing, exporting, etc.

**MillionDance**

Export models, dance animations, morphs and camera motions to MMD equivalents. It is currently in alpha state.

[Manual](https://github.com/OpenMLTD/MLTDTools/wiki/MillionDance-Manual)

Output animation data can be played in either [MikuMikuDance](http://www.geocities.jp/higuchuu4/index_e.htm) (MMD) or [MikuMikuMoving](https://sites.google.com/site/mikumikumovingeng/) (MMM). But, the number of frames is huge due to technical limitations.
MMM is much faster reading motions with large amount of frames. *Don't feed MMD with motions converted in 60 fps. It simply hangs for a long time and then crashes.*

**MLTD Dance Viewer**

A simple viewer for MLTD dancing data, which allows directly reading model and motion
data extracted from MLTD.

Theoretically it can be applied to other Unity games, with a little modification...

*Requires OpenGL 4.0+*

**TDFacial**

Facial expression mapping editor utility to use with MillionDance.

[Manual](https://github.com/OpenMLTD/MLTDTools/wiki/TDFacial-Manual)

## Contributing

Contributions are [Welcome!!](https://www.project-imas.com/wiki/Welcome!!)

## License

BSD 3-Clause Clear License
