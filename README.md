# MLTDTools (MiriTore)

Tools for [THE iDOLM@STER Million Live! Theater Days](https://millionlive.idolmaster.jp/theaterdays/) (MiriShita/ミリシタ). A sister project of [OpenCGSS/DereTore](https://github.com/OpenCGSS/DereTore).

[![AppVeyor](https://img.shields.io/appveyor/ci/hozuki/mltdtools.svg)](https://ci.appveyor.com/project/hozuki/mltdtools)
[![GitHub contributors](https://img.shields.io/github/contributors/OpenMLTD/MLTDTools.svg)](https://github.com/OpenMLTD/MLTDTools/graphs/contributors)
[![Libraries.io for GitHub](https://img.shields.io/librariesio/github/OpenMLTD/MLTDTools.svg)](https://github.com/OpenMLTD/MLTDTools)
[![GitHub (pre-)release](https://img.shields.io/github/release/OpenMLTD/MLTDTools/all.svg)](https://github.com/OpenMLTD/MLTDTools/releases)
[![Github All Releases](https://img.shields.io/github/downloads/OpenMLTD/MLTDTools/total.svg)](https://github.com/OpenMLTD/MLTDTools/releases)

**Downloads:**

- [Nightly build](https://ci.appveyor.com/api/projects/hozuki/mltdtools/artifacts/miritore-appveyor-latest.zip) (Windows)
- [Releases](https://github.com/OpenMLTD/MLTDTools/releases)

This repository:

- contains some common tools to explore possibilities.
- does not serve as a card/event/CG/commu database. If you want that please visit [ミリシタDB](http://imas.gamedbs.jp/mlth/).
- DOES NOT PROVIDE TOOLS FOR YOU TO CHEAT. **Cheating is not fun at all. It must be punished.**

For the Unity project for resource generation, please visit [PlatinumTheater](https://github.com/OpenMLTD/PlatinumTheater).

## Details

The projects include some utilities to investigate MLTD data structures.

**ACB Packer**

Similar to [AcbMaker](https://github.com/OpenCGSS/DereTore/tree/master/Apps/AcbMaker) in DereTore, but packs live music into MLTD variant of ACB format.

**Hca Decoder**

Similar to [Hca2Wav](https://github.com/OpenCGSS/DereTore/tree/master/Apps/Hca2Wav) in DereTore, but uses MLTD's HCA cipher by default.

**Manifest Exporter**

Exports manifest description to a text file.

**MLTD Information Viewer**

Views resource manifest, card information and costume information from MLTD database files.

**Scenario Editor**

WIP

**MillionDance**

Exports models, character and camera motions to MMD equivalents. (alpha)

Note:

Ouput data tested in [MikuMikuMoving](https://sites.google.com/site/mikumikumovingeng/) (MMM) but not [MikuMikuDance](http://www.geocities.jp/higuchuu4/index_e.htm) (MMD) because the number of frames is huge.
It is suggested to use MMM for enhanced performance. Also, some conversion seems to fail only when the output is used with MMD; maybe I'll
investigate this issue some day.

**MLTD Dance Viewer**

A simple viewer for MLTD dancing data, which allows directly reading model and motion
data extracted from MLTD.

Theoretically it can be applied to other Unity games, with a little modification...

*Requires OpenGL 4.0+*

## Contributing

Contributions are [Welcome!!](https://www.project-imas.com/wiki/Welcome!!)

## License

BSD 3-Clause Clear License
