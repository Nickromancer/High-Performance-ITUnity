using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text Text;

    private Dictionary<int, string> CachedNumberStrings = new();
    private int[] _frameRateSamples;
    private int _cacheNumbersAmount = 300;
    private int _averageFromAmount = 30;
    private int _averageCounter = 0;
    private int _currentAveraged;

    void Awake()
    {
        // Cache strings and create array
        {
            for (int i = 0; i < _cacheNumbersAmount; i++)
            {
                CachedNumberStrings[i] = i.ToString();
            }
            _frameRateSamples = new int[_averageFromAmount];
        }
    }
    void Update()
    {
        // Sample
        {
            var currentFrame = (int)Math.Round(1f / Time.deltaTime); // If your game modifies Time.timeScale, use unscaledDeltaTime and smooth manually (or not).
            _frameRateSamples[_averageCounter] = currentFrame;
        }

        // Average
        {
            var average = 0f;

            foreach (var frameRate in _frameRateSamples)
            {
                average += frameRate;
            }

            _currentAveraged = (int)Math.Round(average / _averageFromAmount);
            _averageCounter = (_averageCounter + 1) % _averageFromAmount;
        }

        // Assign to UI
        {
            // string fpsString = _currentAveraged switch
            // {
            //     var x when x >= 0 && x < _cacheNumbersAmount * 1000 => CachedNumberStrings[x],
            //     var x when x >= _cacheNumbersAmount * 1000 => $"> {_cacheNumbersAmount * 1000}",
            //     var x when x < 0 => "< 0",
            //     _ => "?"
            // };
            float msPerFrame = _currentAveraged > 0 ? 1000f / _currentAveraged : 0f;
            Text.text = $"FPS: {_currentAveraged} | {msPerFrame:F2} ms";
            //Text.text = $"FPS: {currentFrame}";
        }
    }
}