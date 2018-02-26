using System;
using UnityEngine;

namespace Imas.Live {

    [Serializable]
    public sealed class ScenarioScrObj : ScriptableObject {

        public EventScenarioData[] scenario = {
            new EventScenarioData {
                absTime = 1,
                beat = 4,
                duration = 1000
            },
            new EventScenarioData()
        };

        public TexTargetName[] texs = {
            new TexTargetName {
                name = "vj_white",
                target = 20
            },
            new TexTargetName {
                name = "vj_cool_01",
                target = 20
            },
            new TexTargetName {
                name = "ltmap_core01",
                target = 13
            },
            new TexTargetName {
                name = "vj_cool_03",
                target = 20
            },
        };

        public EventScenarioData ap_st = new EventScenarioData();

        public EventScenarioData ap_pose = new EventScenarioData();

        public EventScenarioData ap_end = new EventScenarioData();

        public EventScenarioData fine_ev = new EventScenarioData();

    }

}
