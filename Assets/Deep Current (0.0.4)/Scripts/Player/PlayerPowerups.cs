using UnityEngine;

namespace DeepCurrent
{
    // Holds the live power-up timers for the player.
    // PlayerController asks it for the shield, PlayerShooting asks it for rapid and spread.
    // The ring sprite is shown around the hull while the shield is up.
    public class PlayerPowerups : MonoBehaviour
    {
        public bool ShieldActive => shieldTime > 0f;
        public bool RapidActive => rapidTime > 0f;
        public bool SpreadActive => spreadTime > 0f;

        float shieldTime;
        float rapidTime;
        float spreadTime;
        GameObject ring;

        void Awake()
        {
            ring = BuildRing();
        }

        // The sonar ring is built in code so the player prefab stays simple.
        GameObject BuildRing()
        {
            var go = new GameObject("ShieldRing");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Sprites/spr_shield_ring");
            if (sr.sprite == null)
                Debug.LogWarning("[DeepCurrent] Missing sprite: Sprites/spr_shield_ring");
            sr.sortingOrder = 19;
            go.SetActive(false);
            return go;
        }

        void Update()
        {
            float dt = SimTime.Delta;
            shieldTime = Mathf.Max(0f, shieldTime - dt);
            rapidTime = Mathf.Max(0f, rapidTime - dt);
            spreadTime = Mathf.Max(0f, spreadTime - dt);

            if (ring != null) ring.SetActive(ShieldActive);
        }

        public void ActivateShield(float seconds) => shieldTime = seconds;
        public void ActivateRapid(float seconds) => rapidTime = seconds;
        public void ActivateSpread(float seconds) => spreadTime = seconds;

        // Called on a new run so old timers and the ring never leak into it.
        public void ResetPowerups()
        {
            shieldTime = 0f;
            rapidTime = 0f;
            spreadTime = 0f;
            if (ring != null) ring.SetActive(false);
        }

        // One short line for the HUD, such as "SHIELD 4.2s   RAPID 5.0s". Empty when no power is active.
        public string StatusLine()
        {
            string line = "";
            if (ShieldActive) line += "SHIELD " + shieldTime.ToString("0.0") + "s   ";
            if (RapidActive) line += "RAPID " + rapidTime.ToString("0.0") + "s   ";
            if (SpreadActive) line += "WIDE " + spreadTime.ToString("0.0") + "s   ";
            return line.TrimEnd();
        }
    }
}
