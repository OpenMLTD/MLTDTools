# MLTDTools

Tools for [THE iDOLM@STER Million Live! Theater Days](https://millionlive.idolmaster.jp/theaterdays/) (MiriShita/ミリシタ). A sister project of [OpenCGSS/DereTore](https://github.com/OpenCGSS/DereTore).

This repository:

- contains some common tools to explore possibilities.
- does not serve as a card/event/CG/commu database. If you want that please visit [ミリシタDB](http://imas.gamedbs.jp/mlth/).
- DOES NOT PROVIDE TOOLS FOR YOU TO CHEAT. **Cheating is not fun at all, and it must be punished.**

Some sensitive information is hidden so some projects may not compile if you simply clone this repo.

## Details

### Visual Studio Projects

The projects include some utilities to investigate MLTD data structures.

### Unity Project

The Unity project demonstrates how to build custom music and beatmap playable in MLTD.

- The score source is from custom beatmap [Death by Glamour](http://undertale.wikia.com/wiki/Death_by_Glamour) ([here](https://www.bilibili.com/video/av15612246/) is its preview) while the data is built into a beatmap for [Blue Symphony](https://www.project-imas.com/wiki/Blue_Symphony). You can check this by running the game. Scenario data (including mouth sync/morph, UI animations, etc.) is from [Shooting Stars](https://www.project-imas.com/wiki/Shooting_Stars), so you will see mismatches in various UI elements. There's an alternative file from Shooting Stars. Replace the content in `blsymp_fumen_sobj.txt` with the content in `shtstr_fumen_sobj.txt` to work.
- Blue Symphony's ACB file contains audio data from Death by Glamour. An alternative test contains [Brand New Theater!](https://www.project-imas.com/wiki/Brand_New_Theater!).
- Blue Symphony's song title is changed to an image writing "Death by Glamour". The most important parameters are sprite names (`songname_1` and `songname_2`) and sizes (256x72 and 84x72).

To run the Unity project you need Unity 5.4+, but NO LATER THAN 5.6.x. Recommended version is 5.6.2f1, which is also the one that MLTD is built with.

## Contributing

Contributions are [Welcome!!](https://www.project-imas.com/wiki/Welcome!!)

## License

BSD 3-Clause Clear License

External references:

- [YamlDotNet for Unity](https://assetstore.unity.com/packages/tools/integration/yamldotnet-for-unity-36292) is the work of Beetle Circus.
- [Standalone File Browser](https://github.com/gkngkc/UnityStandaloneFileBrowser) is the work of [Gökhan Gökçe](https://github.com/gkngkc) and contributors.
- `ThankYou` is an adaptation of Lawrence Gripper's [FiddlerDnsForwarder](https://github.com/lawrencegripper/FiddlerDnsForwarder).
