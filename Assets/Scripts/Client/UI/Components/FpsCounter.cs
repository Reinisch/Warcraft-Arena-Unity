using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

namespace Client
{
    public class FpsCounter : MonoBehaviour
    {
        // Latency is sampled once per display interval; average the last few seconds so the readout doesn't
        // jitter (transport RTT is noisy, especially over Relay and with two editors sharing one machine).
        private const int LatencySampleCount = 8;

        [SerializeField, UsedImplicitly] private TextMeshProUGUI fpsLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI rttLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI wireLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI latencyHintLabel;
        [SerializeField, UsedImplicitly] private float alpha = 0.5f;
        [SerializeField, UsedImplicitly] private float updateInterval = 0.33f;

        // Optional: only present in an injected context. Gates + feeds the latency display; null-safe so the
        // counter still works as a plain FPS readout if shown outside a session.
        [Inject(Optional = true), UsedImplicitly] private GameSession gameSession;

        private readonly char[] fpsCharArray = Array.Empty<char>();
        private readonly RollingAverage rttAverage = new RollingAverage(LatencySampleCount);
        private readonly RollingAverage wireAverage = new RollingAverage(LatencySampleCount);
        private float timeScaledFrames;
        private int intervalFrames;
        private float intervalTimeLeft;
        private bool latencyShown;

        [UsedImplicitly]
        private void Start()
        {
            intervalTimeLeft = updateInterval;
            if (latencyHintLabel != null)
                latencyHintLabel.text = "rtt felt / net est.";
            latencyShown = true; // force the first SetLatencyVisible to apply
            SetLatencyVisible(false);
        }

        [UsedImplicitly]
        private void Update()
        {
            intervalTimeLeft -= Time.deltaTime;
            timeScaledFrames += Time.timeScale / Time.deltaTime;
            ++intervalFrames;

            if (intervalTimeLeft > 0.0)
                return;

            UpdateFps();
            UpdateLatency();

            intervalTimeLeft = updateInterval;
            timeScaledFrames = 0.0F;
            intervalFrames = 0;
        }

        private void UpdateFps()
        {
            int fps = Mathf.FloorToInt(timeScaledFrames / intervalFrames);
            fpsLabel.SetCharArray(fpsCharArray.SetIntNonAlloc(fps, out int length), 0, length);
            fpsLabel.color = Tint(fps >= 55 ? Color.green : fps >= 30 ? Color.yellow : Color.red);
        }

        // Two rolling-averaged numbers, shown only while connected to a remote server (no meaningful ping to
        // self on a host/single-player). rtt = the round trip the player feels; net = that minus NGO's local
        // send-tick batching, i.e. roughly the real wire round trip. Lower is better, so colours invert FPS.
        private void UpdateLatency()
        {
            bool show = gameSession != null && gameSession.IsRemoteClient;
            SetLatencyVisible(show);
            if (!show)
            {
                rttAverage.Reset();
                wireAverage.Reset();
                return;
            }

            int rtt = rttAverage.Add(gameSession.RoundTripTimeMs);
            if (rttLabel != null)
            {
                rttLabel.SetText("rtt {0} ms", rtt);
                rttLabel.color = Tint(LatencyColor(rtt));
            }

            int wire = wireAverage.Add(gameSession.EstimatedWireLatencyMs);
            if (wireLabel != null)
            {
                wireLabel.SetText("net {0} ms", wire);
                wireLabel.color = Tint(LatencyColor(wire));
            }
        }

        private void SetLatencyVisible(bool show)
        {
            if (latencyShown == show)
                return;
            latencyShown = show;

            if (rttLabel != null) rttLabel.enabled = show;
            if (wireLabel != null) wireLabel.enabled = show;
            if (latencyHintLabel != null) latencyHintLabel.enabled = show;
        }

        private static Color LatencyColor(int ms) => ms <= 80 ? Color.green : ms <= 150 ? Color.yellow : Color.red;

        private Color Tint(Color color) => new Color(color.r, color.g, color.b, alpha);

        // Fixed-window moving average over the last N integer samples (circular buffer, O(1) per add).
        private sealed class RollingAverage
        {
            private readonly int[] samples;
            private int count;
            private int index;
            private long sum;

            public RollingAverage(int size) => samples = new int[Mathf.Max(1, size)];

            public int Add(int value)
            {
                sum += value - samples[index];
                samples[index] = value;
                index = (index + 1) % samples.Length;
                if (count < samples.Length)
                    count++;
                return (int)(sum / count);
            }

            public void Reset()
            {
                Array.Clear(samples, 0, samples.Length);
                count = 0;
                index = 0;
                sum = 0;
            }
        }
    }
}
